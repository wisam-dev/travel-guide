namespace Models.Dto;

public class AttractionListItemDto
{
    public Guid AttractionId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public int CommentCount { get; set; }

    // Only populated when the caller asks for it (includeComments=true) - keeps list responses lean by default.
    public List<CommentDto> Comments { get; set; }
}

public class CommentDto
{
    public Guid CommentId { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; }
}

public class AttractionDetailDto
{
    public Guid AttractionId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Address { get; set; }
    public string Category { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public PagedResult<CommentDto> Comments { get; set; }
}
