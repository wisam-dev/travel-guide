using System.Data; // CommandType, ConnectionState, ParameterDirection, SqlDbType
using System.Data.Common; // DbParameter
using Configuration;
using DbContext;
using DbModels;
using Microsoft.Data.SqlClient; // SqlParameter
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Dto;
using Seido.Utilities.SeedGenerator;

namespace DbRepos;

public class AdminDbRepos
{
    private const string _seedSource = "./app-seeds.json";

    private static readonly string[] _countryNames = { "Sweden", "Norway", "Denmark", "Finland" };

    private static readonly string[] _categoryNames =
    {
        "Restaurant",
        "Cafe",
        "Architecture",
        "Museum",
        "Park",
        "Shopping",
        "Nightlife",
        "Historical Site",
    };

    private readonly ILogger<AdminDbRepos> _logger;
    private Encryptions _encryptions;
    private readonly MainDbContext _dbContext;

    public async Task SeedAsync(int nrUsers = 50, int nrCities = 100, int nrAttractions = 1000)
    {
        var fn = Path.GetFullPath(_seedSource);
        var seeder = new SeedGenerator(fn);

        _logger.LogInformation($"{nameof(SeedAsync)}: clearing existing seeded data");
        await ClearDataAsync(true);

        // --- Countries -------------------------------------------------
        var countries = _countryNames
            .Select(name => new CountryDbM(name) { Seeded = true })
            .ToList();
        _dbContext.Countries.AddRange(countries);
        await _dbContext.SaveChangesAsync();

        var countryNameById = countries.ToDictionary(c => c.CountryId, c => c.Name);

        // --- Categories --------------------------------------------------
        var categories = _categoryNames
            .Select(name => new CategoryDbM(name) { Seeded = true })
            .ToList();
        _dbContext.Categories.AddRange(categories);
        await _dbContext.SaveChangesAsync();

        // --- Cities: spread evenly across all 4 countries -----------------
        var cities = new List<CityDbM>();
        for (int i = 0; i < nrCities; i++)
        {
            var country = countries[i % countries.Count];
            var cityName = seeder.City(country.Name);
            cities.Add(new CityDbM(cityName, country.CountryId) { Seeded = true });
        }
        _dbContext.Cities.AddRange(cities);
        await _dbContext.SaveChangesAsync();

        // --- Addresses: one per attraction, each tied to a random city (and that city's country) ---
        var addresses = new List<AddressDbM>();
        for (int i = 0; i < nrAttractions; i++)
        {
            var city = cities[seeder.Next(0, cities.Count)];
            var countryName = countryNameById[city.CountryId];
            addresses.Add(
                new AddressDbM(seeder, city.CityId, city.CountryId, countryName) { Seeded = true }
            );
        }
        _dbContext.Addresses.AddRange(addresses);
        await _dbContext.SaveChangesAsync();

        // --- Users: unique by email --------------------------------------
        var users = new List<UserDbM>();
        var seenEmails = new HashSet<string>();
        while (users.Count < nrUsers)
        {
            var candidate = new UserDbM(seeder) { Seeded = true };
            if (seenEmails.Add(candidate.Email.Trim().ToLower()))
                users.Add(candidate);
        }
        _dbContext.Users.AddRange(users);
        await _dbContext.SaveChangesAsync();

        // --- Attractions: one per address, random category ---------------
        var attractions = new List<AttractionDbM>();
        for (int i = 0; i < addresses.Count; i++)
        {
            var category = categories[seeder.Next(0, categories.Count)];
            attractions.Add(
                new AttractionDbM(seeder, category.CategoryId, addresses[i].AddressId)
                {
                    Seeded = true,
                }
            );
        }
        _dbContext.Attractions.AddRange(attractions);
        await _dbContext.SaveChangesAsync();

        // --- Comments: 0-20 random comments per attraction -----------------
        var comments = new List<CommentDbM>();
        foreach (var attraction in attractions)
        {
            var nrComments = seeder.Next(0, 21); // 0..20 inclusive
            for (int i = 0; i < nrComments; i++)
            {
                var user = users[seeder.Next(0, users.Count)];
                comments.Add(
                    new CommentDbM(seeder, user.UserId, attraction.AttractionId) { Seeded = true }
                );
            }
        }
        _dbContext.Comments.AddRange(comments);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            $"{nameof(SeedAsync)}: seeded {countries.Count} countries, {categories.Count} categories, "
                + $"{cities.Count} cities, {addresses.Count} addresses, {users.Count} users, {attractions.Count} attractions, {comments.Count} comments"
        );
    }

    // Deletes existing rows in FK-safe order (children before parents).
    public async Task<DataResultInfoDto> ClearDataAsync(bool onlySeeded = true)
    {
        _logger.LogInformation($"{nameof(ClearDataAsync)}: onlySeeded={onlySeeded}");

        var connection = _dbContext.Database.GetDbConnection();
        using var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = "clear_db_data";

        var onlySeededParam = new SqlParameter("@OnlySeeded", onlySeeded);
        var nrCommentsParam = new SqlParameter("@NrCommentsAffected", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output,
        };
        var nrAttractionsParam = new SqlParameter("@NrAttractionsAffected", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output,
        };
        var nrAddressesParam = new SqlParameter("@NrAddressesAffected", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output,
        };
        var nrCitiesParam = new SqlParameter("@NrCitiesAffected", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output,
        };
        var nrCategoriesParam = new SqlParameter("@NrCategoriesAffected", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output,
        };
        var nrUsersParam = new SqlParameter("@NrUsersAffected", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output,
        };
        var nrCountriesParam = new SqlParameter("@NrCountriesAffected", SqlDbType.Int)
        {
            Direction = ParameterDirection.Output,
        };

        command.Parameters.AddRange(
            new DbParameter[]
            {
                onlySeededParam,
                nrCommentsParam,
                nrAttractionsParam,
                nrAddressesParam,
                nrCitiesParam,
                nrCategoriesParam,
                nrUsersParam,
                nrCountriesParam,
            }
        );

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        await command.ExecuteNonQueryAsync();

        return new DataResultInfoDto
        {
            NrCommentsAffected = (int)nrCommentsParam.Value,
            NrAttractionsAffected = (int)nrAttractionsParam.Value,
            NrAddressesAffected = (int)nrAddressesParam.Value,
            NrCitiesAffected = (int)nrCitiesParam.Value,
            NrCategoriesAffected = (int)nrCategoriesParam.Value,
            NrUsersAffected = (int)nrUsersParam.Value,
            NrCountriesAffected = (int)nrCountriesParam.Value,
        };
    }

    public Task<DbInfoDto> GetDbInfoAsync() =>
        _dbContext.DbInfo.AsNoTracking().FirstOrDefaultAsync();

    public AdminDbRepos(
        ILogger<AdminDbRepos> logger,
        Encryptions encryptions,
        MainDbContext context
    )
    {
        _logger = logger;
        _encryptions = encryptions;
        _dbContext = context;
    }
}
