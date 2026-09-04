using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models;
using Seido.Utilities.SeedGenerator;

namespace DbModels;

public sealed class CommentDbM : Comment, IEquatable<CommentDbM>
{
    [Key]
    public override Guid CommentId { get; set; }

    [ForeignKey(nameof(UserId))]
    public UserDbM User { get; set; }

    [ForeignKey(nameof(AttractionId))]
    public AttractionDbM Attraction { get; set; }

    #region constructors
    public CommentDbM()
        : base() { }

    public CommentDbM(SeedGenerator seeder, Guid userId, Guid attractionId)
        : base(seeder, userId, attractionId) { }
    #endregion

    #region implementing IEquatable
    public bool Equals(CommentDbM other) =>
        (other != null)
        && (UserId == other.UserId)
        && (AttractionId == other.AttractionId)
        && (Text?.Trim().ToLower() == other.Text?.Trim().ToLower());

    public override bool Equals(object obj) => Equals(obj as CommentDbM);

    public override int GetHashCode() =>
        (UserId, AttractionId, Text?.Trim().ToLower()).GetHashCode();
    #endregion
}
