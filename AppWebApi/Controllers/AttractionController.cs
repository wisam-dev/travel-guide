using Microsoft.AspNetCore.Mvc;
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

        // GET: api/attractions/read?category=&title=&description=&country=&city=&includeComments=false&pageNumber=1&pageSize=20
        // All filter parameters are optional; provided ones are combined with AND (substring match, case-insensitive).
        // includeComments=true also returns each attraction's full comment list, not just the count.
        [HttpGet()]
        [ActionName("Read")]
        [ProducesResponseType(200, Type = typeof(PagedResult<AttractionListItemDto>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> Read(
            string category = null,
            string title = null,
            string description = null,
            string country = null,
            string city = null,
            bool includeComments = false,
            int pageNumber = 1,
            int pageSize = 20
        )
        {
            try
            {
                _logger.LogInformation($"{nameof(Read)}");
                var result = await _service.ReadAllAsync(
                    category,
                    title,
                    description,
                    country,
                    city,
                    includeComments,
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

        // GET: api/attractions/readwithoutcomments?pageNumber=1&pageSize=20
        [HttpGet()]
        [ActionName("ReadWithoutComments")]
        [ProducesResponseType(200, Type = typeof(PagedResult<AttractionListItemDto>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> WithoutComments(int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                _logger.LogInformation($"{nameof(WithoutComments)}");
                var result = await _service.ReadAllWithoutCommentsAsync(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(WithoutComments)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // GET: api/attractions/detail/{attractionId}?commentsPageNumber=1&commentsPageSize=20
        [HttpGet("{attractionId}")]
        [ActionName("ReadItem")]
        [ProducesResponseType(200, Type = typeof(AttractionDetailDto))]
        [ProducesResponseType(404, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> ReadItem(
            Guid attractionId,
            int commentsPageNumber = 1,
            int commentsPageSize = 20
        )
        {
            try
            {
                _logger.LogInformation($"{nameof(ReadItem)}: {attractionId}");
                var result = await _service.ReadItemAsync(
                    attractionId,
                    commentsPageNumber,
                    commentsPageSize
                );

                if (result == null)
                    return NotFound($"Attraction {attractionId} not found");

                return Ok(result);
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
