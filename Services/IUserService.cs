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
    public Task<ResponseItemDto<IUser>> CreateUserAsync(UserCuDto itemDto);
    public Task<ResponseItemDto<IUser>> UpdateUserAsync(UserCuDto itemDto);
    public Task<ResponseItemDto<IUser>> DeleteUserAsync(Guid id);
}
