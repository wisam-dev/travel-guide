using System.ComponentModel.DataAnnotations;
using Models;

namespace DbModels;

public sealed class CategoryDbM : Category, IEquatable<CategoryDbM>
{
    [Key]
    public override Guid CategoryId { get; set; }

    public List<AttractionDbM> Attractions { get; set; } = new();

    #region constructors
    public CategoryDbM()
        : base() { }

    public CategoryDbM(string name)
        : base(name) { }
    #endregion

    #region implementing IEquatable
    public bool Equals(CategoryDbM other) =>
        (other != null) && (Name?.Trim().ToLower() == other.Name?.Trim().ToLower());

    public override bool Equals(object obj) => Equals(obj as CategoryDbM);

    public override int GetHashCode() => Name?.Trim().ToLower().GetHashCode() ?? 0;
    #endregion
}
