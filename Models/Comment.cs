using Seido.Utilities.SeedGenerator;

namespace Models;

public interface IComment
{
    public Guid CommentId { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid UserId { get; set; }
    public Guid AttractionId { get; set; }
}

public class Comment : IComment, IEquatable<Comment>
{
    public virtual Guid CommentId { get; set; }
    public virtual string Text { get; set; }
    public virtual DateTime CreatedAt { get; set; }
    public virtual Guid UserId { get; set; }
    public virtual Guid AttractionId { get; set; }

    #region constructors
    public Comment() { }

    // userId and attractionId must reference rows already saved in the database
    public Comment(SeedGenerator seeder, Guid userId, Guid attractionId)
    {
        UserId = userId;
        AttractionId = attractionId;
        Text = seeder.LatinSentence;
        CreatedAt = seeder.DateAndTime(2023, 2026);
    }
    #endregion

    #region implementing IEquatable
    // Two rows are only accidental duplicates if same user+attraction+text
    public bool Equals(Comment other) =>
        (other != null)
        && (UserId == other.UserId)
        && (AttractionId == other.AttractionId)
        && (Text?.Trim().ToLower() == other.Text?.Trim().ToLower());

    public override bool Equals(object obj) => Equals(obj as Comment);

    public override int GetHashCode() =>
        (UserId, AttractionId, Text?.Trim().ToLower()).GetHashCode();
    #endregion
}
