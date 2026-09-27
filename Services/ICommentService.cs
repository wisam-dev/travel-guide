using Models;
using Models.Dto;

namespace Services;

public interface ICommentService
{
    public Task<ResponseItemDto<IComment>> CreateCommentAsync(CommentCuDto itemDto);
    public Task<ResponseItemDto<IComment>> DeleteCommentAsync(Guid id);
}
