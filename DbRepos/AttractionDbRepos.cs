using DbContext;
using DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Dto;

namespace DbRepos;

public class AttractionDbRepos
{
    private readonly ILogger<AttractionDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    // Task 6, bullet 1: filter attractions by category, title, description, country and city.
    // All filters are optional - only the ones actually provided (non-empty) are applied, combined with AND.
    // includeComments: when true, each item also carries its full list of comments (with author name).
    public async Task<PagedResult<AttractionListItemDto>> GetFilteredAsync(
        string category,
        string title,
        string description,
        string country,
        string city,
        bool includeComments,
        int pageNumber,
        int pageSize
    )
    {
        var query = _dbContext
            .Attractions.Include(a => a.Category)
            .Include(a => a.City)
                .ThenInclude(c => c.Country)
            .Include(a => a.Comments)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(a => a.Category.Name.Contains(category));

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(a => a.Title.Contains(title));

        if (!string.IsNullOrWhiteSpace(description))
            query = query.Where(a => a.Description.Contains(description));

        if (!string.IsNullOrWhiteSpace(country))
            query = query.Where(a => a.City.Country.Name.Contains(country));

        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(a => a.City.Name.Contains(city));

        return await ToPagedListItemsAsync(query, includeComments, pageNumber, pageSize);
    }

    // Task 6, bullet 2: attractions that have zero comments.
    public async Task<PagedResult<AttractionListItemDto>> GetWithoutCommentsAsync(
        bool includeComments,
        int pageNumber,
        int pageSize
    )
    {
        var query = _dbContext
            .Attractions.Include(a => a.Category)
            .Include(a => a.City)
                .ThenInclude(c => c.Country)
            .Include(a => a.Comments)
            .Where(a => !a.Comments.Any());

        // note: includeComments is kept here for API symmetry, but since these attractions have
        // zero comments by definition, the resulting Comments list will always be empty.
        return await ToPagedListItemsAsync(query, includeComments, pageNumber, pageSize);
    }

    // Task 6, bullet 3: one attraction's category, title, description, and all its comments (paginated).
    public async Task<AttractionDetailDto> GetDetailAsync(
        Guid attractionId,
        int commentsPageNumber,
        int commentsPageSize
    )
    {
        var attraction = await _dbContext
            .Attractions.Include(a => a.Category)
            .Include(a => a.City)
                .ThenInclude(c => c.Country)
            .FirstOrDefaultAsync(a => a.AttractionId == attractionId);

        if (attraction == null)
            return null;

        var commentsQuery = _dbContext
            .Comments.Include(c => c.User)
            .Where(c => c.AttractionId == attractionId)
            .OrderByDescending(c => c.CreatedAt);

        var totalComments = await commentsQuery.CountAsync();
        var pageOfComments = await commentsQuery
            .Skip((commentsPageNumber - 1) * commentsPageSize)
            .Take(commentsPageSize)
            .Select(c => new CommentDto
            {
                CommentId = c.CommentId,
                Text = c.Text,
                CreatedAt = c.CreatedAt,
                UserId = c.UserId,
                UserName = c.User.Name,
            })
            .ToListAsync();

        return new AttractionDetailDto
        {
            AttractionId = attraction.AttractionId,
            Title = attraction.Title,
            Description = attraction.Description,
            Address = attraction.Address,
            Category = attraction.Category.Name,
            City = attraction.City.Name,
            Country = attraction.City.Country.Name,
            Comments = new PagedResult<CommentDto>
            {
                Items = pageOfComments,
                PageNumber = commentsPageNumber,
                PageSize = commentsPageSize,
                TotalCount = totalComments,
            },
        };
    }

    private async Task<PagedResult<AttractionListItemDto>> ToPagedListItemsAsync(
        IQueryable<AttractionDbM> query,
        bool includeComments,
        int pageNumber,
        int pageSize
    )
    {
        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(a => a.Title)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new AttractionListItemDto
            {
                AttractionId = a.AttractionId,
                Title = a.Title,
                Description = a.Description,
                Category = a.Category.Name,
                City = a.City.Name,
                Country = a.City.Country.Name,
                CommentCount = a.Comments.Count,
                Comments = a
                    .Comments.Select(c => new CommentDto
                    {
                        CommentId = c.CommentId,
                        Text = c.Text,
                        CreatedAt = c.CreatedAt,
                        UserId = c.UserId,
                        UserName = c.User.Name,
                    })
                    .ToList(),
            })
            .ToListAsync();

        if (!includeComments)
            foreach (var item in items)
                item.Comments = null;

        return new PagedResult<AttractionListItemDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
        };
    }

    public AttractionDbRepos(ILogger<AttractionDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }
}
