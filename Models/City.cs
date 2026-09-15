namespace Models;

public interface ICity
{
    public Guid CityId { get; set; }
    public string Name { get; set; }
    public bool Seeded { get; set; }
    public Guid CountryId { get; set; }
}

public class City : ICity, IEquatable<City>
{
    public virtual Guid CityId { get; set; }
    public virtual string Name { get; set; }
    public virtual bool Seeded { get; set; }
    public virtual Guid CountryId { get; set; }

    #region constructors
    public City() { }

    public City(string name, Guid countryId)
    {
        Name = name;
        CountryId = countryId;
    }
    #endregion

    #region implementing IEquatable
    // Two cities are only "the same" if same name AND same country
    public bool Equals(City other) =>
        (other != null)
        && (Name?.Trim().ToLower() == other.Name?.Trim().ToLower())
        && (CountryId == other.CountryId);

    public override bool Equals(object obj) => Equals(obj as City);

    public override int GetHashCode() => (Name?.Trim().ToLower(), CountryId).GetHashCode();
    #endregion
}
