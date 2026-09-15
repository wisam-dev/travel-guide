namespace Models;

public interface ICountry
{
    public Guid CountryId { get; set; }
    public string Name { get; set; }
    public bool Seeded { get; set; }
}

public class Country : ICountry, IEquatable<Country>
{
    public virtual Guid CountryId { get; set; }
    public virtual string Name { get; set; }
    public virtual bool Seeded { get; set; }

    #region constructors
    public Country() { }

    public Country(string name)
    {
        CountryId = Guid.NewGuid();
        Name = name;
    }
    #endregion

    #region implementing IEquatable
    public bool Equals(Country other) =>
        (other != null) && (Name?.Trim().ToLower() == other.Name?.Trim().ToLower());

    public override bool Equals(object obj) => Equals(obj as Country);

    public override int GetHashCode() => Name?.Trim().ToLower().GetHashCode() ?? 0;
    #endregion
}
