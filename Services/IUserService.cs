using Models;
using Models.Dto;

namespace Services;

public interface IUserService
{
    public Task<ResponsePageDto<IUser>> ReadUsersAsync(
        bool seeded,
        bool flat,
        string filter,
        int pageNumber,
        int pageSize
    );
}
