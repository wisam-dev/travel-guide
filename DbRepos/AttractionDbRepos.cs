using DbContext;
using DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.Dto;

namespace DbRepos;

public class AttractionDbRepos
{
    private readonly ILogger<AttractionDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public async Task<ResponseItemDto<IAttraction>> ReadAttractionAsync(Guid id, bool flat)
    {
        IAttraction item;

        if (!flat)
        {
            var query = _dbContext
                .Attractions.AsNoTracking()
                .Include(a => a.Category)
                .Include(a => a.Address)
                    .ThenInclude(addr => addr.City)
                        .ThenInclude(c => c.Country)
                .Include(a => a.Comments)
                    .ThenInclude(c => c.User)
                .Where(a => a.AttractionId == id);

            item = await query.FirstOrDefaultAsync<IAttraction>();
        }
        else
        {
            var query = _dbContext.Attractions.AsNoTracking().Where(a => a.AttractionId == id);

            item = await query.FirstOrDefaultAsync<IAttraction>();
        }

        if (item == null)
            throw new ArgumentException($"Attraction {id} does not exist");

        return new ResponseItemDto<IAttraction>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            Item = item,
        };
    }

    public async Task<ResponsePageDto<IAttraction>> ReadAttractionsAsync(
        bool seeded,
        bool flat,
        string category,
        string title,
        string description,
        string country,
        string city,
        bool onlyWithoutComments,
        int pageNumber,
        int pageSize
    )
    {
        category ??= "";
        title ??= "";
        description ??= "";
        country ??= "";
        city ??= "";

        IQueryable<AttractionDbM> query;
        if (flat)
        {
            query = _dbContext.Attractions.AsNoTracking();
        }
        else
        {
            query = _dbContext
                .Attractions.AsNoTracking()
                .Include(a => a.Category)
                .Include(a => a.Address)
                    .ThenInclude(addr => addr.City)
                        .ThenInclude(c => c.Country)
                .Include(a => a.Comments)
                    .ThenInclude(c => c.User);
        }

        query = query.Where(a =>
            (a.Seeded == seeded)
            && a.Category.Name.ToLower().Contains(category.ToLower())
            && a.Title.ToLower().Contains(title.ToLower())
            && a.Description.ToLower().Contains(description.ToLower())
            && a.Address.City.Country.Name.ToLower().Contains(country.ToLower())
            && a.Address.City.Name.ToLower().Contains(city.ToLower())
        );

        if (onlyWithoutComments)
            query = query.Where(a => !a.Comments.Any());

        return new ResponsePageDto<IAttraction>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            DbItemsCount = await query.CountAsync(),
            PageItems = await query
                .OrderBy(a => a.Title)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync<IAttraction>(),
            PageNr = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<ResponseItemDto<IAttraction>> CreateAttractionAsync(AttractionCuDto itemDto)
    {
        if (itemDto.AttractionId != null)
            throw new ArgumentException(
                $"{nameof(itemDto.AttractionId)} must be null when creating a new object"
            );

        var address = await _dbContext.Addresses.FirstOrDefaultAsync(a =>
            a.AddressId == itemDto.AddressId
        );
        if (address == null)
            throw new ArgumentException($"Address {itemDto.AddressId} does not exist");

        var category = await _dbContext.Categories.FirstOrDefaultAsync(c =>
            c.CategoryId == itemDto.CategoryId
        );
        if (category == null)
            throw new ArgumentException($"Category {itemDto.CategoryId} does not exist");

        var attraction = new AttractionDbM(itemDto);
        _dbContext.Attractions.Add(attraction);
        await _dbContext.SaveChangesAsync();

        return await ReadAttractionAsync(attraction.AttractionId, false);
    }

    // Country/city changes happen by pointing AddressId at a different (already-existing) address.
    public async Task<ResponseItemDto<IAttraction>> UpdateAttractionAsync(AttractionCuDto itemDto)
    {
        var attraction = await _dbContext.Attractions.FirstOrDefaultAsync(a =>
            a.AttractionId == itemDto.AttractionId
        );
        if (attraction == null)
            throw new ArgumentException($"Attraction {itemDto.AttractionId} does not exist");

        var address = await _dbContext.Addresses.FirstOrDefaultAsync(a =>
            a.AddressId == itemDto.AddressId
        );
        if (address == null)
            throw new ArgumentException($"Address {itemDto.AddressId} does not exist");

        var category = await _dbContext.Categories.FirstOrDefaultAsync(c =>
            c.CategoryId == itemDto.CategoryId
        );
        if (category == null)
            throw new ArgumentException($"Category {itemDto.CategoryId} does not exist");

        attraction.UpdateFromDTO(itemDto);

        _dbContext.Attractions.Update(attraction);
        await _dbContext.SaveChangesAsync();

        return await ReadAttractionAsync(attraction.AttractionId, false);
    }

    public async Task<ResponseItemDto<IAttraction>> DeleteAttractionAsync(Guid id)
    {
        var attraction = await _dbContext
            .Attractions.Include(a => a.Address)
            .FirstOrDefaultAsync(a => a.AttractionId == id);
        if (attraction == null)
            throw new ArgumentException($"Attraction {id} does not exist");

        var address = attraction.Address;

        _dbContext.Attractions.Remove(attraction);
        await _dbContext.SaveChangesAsync();

        if (address != null)
        {
            _dbContext.Addresses.Remove(address);
            await _dbContext.SaveChangesAsync();
        }

        return new ResponseItemDto<IAttraction>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            Item = attraction,
        };
    }

    public AttractionDbRepos(ILogger<AttractionDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }
}
