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

    public async Task<ResponseItemDto<IUser>> CreateUserAsync(UserCuDto itemDto)
    {
        if (itemDto.UserId != null)
            throw new ArgumentException(
                $"{nameof(itemDto.UserId)} must be null when creating a new object"
            );

        var existing = await _dbContext.Users.FirstOrDefaultAsync(u =>
            u.Email.ToLower() == itemDto.Email.ToLower()
        );
        if (existing != null)
            throw new ArgumentException(
                $"A user with email {itemDto.Email} already exists (id {existing.UserId})"
            );

        var item = new UserDbM(itemDto);

        _dbContext.Users.Add(item);
        await _dbContext.SaveChangesAsync();

        return new ResponseItemDto<IUser>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            Item = item,
        };
    }

    public async Task<ResponseItemDto<IUser>> UpdateUserAsync(UserCuDto itemDto)
    {
        var item = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == itemDto.UserId);
        if (item == null)
            throw new ArgumentException($"User {itemDto.UserId} does not exist");

        var existing = await _dbContext.Users.FirstOrDefaultAsync(u =>
            u.Email.ToLower() == itemDto.Email.ToLower()
        );
        if (existing != null && existing.UserId != itemDto.UserId)
            throw new ArgumentException(
                $"A user with email {itemDto.Email} already exists (id {existing.UserId})"
            );

        item.UpdateFromDTO(itemDto);

        _dbContext.Users.Update(item);
        await _dbContext.SaveChangesAsync();

        return new ResponseItemDto<IUser>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            Item = item,
        };
    }

    public async Task<ResponseItemDto<IUser>> DeleteUserAsync(Guid id)
    {
        var item = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == id);
        if (item == null)
            throw new ArgumentException($"User {id} does not exist");

        _dbContext.Users.Remove(item);
        await _dbContext.SaveChangesAsync();

        return new ResponseItemDto<IUser>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            Item = item,
        };
    }

    public UserDbRepos(ILogger<UserDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }
}
