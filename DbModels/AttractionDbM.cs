using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models;
using Seido.Utilities.SeedGenerator;

namespace DbModels;

public sealed class AttractionDbM : Attraction, IEquatable<AttractionDbM>
{
    [Key]
    public override Guid AttractionId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public CategoryDbM Category { get; set; }

    [ForeignKey(nameof(CityId))]
    public CityDbM City { get; set; }

    public List<CommentDbM> Comments { get; set; } = new();

    #region constructors
    public AttractionDbM()
        : base() { }

    public AttractionDbM(
        SeedGenerator seeder,
        Guid categoryId,
        Guid cityId,
        string countryForAddress = null
    )
        : base(seeder, categoryId, cityId, countryForAddress) { }
    #endregion

    #region implementing IEquatable
    public bool Equals(AttractionDbM other) =>
        (other != null)
        && (Title?.Trim().ToLower() == other.Title?.Trim().ToLower())
        && (CityId == other.CityId);

    public override bool Equals(object obj) => Equals(obj as AttractionDbM);

    public override int GetHashCode() => (Title?.Trim().ToLower(), CityId).GetHashCode();
    #endregion
}
