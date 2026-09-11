using Models.Dto;

namespace Services;

public interface IAttractionService
{
    public Task<PagedResult<AttractionListItemDto>> GetFilteredAsync(
        string category,
        string title,
        string description,
        string country,
        string city,
        int pageNumber = 1,
        int pageSize = 20
    );

    public Task<PagedResult<AttractionListItemDto>> GetWithoutCommentsAsync(
        int pageNumber = 1,
        int pageSize = 20
    );

    public Task<AttractionDetailDto> GetDetailAsync(
        Guid attractionId,
        int commentsPageNumber = 1,
        int commentsPageSize = 20
    );
}
