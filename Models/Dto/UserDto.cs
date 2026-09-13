namespace Models.Dto;

public class UserCommentDto
{
    public Guid CommentId { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid AttractionId { get; set; }
    public string AttractionTitle { get; set; }
}

public class UsersDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int CommentCount { get; set; }
    public List<UserCommentDto> Comments { get; set; } = new();
}
