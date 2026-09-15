using Microsoft.AspNetCore.Mvc;
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

        // GET: api/users/getall?includeComments=true&pageNumber=1&pageSize=20
        [HttpGet()]
        [ActionName("")]
        [ProducesResponseType(200, Type = typeof(PagedResult<UsersDto>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> GetAll(
            bool includeComments = true,
            int pageNumber = 1,
            int pageSize = 20
        )
        {
            try
            {
                _logger.LogInformation($"{nameof(GetAll)}");
                var result = await _service.ReadAllAsync(includeComments, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(GetAll)}: {ex.Message}");
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
