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

        // GET: api/users/withcomments?pageNumber=1&pageSize=20
        [HttpGet()]
        [ActionName("WithComments")]
        [ProducesResponseType(200, Type = typeof(PagedResult<UserWithCommentsDto>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> WithComments(int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                _logger.LogInformation($"{nameof(WithComments)}");
                var result = await _service.GetAllWithCommentsAsync(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(WithComments)}: {ex.Message}");
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
