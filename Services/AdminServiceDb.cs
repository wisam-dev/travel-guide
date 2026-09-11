using DbRepos;
using Microsoft.Extensions.Logging;

namespace Services;

public class AdminServiceDb : IAdminService
{
    private readonly AdminDbRepos _repo = null;
    private readonly ILogger<AdminServiceDb> _logger = null;

    public Task SeedAsync(int nrUsers = 50, int nrCities = 100, int nrAttractions = 1000) =>
        _repo.SeedAsync(nrUsers, nrCities, nrAttractions);

    public Task ClearAllAsync() => _repo.ClearAllAsync();

    #region constructors
    public AdminServiceDb(AdminDbRepos repo)
    {
        _repo = repo;
    }

    public AdminServiceDb(AdminDbRepos repo, ILogger<AdminServiceDb> logger)
        : this(repo)
    {
        _logger = logger;
    }
    #endregion
}
