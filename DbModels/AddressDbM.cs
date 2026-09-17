using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models;
using Seido.Utilities.SeedGenerator;

namespace DbModels;

// [Table("Addresses", Schema = "supusr")]
// [Index(nameof(StreetAddress), nameof(ZipCode), nameof(City), nameof(Country), IsUnique = true)]
public sealed class AddressDbM : Address, IEquatable<AddressDbM>
{
    [Key]
    public override Guid AddressId { get; set; }

    [ForeignKey(nameof(CityId))]
    public CityDbM City { get; set; }

    [ForeignKey(nameof(CountryId))]
    public CountryDbM Country { get; set; }

    #region constructors
    public AddressDbM()
        : base() { }

    public AddressDbM(SeedGenerator seeder, Guid cityId, Guid countryId, string countryName)
        : base(seeder, cityId, countryId, countryName) { }
    #endregion

    #region implementing IEquatable
    public bool Equals(AddressDbM other) =>
        (other != null)
        && (Street?.Trim().ToLower() == other.Street?.Trim().ToLower())
        && (ZipCode == other.ZipCode)
        && (CityId == other.CityId)
        && (CountryId == other.CountryId);

    public override bool Equals(object obj) => Equals(obj as AddressDbM);

    public override int GetHashCode() =>
        (Street?.Trim().ToLower(), ZipCode, CityId, CountryId).GetHashCode();

    // public AddressDbM Seed(SeedGenerator seedGenerator)
    // {
    //     throw new NotImplementedException();
    // }

    // public override AddressDbM Seed(SeedGenerator seedGenerator)
    // {
    //     base.Seed(seedGenerator);
    //     return this;
    // }
    #endregion
}
