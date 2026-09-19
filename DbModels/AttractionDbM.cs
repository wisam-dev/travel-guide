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

    [ForeignKey(nameof(AddressId))]
    public AddressDbM Address { get; set; }

    public List<CommentDbM> Comments { get; set; } = new();

    #region constructors
    public AttractionDbM()
        : base() { }

    public AttractionDbM(SeedGenerator seeder, Guid categoryId, Guid addressId)
        : base(seeder, categoryId, addressId) { }
    #endregion

    #region implementing IEquatable
    public bool Equals(AttractionDbM other) =>
        (other != null)
        && (Title?.Trim().ToLower() == other.Title?.Trim().ToLower())
        && (AddressId == other.AddressId);

    public override bool Equals(object obj) => Equals(obj as AttractionDbM);

    public override int GetHashCode() => (Title?.Trim().ToLower(), AddressId).GetHashCode();
    #endregion
}
