using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Dto;
using Services;

namespace AppWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AttractionsController : Controller
    {
        readonly ILogger<AttractionsController> _logger;
        readonly IAttractionService _service;

        // GET: api/attractions/read?seeded=true&flat=false&category=&title=&description=&country=&city=&onlyWithoutComments=false&pageNumber=0&pageSize=20
        // All filter parameters are optional (default "": matches everything), combined with AND.
        [HttpGet()]
        [ActionName("Read")]
        [ProducesResponseType(200, Type = typeof(ResponsePageDto<IAttraction>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> Read(
            bool seeded = true,
            bool flat = false,
            string category = "",
            string title = "",
            string description = "",
            string country = "",
            string city = "",
            bool onlyWithoutComments = false,
            int pageNumber = 0,
            int pageSize = 20
        )
        {
            try
            {
                _logger.LogInformation($"{nameof(Read)}");
                var result = await _service.ReadAttractionsAsync(
                    seeded,
                    flat,
                    category,
                    title,
                    description,
                    country,
                    city,
                    onlyWithoutComments,
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

        // GET: api/attractions/read/{attractionId}?flat=false
        [HttpGet("{attractionId}")]
        [ActionName("ReadItem")]
        [ProducesResponseType(200, Type = typeof(ResponseItemDto<IAttraction>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> ReadItem(Guid attractionId, bool flat = false)
        {
            try
            {
                _logger.LogInformation($"{nameof(ReadItem)}: {attractionId}");
                var result = await _service.ReadAttractionAsync(attractionId, flat);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"{nameof(ReadItem)}: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(ReadItem)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        public AttractionsController(
            ILogger<AttractionsController> logger,
            IAttractionService service
        )
        {
            _logger = logger;
            _service = service;
        }
    }
}
