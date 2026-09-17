using DbContext;
using DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.Dto;

namespace DbRepos;

public class AddressDbRepos
{
    private readonly ILogger<AddressDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public async Task<ResponseItemDto<IAddress>> ReadAddressAsync(Guid id, bool flat)
    {
        IAddress item;

        if (!flat)
        {
            var query = _dbContext
                .Addresses.AsNoTracking()
                .Include(a => a.City)
                .Include(a => a.Country)
                .Where(a => a.AddressId == id);

            item = await query.FirstOrDefaultAsync<IAddress>();
        }
        else
        {
            var query = _dbContext.Addresses.AsNoTracking().Where(a => a.AddressId == id);

            item = await query.FirstOrDefaultAsync<IAddress>();
        }

        if (item == null)
            throw new ArgumentException($"Address {id} does not exist");

        return new ResponseItemDto<IAddress>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            Item = item,
        };
    }

    public async Task<ResponsePageDto<IAddress>> ReadAddressesAsync(
        bool seeded,
        bool flat,
        string filter,
        int pageNumber,
        int pageSize
    )
    {
        filter ??= "";

        IQueryable<AddressDbM> query;
        if (flat)
        {
            query = _dbContext.Addresses.AsNoTracking();
        }
        else
        {
            query = _dbContext
                .Addresses.AsNoTracking()
                .Include(a => a.City)
                .Include(a => a.Country);
        }

        query = query.Where(a =>
            (a.Seeded == seeded) && a.Street.ToLower().Contains(filter.ToLower())
        );

        return new ResponsePageDto<IAddress>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            DbItemsCount = await query.CountAsync(),
            PageItems = await query
                .OrderBy(a => a.Street)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync<IAddress>(),
            PageNr = pageNumber,
            PageSize = pageSize,
        };
    }

    public AddressDbRepos(ILogger<AddressDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }
}
