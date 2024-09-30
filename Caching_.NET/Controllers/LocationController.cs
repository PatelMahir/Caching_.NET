using Caching_.NET.DB;
using Caching_.NET.Interfaces;
using Caching_.NET.Models;
using Caching_.NET.Repository;
using Microsoft.AspNetCore.Mvc;
namespace Caching_.NET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly LocationRepository _repository;
        //private readonly ApplicationDbContext _context;
        //private readonly IMemoryCache _cache;
        private readonly ICustomCache _cache;
        public LocationController(LocationRepository repository, ICustomCache cache)//,ApplicationDbContext context,IMemoryCache cache,)
        {
            _repository = repository;
            //_context = context;
            cache= _cache;
        }
        [HttpGet("countries")]
        public async Task<IActionResult> GetCountries()
        {
            var countries = await _repository.GetCountriesAsync();
            return Ok(countries);
        }
        [HttpGet("states/{countryId}")]
        public async Task<IActionResult> GetStates(int countryId)
        {
            var states = await _repository.GetStatesAsync(countryId);
            return Ok(states);
        }
        [HttpGet("cities/{stateId}")]
        public async Task<IActionResult> GetCities(int stateId)
        {
            var cities = await _repository.GetCitiesAsync(stateId);
            return Ok(cities);
        }
        [HttpPost("ClearCache")]
        public IActionResult ClearCache()
        {
            _repository.ClearAllCache();
            return Ok("All Cache Cleared");
        }
        [HttpPut("countries/{id}")]
        public async Task<IActionResult> UpdateCountry(int id, Country country)
        {
            if (id != country.CountryId)
                return BadRequest();
            try
            {
                await _repository.UpdateCountry(country);
                return NoContent(); // Indicates success with no content to return
            }
            catch (Exception ex)
            {
                if (!CountryExists(id))
                    return NotFound();
                else
                {
                    var customResponse = new
                    {
                        Code = 500,
                        Message = "Internal Server Error",
                        // Do not expose the actual error to the client
                        ErrorMessage = ex.Message
                    };
                    return StatusCode(StatusCodes.Status500InternalServerError, customResponse);
                }
            }
        }
        private bool CountryExists(int id)
        {
            return _repository.GetCountriesAsync().Result.Any(e => e.CountryId == id);
        }
        [HttpGet("AllCaches")]
        public async Task<IActionResult> GetAllCaches()
        {
            var allCaches = await cache.GetAllCachesAsync();
            return Ok(allCaches);
        }
    }
}