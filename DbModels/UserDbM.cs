using System.ComponentModel.DataAnnotations;
using Models;
using Seido.Utilities.SeedGenerator;

namespace DbModels;

public sealed class UserDbM : User, IEquatable<UserDbM>
{
    [Key]
    public override Guid UserId { get; set; }

    public List<CommentDbM> Comments { get; set; } = new();

    #region constructors
    public UserDbM()
        : base() { }

    public UserDbM(SeedGenerator seeder)
        : base(seeder) { }
    #endregion

    #region implementing IEquatable
    public bool Equals(UserDbM other) =>
        (other != null) && (Email?.Trim().ToLower() == other.Email?.Trim().ToLower());

    public override bool Equals(object obj) => Equals(obj as UserDbM);

    public override int GetHashCode() => Email?.Trim().ToLower().GetHashCode() ?? 0;
    #endregion
}
