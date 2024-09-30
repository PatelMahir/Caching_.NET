using Caching_.NET.DB;
using Caching_.NET.Interfaces;
using Caching_.NET.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
namespace Caching_.NET.Repository
{
    public class LocationRepository
    {
        private readonly ICustomCache cache;
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);

        public LocationRepository(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }
        public async Task<List<Country>> GetCountriesAsync()
        {
            var cacheKey = "Countries";
            if (!_cache.TryGetValue(cacheKey, out List<Country>? countries))
            {
                countries = await _context.Countries.ToListAsync();
                _cache.Set(cacheKey, countries, _cacheExpiration);
            }
            return countries ?? new List<Country>();
        }

        public async Task<List<State>> GetStatesAsync(int countryId)
        {
            string cacheKey = $"States_{countryId}";

            if (!_cache.TryGetValue(cacheKey, out List<State>? states))
            {
                states = await _context.States.Where(s => s.CountryId == countryId).ToListAsync();
                _cache.Set(cacheKey, states, _cacheExpiration);
            }

            return states ?? new List<State>();
        }
        public async Task<List<City>> GetCitiesAsync(int stateId)
        {
            string cacheKey = $"Cities_{stateId}";

            if (!_cache.TryGetValue(cacheKey, out List<City>? cities))
            {
                cities = await _context.Cities.Where(c => c.StateId == stateId).ToListAsync();
                _cache.Set(cacheKey, cities, _cacheExpiration);
            }
            return cities ?? new List<City>();
        }
        public void ClearAllCache()
        {
            if(_cache is MemoryCache concreteMemoryCache)
            {
                concreteMemoryCache.Clear();
            }
        }
        public async Task UpdateCountry(Country updatedCountry)
        {
            _context.Countries.Update(updatedCountry);
            await _context.SaveChangesAsync();
            RemoveCountriesFromCache();
        }
        public void RemoveCountriesFromCache()
        {
            var cacheKey = "Countries";
            cache.RemoveAsync(cacheKey);
        }
    }
}