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
    // Pages the users first (cheap), then - only if includeComments is requested - fetches comments
    // for just those user ids in one flat query and stitches them in memory. A nested
    // `u.Comments.Select(...)` projection with a joined Attraction.Title generates a slow per-row
    // correlated subquery and can hang for a large Comments table.
    public async Task<PagedResult<UsersDto>> GetAllUsersAsync(
        bool includeComments,
        int pageNumber,
        int pageSize
    )
    {
        var query = _dbContext.Users.OrderBy(u => u.Name);

        var totalCount = await query.CountAsync();

        var pagedUsers = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UsersDto
            {
                UserId = u.UserId,
                Name = u.Name,
                Email = u.Email,
                CommentCount = u.Comments.Count,
                Comments = null,
            })
            .ToListAsync();

        if (includeComments && pagedUsers.Count > 0)
        {
            var userIds = pagedUsers.Select(u => u.UserId).ToList();

            var comments = await _dbContext
                .Comments.Where(c => userIds.Contains(c.UserId))
                .Select(c => new
                {
                    c.UserId,
                    Comment = new UserCommentDto
                    {
                        CommentId = c.CommentId,
                        Text = c.Text,
                        CreatedAt = c.CreatedAt,
                        AttractionId = c.AttractionId,
                        AttractionTitle = c.Attraction.Title,
                    },
                })
                .ToListAsync();

            var commentsByUser = comments.ToLookup(x => x.UserId, x => x.Comment);

            foreach (var user in pagedUsers)
                user.Comments = commentsByUser[user.UserId].ToList();
        }

        return new PagedResult<UsersDto>
        {
            Items = pagedUsers,
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
