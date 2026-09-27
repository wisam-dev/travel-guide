using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models;
using Seido.Utilities.SeedGenerator;

namespace DbModels;

public sealed class CityDbM : City, IEquatable<CityDbM>
{
    [Key]
    public override Guid CityId { get; set; }

    [ForeignKey(nameof(CountryId))]
    public CountryDbM Country { get; set; }

    #region constructors
    public CityDbM()
        : base() { }

    public CityDbM(CityDbM org)
        : base(org) { }
    #endregion

    #region implementing IEquatable
    public bool Equals(CityDbM other) =>
        (other != null)
        && (Name?.Trim().ToLower() == other.Name?.Trim().ToLower())
        && (CountryId == other.CountryId);

    public override bool Equals(object obj) => Equals(obj as CityDbM);

    public override int GetHashCode() => (Name?.Trim().ToLower(), CountryId).GetHashCode();
    #endregion

    #region randomly seed this instance
    public new CityDbM Seed(SeedGenerator seedGenerator, Guid countryId, string countryName)
    {
        base.Seed(seedGenerator, countryId, countryName);
        return this;
    }
    #endregion
}
