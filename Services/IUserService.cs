using Models.Dto;

namespace Services;

public interface IUserService
{
    public Task<PagedResult<UsersDto>> GetAllUsersAsync(
        bool includeComments = true,
        int pageNumber = 1,
        int pageSize = 20
    );
}
