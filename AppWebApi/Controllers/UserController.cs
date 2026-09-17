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

        // GET: api/users/read?seeded=true&flat=false&filter=&pageNumber=0&pageSize=20
        // Task 6, bullet 4: all users and the comments each has posted (flat=false includes them).
        [HttpGet()]
        [ActionName("Read")]
        [ProducesResponseType(200, Type = typeof(ResponsePageDto<IUser>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> Read(
            bool seeded = true,
            bool flat = false,
            string filter = "",
            int pageNumber = 0,
            int pageSize = 5
        )
        {
            try
            {
                _logger.LogInformation($"{nameof(Read)}");
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
                _logger.LogError($"{nameof(Read)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        public UsersController(ILogger<UsersController> logger, IUserService service)
        {
            _logger = logger;
            _service = service;
        }
    }
}
