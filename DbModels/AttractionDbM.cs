using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models;
using Models.Dto;
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

    public AttractionDbM(AttractionDbM org)
        : base(org) { }

    public AttractionDbM(AttractionCuDto org, Guid addressId)
    {
        AttractionId = Guid.NewGuid();
        AddressId = addressId;
        CategoryId = org.CategoryId;
        Title = org.Title;
        Description = org.Description;
    }
    #endregion

    #region implementing IEquatable
    public bool Equals(AttractionDbM other) =>
        (other != null)
        && (Title?.Trim().ToLower() == other.Title?.Trim().ToLower())
        && (AddressId == other.AddressId);

    public override bool Equals(object obj) => Equals(obj as AttractionDbM);

    public override int GetHashCode() => (Title?.Trim().ToLower(), AddressId).GetHashCode();
    #endregion

    #region randomly seed this instance
    public new AttractionDbM Seed(SeedGenerator seedGenerator, Guid categoryId, Guid addressId)
    {
        base.Seed(seedGenerator, categoryId, addressId);
        return this;
    }
    #endregion

    #region Update from DTO
    // Only Category/Title/Description live here - City/Country/Street/ZipCode belong to the
    // linked Address and are updated separately by the repo (see AttractionDbRepos.UpdateAsync).
    public AttractionDbM UpdateFromDTO(AttractionCuDto org)
    {
        if (org == null)
            return null;

        CategoryId = org.CategoryId;
        Title = org.Title;
        Description = org.Description;

        return this;
    }
    #endregion
}
