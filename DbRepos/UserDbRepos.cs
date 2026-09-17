using DbContext;
using DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.Dto;

namespace DbRepos;

public class UserDbRepos
{
    private readonly ILogger<UserDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    // Task 6, bullet 4: all users and the comments each of them has posted.
    // flat=false includes each user's Comments (and each comment's Attraction, for its title).
    public async Task<ResponsePageDto<IUser>> ReadUsersAsync(
        bool seeded,
        bool flat,
        string filter,
        int pageNumber,
        int pageSize
    )
    {
        filter ??= "";

        IQueryable<UserDbM> query;
        if (flat)
        {
            query = _dbContext.Users.AsNoTracking();
        }
        else
        {
            query = _dbContext
                .Users.AsNoTracking()
                .Include(u => u.Comments)
                    .ThenInclude(c => c.Attraction);
        }

        query = query.Where(u =>
            (u.Seeded == seeded)
            && (
                u.Name.ToLower().Contains(filter.ToLower())
                || u.Email.ToLower().Contains(filter.ToLower())
            )
        );

        return new ResponsePageDto<IUser>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            DbItemsCount = await query.CountAsync(),
            PageItems = await query
                .OrderBy(u => u.Name)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync<IUser>(),
            PageNr = pageNumber,
            PageSize = pageSize,
        };
    }

    public UserDbRepos(ILogger<UserDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }
}
