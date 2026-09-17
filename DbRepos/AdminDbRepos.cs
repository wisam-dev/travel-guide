using Configuration;
using DbContext;
using DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

        _logger.LogInformation($"{nameof(SeedAsync)}: clearing existing data");
        await ClearAllAsync();

        // --- Countries -------------------------------------------------
        var countries = _countryNames.Select(name => new CountryDbM(name)).ToList();
        _dbContext.Countries.AddRange(countries);
        await _dbContext.SaveChangesAsync();

        // --- Categories --------------------------------------------------
        var categories = _categoryNames.Select(name => new CategoryDbM(name)).ToList();
        _dbContext.Categories.AddRange(categories);
        await _dbContext.SaveChangesAsync();

        // --- Cities: spread evenly across all 4 countries -----------------
        var cities = new List<CityDbM>();
        for (int i = 0; i < nrCities; i++)
        {
            var country = countries[i % countries.Count];
            var cityName = seeder.City(country.Name);
            cities.Add(new CityDbM(cityName, country.CountryId));
        }
        _dbContext.Cities.AddRange(cities);
        await _dbContext.SaveChangesAsync();

        var countryNameById = countries.ToDictionary(c => c.CountryId, c => c.Name);
        var addresses = new HashSet<AddressDbM>();
        foreach (var city in cities)
        {
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
            var candidate = new UserDbM(seeder);
            if (seenEmails.Add(candidate.Email.Trim().ToLower()))
                users.Add(candidate);
        }
        _dbContext.Users.AddRange(users);
        await _dbContext.SaveChangesAsync();

        // --- Attractions: random category + city (and matching country for a believable address) ---
        var attractions = new List<AttractionDbM>();
        for (int i = 0; i < nrAttractions; i++)
        {
            var city = cities[seeder.Next(0, cities.Count)];
            var category = categories[seeder.Next(0, categories.Count)];
            var countryName = countryNameById[city.CountryId];

            attractions.Add(
                new AttractionDbM(seeder, category.CategoryId, city.CityId, countryName)
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
                comments.Add(new CommentDbM(seeder, user.UserId, attraction.AttractionId));
            }
        }
        _dbContext.Comments.AddRange(comments);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            $"{nameof(SeedAsync)}: seeded {countries.Count} countries, {categories.Count} categories, "
                + $"{cities.Count} cities, {users.Count} users, {attractions.Count} attractions, {comments.Count} comments"
        );
    }

    // Deletes existing rows in FK-safe order (children before parents).
    public async Task ClearAllAsync()
    {
        _dbContext.Comments.RemoveRange(_dbContext.Comments);
        await _dbContext.SaveChangesAsync();

        _dbContext.Attractions.RemoveRange(_dbContext.Attractions);
        await _dbContext.SaveChangesAsync();

        _dbContext.Cities.RemoveRange(_dbContext.Cities);
        _dbContext.Categories.RemoveRange(_dbContext.Categories);
        _dbContext.Users.RemoveRange(_dbContext.Users);
        await _dbContext.SaveChangesAsync();

        _dbContext.Countries.RemoveRange(_dbContext.Countries);
        await _dbContext.SaveChangesAsync();
    }

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
