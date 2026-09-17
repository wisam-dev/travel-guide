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

    // cityId/countryId must reference rows already saved in the database.
    // countryName is required so the seeder picks a street from the RIGHT country's street list -
    // calling seeder.StreetAddress() with no argument picks a random country's streets instead,
    // which would mismatch the actual city/country this address belongs to.
    public Address(SeedGenerator seeder, Guid cityId, Guid countryId, string countryName)
    {
        CityId = cityId;
        CountryId = countryId;
        Street = seeder.StreetAddress(countryName);
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
