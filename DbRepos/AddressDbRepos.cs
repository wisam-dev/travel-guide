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

    public async Task<ResponseItemDto<IAddress>> CreateAddressAsync(AddressCuDto itemDto)
    {
        if (itemDto.AddressId != null)
            throw new ArgumentException(
                $"{nameof(itemDto.AddressId)} must be null when creating a new object"
            );

        var city = await _dbContext.Cities.FirstOrDefaultAsync(c => c.CityId == itemDto.CityId);
        if (city == null)
            throw new ArgumentException($"City {itemDto.CityId} does not exist");

        // no duplicates: same street+zip+city
        var existing = await _dbContext.Addresses.FirstOrDefaultAsync(a =>
            a.Street == itemDto.Street && a.ZipCode == itemDto.ZipCode && a.CityId == itemDto.CityId
        );
        if (existing != null)
            throw new ArgumentException($"Address already exists with id {existing.AddressId}");

        var item = new AddressDbM(itemDto) { CountryId = city.CountryId };

        _dbContext.Addresses.Add(item);
        await _dbContext.SaveChangesAsync();

        return await ReadAddressAsync(item.AddressId, false);
    }

    public async Task<ResponseItemDto<IAddress>> UpdateAddressAsync(AddressCuDto itemDto)
    {
        var item = await _dbContext.Addresses.FirstOrDefaultAsync(a =>
            a.AddressId == itemDto.AddressId
        );
        if (item == null)
            throw new ArgumentException($"Address {itemDto.AddressId} does not exist");

        var city = await _dbContext.Cities.FirstOrDefaultAsync(c => c.CityId == itemDto.CityId);
        if (city == null)
            throw new ArgumentException($"City {itemDto.CityId} does not exist");

        var existing = await _dbContext.Addresses.FirstOrDefaultAsync(a =>
            a.Street == itemDto.Street && a.ZipCode == itemDto.ZipCode && a.CityId == itemDto.CityId
        );
        if (existing != null && existing.AddressId != itemDto.AddressId)
            throw new ArgumentException($"Address already exists with id {existing.AddressId}");

        item.UpdateFromDTO(itemDto);
        item.CountryId = city.CountryId;

        _dbContext.Addresses.Update(item);
        await _dbContext.SaveChangesAsync();

        return await ReadAddressAsync(item.AddressId, false);
    }

    public async Task<ResponseItemDto<IAddress>> DeleteAddressAsync(Guid id)
    {
        var item = await _dbContext.Addresses.FirstOrDefaultAsync(a => a.AddressId == id);
        if (item == null)
            throw new ArgumentException($"Address {id} does not exist");

        _dbContext.Addresses.Remove(item);
        await _dbContext.SaveChangesAsync();

        return new ResponseItemDto<IAddress>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            Item = item,
        };
    }

    public AddressDbRepos(ILogger<AddressDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }
}
