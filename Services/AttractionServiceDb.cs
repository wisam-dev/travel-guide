using DbRepos;
using Microsoft.Extensions.Logging;
using Models;
using Models.Dto;

namespace Services;

public class AttractionServiceDb : IAttractionService
{
    private readonly AttractionDbRepos _repo;
    private readonly ILogger<AttractionServiceDb> _logger;

    public Task<ResponseItemDto<IAttraction>> ReadAttractionAsync(Guid id, bool flat) =>
        _repo.ReadAttractionAsync(id, flat);

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
    ) =>
        _repo.ReadAttractionsAsync(
            seeded,
            flat,
            category,
            title,
            description,
            country,
            city,
            onlyWithoutComments,
            pageNumber,
            pageSize
        );

    public Task<ResponseItemDto<IAttraction>> CreateAttractionAsync(AttractionCuDto itemDto) =>
        _repo.CreateAttractionAsync(itemDto);

    public Task<ResponseItemDto<IAttraction>> UpdateAttractionAsync(AttractionCuDto itemDto) =>
        _repo.UpdateAttractionAsync(itemDto);

    public Task<ResponseItemDto<IAttraction>> DeleteAttractionAsync(Guid id) =>
        _repo.DeleteAttractionAsync(id);

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
