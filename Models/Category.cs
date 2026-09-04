namespace Models;

public interface ICategory
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; }
}

public class Category : ICategory, IEquatable<Category>
{
    public virtual Guid CategoryId { get; set; }
    public virtual string Name { get; set; }

    #region constructors
    public Category() { }

    public Category(string name)
    {
        Name = name;
    }
    #endregion

    #region implementing IEquatable
    public bool Equals(Category other) =>
        (other != null) && (Name?.Trim().ToLower() == other.Name?.Trim().ToLower());

    public override bool Equals(object obj) => Equals(obj as Category);

    public override int GetHashCode() => Name?.Trim().ToLower().GetHashCode() ?? 0;
    #endregion
}
