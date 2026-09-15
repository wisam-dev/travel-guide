using Seido.Utilities.SeedGenerator;

namespace Models;

public interface IUser
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public bool Seeded { get; set; }
}

public class User : IUser, IEquatable<User>
{
    public virtual Guid UserId { get; set; }
    public virtual string Name { get; set; }
    public virtual string Email { get; set; }
    public virtual bool Seeded { get; set; }

    #region constructors
    public User() { }

    public User(SeedGenerator seeder)
    {
        var first = seeder.FirstName;
        var last = seeder.LastName;

        Name = $"{first} {last}";
        Email = seeder.Email(first, last);
    }
    #endregion

    #region implementing IEquatable
    // Email is the natural unique key for a user
    public bool Equals(User other) =>
        (other != null) && (Email?.Trim().ToLower() == other.Email?.Trim().ToLower());

    public override bool Equals(object obj) => Equals(obj as User);

    public override int GetHashCode() => Email?.Trim().ToLower().GetHashCode() ?? 0;
    #endregion
}
