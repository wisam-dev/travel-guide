using Seido.Utilities.SeedGenerator;

namespace Models;

public interface IComment
{
    public Guid CommentId { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid UserId { get; set; }
    public Guid AttractionId { get; set; }
    public bool Seeded { get; set; }
}

public class Comment : IComment, IEquatable<Comment>
{
    public virtual Guid CommentId { get; set; }
    public virtual string Text { get; set; }
    public virtual DateTime CreatedAt { get; set; }
    public virtual Guid UserId { get; set; }
    public virtual Guid AttractionId { get; set; }
    public virtual bool Seeded { get; set; } = false;

    #region constructors
    public Comment() { }

    public Comment(Comment org)
    {
        Seeded = org.Seeded;
        CommentId = org.CommentId;
        Text = org.Text;
        CreatedAt = org.CreatedAt;
        UserId = org.UserId;
        AttractionId = org.AttractionId;
    }
    #endregion

    #region implementing IEquatable
    public bool Equals(Comment other) =>
        (other != null)
        && (UserId == other.UserId)
        && (AttractionId == other.AttractionId)
        && (Text?.Trim().ToLower() == other.Text?.Trim().ToLower());

    public override bool Equals(object obj) => Equals(obj as Comment);

    public override int GetHashCode() =>
        (UserId, AttractionId, Text?.Trim().ToLower()).GetHashCode();
    #endregion

    #region randomly seed this instance
    // Depends on an already-persisted User/Attraction - not a literal ISeed<Comment>.
    public virtual Comment Seed(SeedGenerator seedGenerator, Guid userId, Guid attractionId)
    {
        Seeded = true;
        CommentId = Guid.NewGuid();
        UserId = userId;
        AttractionId = attractionId;
        Text = seedGenerator.LatinSentence;
        CreatedAt = seedGenerator.DateAndTime(2023, 2026);
        return this;
    }
    #endregion
}
