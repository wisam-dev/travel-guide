using Seido.Utilities.SeedGenerator;

namespace Models;

public interface IUser
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public bool Seeded { get; set; }
}

public class User : IUser, ISeed<User>, IEquatable<User>
{
    public virtual Guid UserId { get; set; }
    public virtual string Name { get; set; }
    public virtual string Email { get; set; }
    public virtual bool Seeded { get; set; } = false;

    #region constructors
    public User() { }

    public User(User org)
    {
        Seeded = org.Seeded;
        UserId = org.UserId;
        Name = org.Name;
        Email = org.Email;
    }
    #endregion

    #region implementing IEquatable
    // Email is the natural unique key - this is what UniqueItemsToList<User> dedupes on.
    public bool Equals(User other) =>
        (other != null) && (Email?.Trim().ToLower() == other.Email?.Trim().ToLower());

    public override bool Equals(object obj) => Equals(obj as User);

    public override int GetHashCode() => Email?.Trim().ToLower().GetHashCode() ?? 0;
    #endregion

    #region randomly seed this instance
    public virtual User Seed(SeedGenerator seedGenerator)
    {
        Seeded = true;
        UserId = Guid.NewGuid();

        var first = seedGenerator.FirstName;
        var last = seedGenerator.LastName;
        Name = $"{first} {last}";
        Email = seedGenerator.Email(first, last);

        return this;
    }
    #endregion
}
