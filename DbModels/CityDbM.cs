using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models;

namespace DbModels;

public sealed class CityDbM : City, IEquatable<CityDbM>
{
    [Key]
    public override Guid CityId { get; set; }

    [ForeignKey(nameof(CountryId))]
    public CountryDbM Country { get; set; }

    public List<AttractionDbM> Attractions { get; set; } = new();

    #region constructors
    public CityDbM()
        : base() { }

    public CityDbM(string name, Guid countryId)
        : base(name, countryId) { }
    #endregion

    #region implementing IEquatable
    public bool Equals(CityDbM other) =>
        (other != null)
        && (Name?.Trim().ToLower() == other.Name?.Trim().ToLower())
        && (CountryId == other.CountryId);

    public override bool Equals(object obj) => Equals(obj as CityDbM);

    public override int GetHashCode() => (Name?.Trim().ToLower(), CountryId).GetHashCode();
    #endregion
}
