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

        // GET: api/attractions/read/{attractionId}?flat=false
        [HttpGet("{attractionId}")]
        [ActionName("Read")]
        [ProducesResponseType(200, Type = typeof(ResponseItemDto<IAttraction>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> Read(Guid attractionId, bool flat = false)
        {
            try
            {
                _logger.LogInformation($"{nameof(Read)}: {attractionId}");
                var result = await _service.ReadAttractionAsync(attractionId, flat);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"{nameof(Read)}: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Read)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // GET: api/attractions/readpage?seeded=true&flat=false&category=&title=&description=&country=&city=&onlyWithoutComments=false&pageNumber=0&pageSize=20
        [HttpGet()]
        [ActionName("ReadPage")]
        [ProducesResponseType(200, Type = typeof(ResponsePageDto<IAttraction>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> ReadPage(
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
                _logger.LogInformation($"{nameof(ReadPage)}");
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
                _logger.LogError($"{nameof(ReadPage)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // POST: api/attractions/createitem
        [HttpPost()]
        [ActionName("CreateItem")]
        [ProducesResponseType(200, Type = typeof(ResponseItemDto<IAttraction>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> CreateItem([FromBody] AttractionCuDto item)
        {
            try
            {
                item.EnsureValidity();
                _logger.LogInformation($"{nameof(CreateItem)}");

                var model = await _service.CreateAttractionAsync(item);
                _logger.LogInformation($"Attraction {model.Item.AttractionId} created");

                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(CreateItem)}: {ex.Message}");
                return BadRequest($"Could not create. Error: {ex.Message}");
            }
        }

        // PUT: api/attractions/updateitem/{attractionId}
        [HttpPut("{attractionId}")]
        [ActionName("UpdateItem")]
        [ProducesResponseType(200, Type = typeof(ResponseItemDto<IAttraction>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> UpdateItem(
            Guid attractionId,
            [FromBody] AttractionCuDto item
        )
        {
            try
            {
                item.EnsureValidity();
                if (item.AttractionId != attractionId)
                    throw new ArgumentException("Id mismatch between route and body");

                _logger.LogInformation($"{nameof(UpdateItem)}: {attractionId}");

                var model = await _service.UpdateAttractionAsync(item);
                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(UpdateItem)}: {ex.Message}");
                return BadRequest($"Could not update. Error: {ex.Message}");
            }
        }

        // DELETE: api/attractions/deleteitem/{attractionId}
        // deletes the attraction and (via DB cascade) all its comments.
        [HttpDelete("{attractionId}")]
        [ActionName("DeleteItem")]
        [ProducesResponseType(200, Type = typeof(ResponseItemDto<IAttraction>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> DeleteItem(Guid attractionId)
        {
            try
            {
                _logger.LogInformation($"{nameof(DeleteItem)}: {attractionId}");
                var model = await _service.DeleteAttractionAsync(attractionId);
                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(DeleteItem)}: {ex.Message}");
                return BadRequest($"Could not delete. Error: {ex.Message}");
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
