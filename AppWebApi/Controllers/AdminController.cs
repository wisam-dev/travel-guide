using Configuration;
using Configuration.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Models.Dto;
using Newtonsoft.Json;
using Services;

namespace AppWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AdminController : Controller
    {
        readonly ILogger<AdminController> _logger;
        private readonly DbConnectionSetsOptions _dbSetOptions;
        readonly AesEncryptionOptions _aesOptions;
        readonly JwtOptions _jwtOptions;
        readonly VersionOptions _versionOptions;
        readonly IConfiguration _configuration;
        readonly Encryptions _encryptions = null;
        readonly DatabaseConnections _dbConnections = null;
        readonly IAdminService _service;

        //GET: api/admin/environment
        [HttpGet()]
        [ActionName("Environment")]
        [ProducesResponseType(200, Type = typeof(DatabaseConnections.SetupInformation))]
        public IActionResult Environment()
        {
            try
            {
                var info = _dbConnections.SetupInfo;

                _logger.LogInformation(
                    $"{nameof(Environment)}:\n{JsonConvert.SerializeObject(info)}"
                );
                return Ok(info);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Environment)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet()]
        [ActionName("Version")]
        [ProducesResponseType(typeof(VersionOptions), 200)]
        public IActionResult Version()
        {
            try
            {
                _logger.LogInformation(
                    $"{nameof(Version)}:\n{JsonConvert.SerializeObject(_versionOptions)}"
                );
                return Ok(_versionOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving version information");
                return BadRequest(ex.Message);
            }
        }

        //GET: api/admin/seed?nrUsers={n}&nrCities={n}&nrAttractions={n}
        [HttpGet()]
        [ActionName("Seed")]
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> Seed(
            int nrUsers = 50,
            int nrCities = 100,
            int nrAttractions = 1000
        )
        {
            try
            {
                _logger.LogInformation($"{nameof(Seed)}");
                await _service.SeedAsync(nrUsers, nrCities, nrAttractions);

                return Ok(
                    $"Seeded {nrUsers} users, {nrCities} cities, and {nrAttractions} attractions (with random comments) successfully. "
                );
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Seed)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        //GET: api/admin/clearData?onlySeeded=true
        [HttpGet()]
        [ActionName("ClearData")]
        [ProducesResponseType(200, Type = typeof(DataResultInfoDto))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> ClearData(bool onlySeeded = true)
        {
            try
            {
                _logger.LogInformation($"{nameof(ClearData)}: onlySeeded={onlySeeded}");
                var result = await _service.ClearDataAsync(onlySeeded);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(ClearData)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        //GET: api/admin/dbinfo
        [HttpGet()]
        [ActionName("DbInfo")]
        [ProducesResponseType(200, Type = typeof(DbInfoDto))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> DbInfo()
        {
            try
            {
                _logger.LogInformation($"{nameof(DbInfo)}");
                var result = await _service.GetDbInfoAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(DbInfo)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        //GET: api/admin/log
        [HttpGet()]
        [ActionName("Log")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<LogMessage>))]
        public async Task<IActionResult> Log([FromServices] ILoggerProvider _loggerProvider)
        {
            if (_loggerProvider is InMemoryLoggerProvider cl)
            {
                return Ok(await cl.MessagesAsync);
            }
            return Ok("No messages in log");
        }

        public AdminController(
            ILogger<AdminController> logger,
            IConfiguration configuration,
            IOptions<DbConnectionSetsOptions> dbSetOptions,
            IOptions<AesEncryptionOptions> aesOptions,
            IOptions<JwtOptions> jwtOptions,
            IOptions<VersionOptions> versionOptions,
            Encryptions encryptions,
            DatabaseConnections dbConnections,
            IAdminService service
        )
        {
            _logger = logger;

            _dbSetOptions = dbSetOptions.Value;
            _aesOptions = aesOptions.Value;
            _jwtOptions = jwtOptions.Value;
            _versionOptions = versionOptions.Value;
            _configuration = configuration;

            _encryptions = encryptions;
            _dbConnections = dbConnections;

            _service = service;
        }
    }
}
