using Models;
using Models.Dto;

namespace Services;

public interface IAddressService
{
    public Task<ResponseItemDto<IAddress>> ReadAddressAsync(Guid id, bool flat);
    public Task<ResponsePageDto<IAddress>> ReadAddressesAsync(
        bool seeded,
        bool flat,
        string filter,
        int pageNumber,
        int pageSize
    );
}
