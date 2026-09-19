using Models.Dto;

namespace Services;

public interface IAdminService
{
    public Task SeedAsync(int nrUsers = 50, int nrCities = 100, int nrAttractions = 1000);
    public Task<DataResultInfoDto> ClearDataAsync(bool onlySeeded = true);
    public Task<DbInfoDto> GetDbInfoAsync();
}
