using DbRepos;
using Microsoft.Extensions.Logging;
using Models;
using Models.Dto;

namespace Services;

public class CommentServiceDb : ICommentService
{
    private readonly CommentDbRepos _repo;
    private readonly ILogger<CommentServiceDb> _logger;

    public Task<ResponseItemDto<IComment>> CreateCommentAsync(CommentCuDto itemDto) =>
        _repo.CreateCommentAsync(itemDto);

    public Task<ResponseItemDto<IComment>> DeleteCommentAsync(Guid id) =>
        _repo.DeleteCommentAsync(id);

    #region constructors
    public CommentServiceDb(CommentDbRepos repo)
    {
        _repo = repo;
    }

    public CommentServiceDb(CommentDbRepos repo, ILogger<CommentServiceDb> logger)
        : this(repo)
    {
        _logger = logger;
    }
    #endregion
}
