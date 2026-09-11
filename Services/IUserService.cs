using Models.Dto;

namespace Services;

public interface IUserService
{
    public Task<PagedResult<UserWithCommentsDto>> GetAllWithCommentsAsync(
        int pageNumber = 1,
        int pageSize = 20
    );
}
