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

    // Task 6, bullet 3: one attraction's category, title, description, and all its comments.
    // flat=false includes Category/City/Country and Comments+their User - everything the task asks for
    // in one graph. flat=true returns just the attraction's own scalar columns (no joins).
    public async Task<ResponseItemDto<IAttraction>> ReadAttractionAsync(Guid id, bool flat)
    {
        IAttraction item;

        if (!flat)
        {
            var query = _dbContext
                .Attractions.AsNoTracking()
                .Include(a => a.Category)
                .Include(a => a.City)
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

    // Task 6, bullet 1: filter attractions by category, title, description, country and city.
    // Every filter defaults to "" (matches everything) when not supplied.
    // Task 6, bullet 2 (attractions without comments) is covered by the separate onlyWithoutComments flag.
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
                .Include(a => a.City)
                    .ThenInclude(c => c.Country)
                .Include(a => a.Comments)
                    .ThenInclude(c => c.User);
        }

        // Filtering is done via navigation properties regardless of flat/Include - Include only controls
        // what gets materialized in the result, EF Core still translates these into SQL joins for the WHERE.
        query = query.Where(a =>
            (a.Seeded == seeded)
            && a.Category.Name.ToLower().Contains(category.ToLower())
            && a.Title.ToLower().Contains(title.ToLower())
            && a.Description.ToLower().Contains(description.ToLower())
            && a.City.Country.Name.ToLower().Contains(country.ToLower())
            && a.City.Name.ToLower().Contains(city.ToLower())
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

    public AttractionDbRepos(ILogger<AttractionDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }
}
