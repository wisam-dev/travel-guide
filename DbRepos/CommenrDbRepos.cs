using DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.Dto;

namespace DbRepos;

public class CommentDbRepos
{
    private readonly ILogger<CommentDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public async Task<ResponseItemDto<IComment>> CreateCommentAsync(CommentCuDto itemDto)
    {
        if (itemDto.CommentId != null)
            throw new ArgumentException(
                $"{nameof(itemDto.CommentId)} must be null when creating a new object"
            );

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == itemDto.UserId);
        if (user == null)
            throw new ArgumentException($"User {itemDto.UserId} does not exist");

        var attraction = await _dbContext.Attractions.FirstOrDefaultAsync(a =>
            a.AttractionId == itemDto.AttractionId
        );
        if (attraction == null)
            throw new ArgumentException($"Attraction {itemDto.AttractionId} does not exist");

        var item = new DbModels.CommentDbM(itemDto);

        _dbContext.Comments.Add(item);
        await _dbContext.SaveChangesAsync();

        return new ResponseItemDto<IComment>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            Item = item,
        };
    }

    public async Task<ResponseItemDto<IComment>> DeleteCommentAsync(Guid id)
    {
        var item = await _dbContext.Comments.FirstOrDefaultAsync(c => c.CommentId == id);
        if (item == null)
            throw new ArgumentException($"Comment {id} does not exist");

        _dbContext.Comments.Remove(item);
        await _dbContext.SaveChangesAsync();

        return new ResponseItemDto<IComment>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            Item = item,
        };
    }

    public CommentDbRepos(ILogger<CommentDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }
}
