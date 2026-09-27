using Seido.Utilities.SeedGenerator;

namespace Models;

public interface ICategory
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; }
    public bool Seeded { get; set; }
}

public class Category : ICategory, ISeed<Category>, IEquatable<Category>
{
    // Fixed pool for this domain - SeedGenerator has no built-in notion of "category" the way
    // it does Country/City, so we keep our own small list here.
    private static readonly string[] _names =
    {
        "Restaurant", "Cafe", "Architecture", "Museum", "Park", "Shopping", "Nightlife", "Historical Site"
    };

    public virtual Guid CategoryId { get; set; }
    public virtual string Name { get; set; }
    public virtual bool Seeded { get; set; } = false;

    #region constructors
    public Category() { }
    public Category(Category org)
    {
        Seeded = org.Seeded;
        CategoryId = org.CategoryId;
        Name = org.Name;
    }
    #endregion

    #region implementing IEquatable
    public bool Equals(Category other) => (other != null) && (Name?.Trim().ToLower() == other.Name?.Trim().ToLower());
    public override bool Equals(object obj) => Equals(obj as Category);
    public override int GetHashCode() => Name?.Trim().ToLower().GetHashCode() ?? 0;
    #endregion

    #region randomly seed this instance
    public virtual Category Seed(SeedGenerator seedGenerator)
    {
        Seeded = true;
        CategoryId = Guid.NewGuid();
        Name = _names[seedGenerator.Next(0, _names.Length)];
        return this;
    }
    #endregion
}