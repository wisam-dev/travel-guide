using System.ComponentModel.DataAnnotations;
using Models;
using Models.Dto;
using Seido.Utilities.SeedGenerator;

namespace DbModels;

public sealed class UserDbM : User, ISeed<UserDbM>, IEquatable<UserDbM>
{
    [Key]
    public override Guid UserId { get; set; }

    public List<CommentDbM> Comments { get; set; } = new();

    #region constructors
    public UserDbM()
        : base() { }

    public UserDbM(UserDbM org)
        : base(org) { }

    public UserDbM(UserCuDto org)
    {
        UserId = Guid.NewGuid();
        UpdateFromDTO(org);
    }
    #endregion

    #region implementing IEquatable
    public bool Equals(UserDbM other) =>
        (other != null) && (Email?.Trim().ToLower() == other.Email?.Trim().ToLower());

    public override bool Equals(object obj) => Equals(obj as UserDbM);

    public override int GetHashCode() => Email?.Trim().ToLower().GetHashCode() ?? 0;
    #endregion

    #region randomly seed this instance
    public new UserDbM Seed(SeedGenerator seedGenerator)
    {
        base.Seed(seedGenerator);
        return this;
    }
    #endregion

    #region Update from DTO
    public UserDbM UpdateFromDTO(UserCuDto org)
    {
        if (org == null)
            return null;

        Name = org.Name;
        Email = org.Email;

        return this;
    }
    #endregion
}
