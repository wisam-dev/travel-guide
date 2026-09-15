using Seido.Utilities.SeedGenerator;

namespace Models;

public interface IAttraction
{
    public Guid AttractionId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Address { get; set; }
    public bool Seeded { get; set; }
    public Guid CategoryId { get; set; }
    public Guid CityId { get; set; }
}

public class Attraction : IAttraction, IEquatable<Attraction>
{
    public virtual Guid AttractionId { get; set; }
    public virtual string Title { get; set; }
    public virtual string Description { get; set; }
    public virtual string Address { get; set; }
    public virtual bool Seeded { get; set; }
    public virtual Guid CategoryId { get; set; }
    public virtual Guid CityId { get; set; }

    #region constructors
    public Attraction() { }

    // seeder is used to generate a believable title/description/address for test data;
    // categoryId and cityId must reference rows already saved in the database
    public Attraction(
        SeedGenerator seeder,
        Guid categoryId,
        Guid cityId,
        string countryForAddress = null
    )
    {
        CategoryId = categoryId;
        CityId = cityId;

        // Use a short Latin sentence as a stand-in "title" and a full paragraph as the description
        Title = seeder.LatinSentence.Split('.')[0].Trim();
        Description = seeder.LatinParagraph;
        Address = seeder.StreetAddress(countryForAddress);
    }
    #endregion

    #region implementing IEquatable
    public bool Equals(Attraction other) =>
        (other != null)
        && (Title?.Trim().ToLower() == other.Title?.Trim().ToLower())
        && (CityId == other.CityId);

    public override bool Equals(object obj) => Equals(obj as Attraction);

    public override int GetHashCode() => (Title?.Trim().ToLower(), CityId).GetHashCode();
    #endregion
}
