using DbRepos;
using Microsoft.Extensions.Logging;
using Models.Dto;

namespace Services;

public class AttractionServiceDb : IAttractionService
{
    private readonly AttractionDbRepos _repo;
    private readonly ILogger<AttractionServiceDb> _logger;

    public Task<PagedResult<AttractionListItemDto>> ReadAllAsync(
        string category,
        string title,
        string description,
        string country,
        string city,
        bool includeComments = false,
        int pageNumber = 1,
        int pageSize = 20
    ) =>
        _repo.ReadAllAsync(
            category,
            title,
            description,
            country,
            city,
            includeComments,
            pageNumber,
            pageSize
        );

    public Task<PagedResult<AttractionListItemDto>> ReadAllWithoutCommentsAsync(
        int pageNumber = 1,
        int pageSize = 20
    ) => _repo.ReadAllWithoutCommentsAsync(pageNumber, pageSize);

    public Task<AttractionDetailDto> ReadItemAsync(
        Guid attractionId,
        int commentsPageNumber = 1,
        int commentsPageSize = 20
    ) => _repo.ReadItemAsync(attractionId, commentsPageNumber, commentsPageSize);

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
