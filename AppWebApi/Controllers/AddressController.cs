using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Dto;
using Services;

namespace AppWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AddressesController : Controller
    {
        readonly ILogger<AddressesController> _logger;
        readonly IAddressService _service;

        // GET: api/addresses/read?seeded=true&flat=false&filter=&pageNumber=0&pageSize=20
        [HttpGet()]
        [ActionName("Read")]
        [ProducesResponseType(200, Type = typeof(ResponsePageDto<IAddress>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> Read(
            bool seeded = true,
            bool flat = false,
            string filter = "",
            int pageNumber = 0,
            int pageSize = 20
        )
        {
            try
            {
                _logger.LogInformation($"{nameof(Read)}");
                var result = await _service.ReadAddressesAsync(
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

        // GET: api/addresses/read/{addressId}?flat=false
        [HttpGet("{addressId}")]
        [ActionName("ReadItem")]
        [ProducesResponseType(200, Type = typeof(ResponseItemDto<IAddress>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> ReadItem(Guid addressId, bool flat = false)
        {
            try
            {
                _logger.LogInformation($"{nameof(ReadItem)}: {addressId}");
                var result = await _service.ReadAddressAsync(addressId, flat);
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

        public AddressesController(ILogger<AddressesController> logger, IAddressService service)
        {
            _logger = logger;
            _service = service;
        }
    }
}
