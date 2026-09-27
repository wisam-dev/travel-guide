using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Dto;
using Services;

namespace AppWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CommentsController : Controller
    {
        readonly ILogger<CommentsController> _logger;
        readonly ICommentService _service;

        // POST: api/comments/createitem
        [HttpPost()]
        [ActionName("CreateItem")]
        [ProducesResponseType(200, Type = typeof(ResponseItemDto<IComment>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> CreateItem([FromBody] CommentCuDto item)
        {
            try
            {
                item.EnsureValidity();
                _logger.LogInformation($"{nameof(CreateItem)}");

                var model = await _service.CreateCommentAsync(item);
                _logger.LogInformation($"Comment {model.Item.CommentId} created");

                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(CreateItem)}: {ex.Message}");
                return BadRequest($"Could not create. Error: {ex.Message}");
            }
        }

        // DELETE: api/comments/deleteitem/{commentId}
        [HttpDelete("{commentId}")]
        [ActionName("DeleteItem")]
        [ProducesResponseType(200, Type = typeof(ResponseItemDto<IComment>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> DeleteItem(Guid commentId)
        {
            try
            {
                _logger.LogInformation($"{nameof(DeleteItem)}: {commentId}");
                var model = await _service.DeleteCommentAsync(commentId);
                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(DeleteItem)}: {ex.Message}");
                return BadRequest($"Could not delete. Error: {ex.Message}");
            }
        }

        public CommentsController(ILogger<CommentsController> logger, ICommentService service)
        {
            _logger = logger;
            _service = service;
        }
    }
}
