using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Dto;
using Services;

namespace AppWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UsersController : Controller
    {
        readonly ILogger<UsersController> _logger;
        readonly IUserService _service;

        // GET: api/users/readpage?seeded=true&flat=false&filter=&pageNumber=0&pageSize=20
        [HttpGet()]
        [ActionName("ReadPage")]
        [ProducesResponseType(200, Type = typeof(ResponsePageDto<IUser>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> ReadPage(
            bool seeded = true,
            bool flat = false,
            string filter = "",
            int pageNumber = 0,
            int pageSize = 20
        )
        {
            try
            {
                _logger.LogInformation($"{nameof(ReadPage)}");
                var result = await _service.ReadUsersAsync(
                    seeded,
                    flat,
                    filter,
                    pageNumber,
                    pageSize
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(ReadPage)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // POST: api/users/createitem
        [HttpPost()]
        [ActionName("CreateItem")]
        [ProducesResponseType(200, Type = typeof(ResponseItemDto<IUser>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> CreateItem([FromBody] UserCuDto item)
        {
            try
            {
                item.EnsureValidity();
                _logger.LogInformation($"{nameof(CreateItem)}");

                var model = await _service.CreateUserAsync(item);
                _logger.LogInformation($"User {model.Item.UserId} created");

                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(CreateItem)}: {ex.Message}");
                return BadRequest($"Could not create. Error: {ex.Message}");
            }
        }

        // PUT: api/users/updateitem/{userId}
        [HttpPut("{userId}")]
        [ActionName("UpdateItem")]
        [ProducesResponseType(200, Type = typeof(ResponseItemDto<IUser>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> UpdateItem(Guid userId, [FromBody] UserCuDto item)
        {
            try
            {
                item.EnsureValidity();
                if (item.UserId != userId)
                    throw new ArgumentException("Id mismatch between route and body");

                _logger.LogInformation($"{nameof(UpdateItem)}: {userId}");

                var model = await _service.UpdateUserAsync(item);
                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(UpdateItem)}: {ex.Message}");
                return BadRequest($"Could not update. Error: {ex.Message}");
            }
        }

        // DELETE: api/users/deleteitem/{userId}
        // deletes the user and (via DB cascade) all their comments.
        [HttpDelete("{userId}")]
        [ActionName("DeleteItem")]
        [ProducesResponseType(200, Type = typeof(ResponseItemDto<IUser>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> DeleteItem(Guid userId)
        {
            try
            {
                _logger.LogInformation($"{nameof(DeleteItem)}: {userId}");
                var model = await _service.DeleteUserAsync(userId);
                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(DeleteItem)}: {ex.Message}");
                return BadRequest($"Could not delete. Error: {ex.Message}");
            }
        }

        public UsersController(ILogger<UsersController> logger, IUserService service)
        {
            _logger = logger;
            _service = service;
        }
    }
}
