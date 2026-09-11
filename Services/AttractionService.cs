using DbRepos;
using Microsoft.Extensions.Logging;
using Models.Dto;

namespace Services;

public class AttractionServiceDb : IAttractionService
{
    private readonly AttractionDbRepos _repo;
    private readonly ILogger<AttractionServiceDb> _logger;

    public Task<PagedResult<AttractionListItemDto>> GetFilteredAsync(
        string category,
        string title,
        string description,
        string country,
        string city,
        int pageNumber = 1,
        int pageSize = 20
    ) => _repo.GetFilteredAsync(category, title, description, country, city, pageNumber, pageSize);

    public Task<PagedResult<AttractionListItemDto>> GetWithoutCommentsAsync(
        int pageNumber = 1,
        int pageSize = 20
    ) => _repo.GetWithoutCommentsAsync(pageNumber, pageSize);

    public Task<AttractionDetailDto> GetDetailAsync(
        Guid attractionId,
        int commentsPageNumber = 1,
        int commentsPageSize = 20
    ) => _repo.GetDetailAsync(attractionId, commentsPageNumber, commentsPageSize);

    #region constructors
    public AttractionServiceDb(AttractionDbRepos repo)
    {
        _repo = repo;
    }

    public AttractionServiceDb(AttractionDbRepos repo, ILogger<AttractionServiceDb> logger)
        : this(repo)
    {
        _logger = logger;
    }
    #endregion
}
