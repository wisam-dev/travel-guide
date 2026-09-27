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

        // GET: api/addresses/read/{addressId}?flat=false
        [HttpGet("{addressId}")]
        [ActionName("Read")]
        [ProducesResponseType(200, Type = typeof(ResponseItemDto<IAddress>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> Read(Guid addressId, bool flat = false)
        {
            try
            {
                _logger.LogInformation($"{nameof(Read)}: {addressId}");
                var result = await _service.ReadAddressAsync(addressId, flat);
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

        // GET: api/addresses/readpage?seeded=true&flat=false&filter=&pageNumber=0&pageSize=20
        [HttpGet()]
        [ActionName("ReadPage")]
        [ProducesResponseType(200, Type = typeof(ResponsePageDto<IAddress>))]
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
                _logger.LogError($"{nameof(ReadPage)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // POST: api/addresses/createitem
        [HttpPost()]
        [ActionName("CreateItem")]
        [ProducesResponseType(200, Type = typeof(ResponseItemDto<IAddress>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> CreateItem([FromBody] AddressCuDto item)
        {
            try
            {
                item.EnsureValidity();
                _logger.LogInformation($"{nameof(CreateItem)}");

                var model = await _service.CreateAddressAsync(item);
                _logger.LogInformation($"Address {model.Item.AddressId} created");

                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(CreateItem)}: {ex.Message}");
                return BadRequest($"Could not create. Error: {ex.Message}");
            }
        }

        // PUT: api/addresses/updateitem/{addressId}
        [HttpPut("{addressId}")]
        [ActionName("UpdateItem")]
        [ProducesResponseType(200, Type = typeof(ResponseItemDto<IAddress>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> UpdateItem(Guid addressId, [FromBody] AddressCuDto item)
        {
            try
            {
                item.EnsureValidity();
                if (item.AddressId != addressId)
                    throw new ArgumentException("Id mismatch between route and body");

                _logger.LogInformation($"{nameof(UpdateItem)}: {addressId}");

                var model = await _service.UpdateAddressAsync(item);
                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(UpdateItem)}: {ex.Message}");
                return BadRequest($"Could not update. Error: {ex.Message}");
            }
        }

        // DELETE: api/addresses/deleteitem/{addressId}
        [HttpDelete("{addressId}")]
        [ActionName("DeleteItem")]
        [ProducesResponseType(200, Type = typeof(ResponseItemDto<IAddress>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> DeleteItem(Guid addressId)
        {
            try
            {
                _logger.LogInformation($"{nameof(DeleteItem)}: {addressId}");
                var model = await _service.DeleteAddressAsync(addressId);
                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(DeleteItem)}: {ex.Message}");
                return BadRequest($"Could not delete. Error: {ex.Message}");
            }
        }

        public AddressesController(ILogger<AddressesController> logger, IAddressService service)
        {
            _logger = logger;
            _service = service;
        }
    }
}
