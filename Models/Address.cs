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
    public virtual bool Seeded { get; set; }

    #region constructors
    public Address() { }

    // cityId and countryId must reference rows already saved in the database
    public Address(SeedGenerator seeder, Guid cityId, Guid countryId)
    {
        CityId = cityId;
        CountryId = countryId;
        Street = seeder.StreetAddress();
        ZipCode = seeder.ZipCode;
    }
    #endregion

    #region implementing IEquatable
    // Two rows are only accidental duplicates if same street+zip+city+country
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
}
