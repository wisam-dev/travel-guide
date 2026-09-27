using Seido.Utilities.SeedGenerator;

namespace Models;

public interface IAddress
{
    public Guid AddressId { get; set; }
    public string Street { get; set; }
    public int ZipCode { get; set; }
    public Guid CityId { get; set; }
    public Guid CountryId { get; set; }
    public bool Seeded { get; set; }
}

public class Address : IAddress, IEquatable<Address>
{
    public virtual Guid AddressId { get; set; }
    public virtual string Street { get; set; }
    public virtual int ZipCode { get; set; }
    public virtual Guid CityId { get; set; }
    public virtual Guid CountryId { get; set; }
    public virtual bool Seeded { get; set; } = false;

    #region constructors
    public Address() { }

    public Address(Address org)
    {
        Seeded = org.Seeded;
        AddressId = org.AddressId;
        Street = org.Street;
        ZipCode = org.ZipCode;
        CityId = org.CityId;
        CountryId = org.CountryId;
    }
    #endregion

    #region implementing IEquatable
    public bool Equals(Address other) =>
        (other != null)
        && (Street?.Trim().ToLower() == other.Street?.Trim().ToLower())
        && (ZipCode == other.ZipCode)
        && (CityId == other.CityId)
        && (CountryId == other.CountryId);

    public override bool Equals(object obj) => Equals(obj as Address);

    public override int GetHashCode() =>
        (Street?.Trim().ToLower(), ZipCode, CityId, CountryId).GetHashCode();
    #endregion

    #region randomly seed this instance
    // Depends on an already-persisted City/Country - not a literal ISeed<Address>.
    public virtual Address Seed(
        SeedGenerator seedGenerator,
        Guid cityId,
        Guid countryId,
        string countryName
    )
    {
        Seeded = true;
        AddressId = Guid.NewGuid();
        CityId = cityId;
        CountryId = countryId;
        Street = seedGenerator.StreetAddress(countryName);
        ZipCode = seedGenerator.ZipCode;
        return this;
    }
    #endregion
}
