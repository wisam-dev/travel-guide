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
    public virtual bool Seeded { get; set; }

    #region constructors
    public Attraction() { }

    // categoryId and addressId must reference rows already saved in the database.
    // Street/city/country/zip now live on the Address entity - this constructor only generates
    // the attraction's own Title/Description.
    public Attraction(SeedGenerator seeder, Guid categoryId, Guid addressId)
    {
        CategoryId = categoryId;
        AddressId = addressId;

        // Use a short Latin sentence as a stand-in "title" and a full paragraph as the description
        Title = seeder.LatinSentence.Split('.')[0].Trim();
        Description = seeder.LatinParagraph;
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
}
