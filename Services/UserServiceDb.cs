using DbRepos;
using Microsoft.Extensions.Logging;
using Models;
using Models.Dto;

namespace Services;

public class UserServiceDb : IUserService
{
    private readonly UserDbRepos _repo;
    private readonly ILogger<UserServiceDb> _logger;

    public Task<ResponsePageDto<IUser>> ReadUsersAsync(
        bool seeded,
        bool flat,
        string filter,
        int pageNumber,
        int pageSize
    ) => _repo.ReadUsersAsync(seeded, flat, filter, pageNumber, pageSize);

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
