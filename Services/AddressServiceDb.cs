using DbRepos;
using Microsoft.Extensions.Logging;
using Models;
using Models.Dto;

namespace Services;

public class AddressServiceDb : IAddressService
{
    private readonly AddressDbRepos _repo;
    private readonly ILogger<AddressServiceDb> _logger;

    public Task<ResponseItemDto<IAddress>> ReadAddressAsync(Guid id, bool flat) =>
        _repo.ReadAddressAsync(id, flat);

    public Task<ResponsePageDto<IAddress>> ReadAddressesAsync(
        bool seeded,
        bool flat,
        string filter,
        int pageNumber,
        int pageSize
    ) => _repo.ReadAddressesAsync(seeded, flat, filter, pageNumber, pageSize);

    public Task<ResponseItemDto<IAddress>> CreateAddressAsync(AddressCuDto itemDto) =>
        _repo.CreateAddressAsync(itemDto);

    public Task<ResponseItemDto<IAddress>> UpdateAddressAsync(AddressCuDto itemDto) =>
        _repo.UpdateAddressAsync(itemDto);

    public Task<ResponseItemDto<IAddress>> DeleteAddressAsync(Guid id) =>
        _repo.DeleteAddressAsync(id);

    #region constructors
    public AddressServiceDb(AddressDbRepos repo)
    {
        _repo = repo;
    }

    public AddressServiceDb(AddressDbRepos repo, ILogger<AddressServiceDb> logger)
        : this(repo)
    {
        _logger = logger;
    }
    #endregion
}
