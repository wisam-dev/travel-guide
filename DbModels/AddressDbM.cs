using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models;
using Models.Dto;
using Seido.Utilities.SeedGenerator;

namespace DbModels;

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

    public AddressDbM(AddressDbM org)
        : base(org) { }

    public AddressDbM(AddressCuDto org)
    {
        AddressId = Guid.NewGuid();
        UpdateFromDTO(org);
    }
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
    #endregion

    #region randomly seed this instance
    public new AddressDbM Seed(
        SeedGenerator seedGenerator,
        Guid cityId,
        Guid countryId,
        string countryName
    )
    {
        base.Seed(seedGenerator, cityId, countryId, countryName);
        return this;
    }
    #endregion

    #region Update from DTO
    public AddressDbM UpdateFromDTO(AddressCuDto org)
    {
        if (org == null)
            return null;

        Street = org.Street;
        ZipCode = org.ZipCode;
        CityId = org.CityId;

        return this;
    }
    #endregion
}
