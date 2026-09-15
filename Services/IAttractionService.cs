using Models.Dto;

namespace Services;

public interface IAttractionService
{
    public Task<PagedResult<AttractionListItemDto>> ReadAllAsync(
        string category,
        string title,
        string description,
        string country,
        string city,
        bool includeComments = false,
        int pageNumber = 1,
        int pageSize = 20
    );

    public Task<PagedResult<AttractionListItemDto>> ReadAllWithoutCommentsAsync(
        int pageNumber = 1,
        int pageSize = 20
    );

    public Task<AttractionDetailDto> ReadItemAsync(
        Guid attractionId,
        int commentsPageNumber = 1,
        int commentsPageSize = 20
    );
}
