using Seido.Utilities.SeedGenerator;

namespace Models;

public interface ICity
{
    public Guid CityId { get; set; }
    public string Name { get; set; }
    public Guid CountryId { get; set; }
    public bool Seeded { get; set; }
}

public class City : ICity, IEquatable<City>
{
    public virtual Guid CityId { get; set; }
    public virtual string Name { get; set; }
    public virtual Guid CountryId { get; set; }
    public virtual bool Seeded { get; set; } = false;

    #region constructors
    public City() { }

    public City(City org)
    {
        Seeded = org.Seeded;
        CityId = org.CityId;
        Name = org.Name;
        CountryId = org.CountryId;
    }
    #endregion

    #region implementing IEquatable
    public bool Equals(City other) =>
        (other != null)
        && (Name?.Trim().ToLower() == other.Name?.Trim().ToLower())
        && (CountryId == other.CountryId);

    public override bool Equals(object obj) => Equals(obj as City);

    public override int GetHashCode() => (Name?.Trim().ToLower(), CountryId).GetHashCode();
    #endregion

    #region randomly seed this instance
    public virtual City Seed(SeedGenerator seedGenerator, Guid countryId, string countryName)
    {
        Seeded = true;
        CityId = Guid.NewGuid();
        CountryId = countryId;
        Name = seedGenerator.City(countryName);
        return this;
    }
    #endregion
}
