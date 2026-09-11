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

        // GET: api/attractions/filtered?category=&title=&description=&country=&city=&pageNumber=1&pageSize=20
        // All parameters are optional; provided ones are combined with AND (substring match, case-insensitive).
        [HttpGet()]
        [ActionName("Filtered")]
        [ProducesResponseType(200, Type = typeof(PagedResult<AttractionListItemDto>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> Filtered(
            string category = null,
            string title = null,
            string description = null,
            string country = null,
            string city = null,
            int pageNumber = 1,
            int pageSize = 20
        )
        {
            try
            {
                _logger.LogInformation($"{nameof(Filtered)}");
                var result = await _service.GetFilteredAsync(
                    category,
                    title,
                    description,
                    country,
                    city,
                    pageNumber,
                    pageSize
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Filtered)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // GET: api/attractions/withoutcomments?pageNumber=1&pageSize=20
        [HttpGet()]
        [ActionName("WithoutComments")]
        [ProducesResponseType(200, Type = typeof(PagedResult<AttractionListItemDto>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> WithoutComments(int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                _logger.LogInformation($"{nameof(WithoutComments)}");
                var result = await _service.GetWithoutCommentsAsync(pageNumber, pageSize);
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
        [ActionName("Detail")]
        [ProducesResponseType(200, Type = typeof(AttractionDetailDto))]
        [ProducesResponseType(404, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> Detail(
            Guid attractionId,
            int commentsPageNumber = 1,
            int commentsPageSize = 20
        )
        {
            try
            {
                _logger.LogInformation($"{nameof(Detail)}: {attractionId}");
                var result = await _service.GetDetailAsync(
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
                _logger.LogError($"{nameof(Detail)}: {ex.Message}");
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
