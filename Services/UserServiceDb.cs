using DbRepos;
using Microsoft.Extensions.Logging;
using Models.Dto;

namespace Services;

public class UserServiceDb : IUserService
{
    private readonly UserDbRepos _repo;
    private readonly ILogger<UserServiceDb> _logger;

    public Task<PagedResult<UsersDto>> GetAllUsersAsync(
        bool includeComments = true,
        int pageNumber = 1,
        int pageSize = 20
    ) => _repo.GetAllUsersAsync(includeComments, pageNumber, pageSize);

    #region constructors
    public UserServiceDb(UserDbRepos repo)
    {
        _repo = repo;
    }

    public UserServiceDb(UserDbRepos repo, ILogger<UserServiceDb> logger)
        : this(repo)
    {
        _logger = logger;
    }
    #endregion
}
