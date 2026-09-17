using Models;
using Models.Dto;

namespace Services;

public interface IAttractionService
{
    public Task<ResponseItemDto<IAttraction>> ReadAttractionAsync(Guid id, bool flat);

    public Task<ResponsePageDto<IAttraction>> ReadAttractionsAsync(
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
    );
}
