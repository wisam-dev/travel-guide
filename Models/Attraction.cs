using Seido.Utilities.SeedGenerator;

namespace Models;

public interface IAttraction
{
    public Guid AttractionId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid CategoryId { get; set; }
    public Guid AddressId { get; set; }
    public bool Seeded { get; set; }
}

public class Attraction : IAttraction, IEquatable<Attraction>
{
    public virtual Guid AttractionId { get; set; }
    public virtual string Title { get; set; }
    public virtual string Description { get; set; }
    public virtual Guid CategoryId { get; set; }
    public virtual Guid AddressId { get; set; }
    public virtual bool Seeded { get; set; } = false;

    #region constructors
    public Attraction() { }

    public Attraction(Attraction org)
    {
        Seeded = org.Seeded;
        AttractionId = org.AttractionId;
        Title = org.Title;
        Description = org.Description;
        CategoryId = org.CategoryId;
        AddressId = org.AddressId;
    }
    #endregion

    #region implementing IEquatable
    public bool Equals(Attraction other) =>
        (other != null)
        && (Title?.Trim().ToLower() == other.Title?.Trim().ToLower())
        && (AddressId == other.AddressId);

    public override bool Equals(object obj) => Equals(obj as Attraction);

    public override int GetHashCode() => (Title?.Trim().ToLower(), AddressId).GetHashCode();
    #endregion

    #region randomly seed this instance
    // Depends on an already-persisted Category/Address - not a literal ISeed<Attraction>.
    public virtual Attraction Seed(SeedGenerator seedGenerator, Guid categoryId, Guid addressId)
    {
        Seeded = true;
        AttractionId = Guid.NewGuid();
        CategoryId = categoryId;
        AddressId = addressId;

        Title = seedGenerator.LatinSentence.Split('.')[0].Trim();
        Description = seedGenerator.LatinParagraph;

        return this;
    }
    #endregion
}
