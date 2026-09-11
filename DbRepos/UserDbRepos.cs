using DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Dto;

namespace DbRepos;

public class UserDbRepos
{
    private readonly ILogger<UserDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    // Task 6, bullet 4: all users and the comments each of them has posted.
    // Users are paginated; each user's own comment list is returned in full (comment counts per user
    // are naturally small - seeding spreads ~0-20 comments per attraction across all users).
    public async Task<PagedResult<UsersDto>> GetAllUsersAsync(
        bool includeComments,
        int pageNumber,
        int pageSize
    )
    {
        var query = _dbContext
            .Users.Include(u => u.Comments)
                .ThenInclude(c => c.Attraction)
            .OrderBy(u => u.Name)
            .Include(u => u.Comments)
            .AsQueryable();

        // if (includeComments)
        //     query = query.Include(u => u.Comments);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UsersDto
            {
                UserId = u.UserId,
                Name = u.Name,
                Email = u.Email,
                Comments = u
                    .Comments.Select(c => new UserCommentDto
                    {
                        CommentId = c.CommentId,
                        Text = c.Text,
                        CreatedAt = c.CreatedAt,
                        AttractionId = c.AttractionId,
                        AttractionTitle = c.Attraction.Title,
                    })
                    .ToList(),
            })
            .ToListAsync();

        return new PagedResult<UsersDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
        };
    }

    public UserDbRepos(ILogger<UserDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }
}
