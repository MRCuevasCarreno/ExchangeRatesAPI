using ExchangeRatesAPI.Data;
using ExchangeRatesAPI.Models;
using ExchangeRatesAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ExchangeRatesAPI.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class ExchangeRatesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly FrankfurterService _frankfurterService;

        public ExchangeRatesController(AppDbContext context, IMemoryCache cache, FrankfurterService frankfurterService)
        {
            _context = context;
            _cache = cache;
            _frankfurterService = frankfurterService;
        }

        [HttpGet("latest")]
        [ResponseCache(Duration = 15, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> GetLatestRates()
        {
            var latestRates = await _frankfurterService.GetLatestRatesAsync();
            return Ok(latestRates);
        }

        [HttpGet("historical/{date}")]
        [ResponseCache(Duration = 15, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> GetHistoricalRate(string date)
        {
            var historicalRate = await _frankfurterService.GetHistoricalRateAsync(date);
            return Ok(historicalRate);
        }

        [HttpGet("history")]
        [ResponseCache(Duration = 15, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> GetHistoricalRates(string startDate, string endDate)
        {
            var historicalRates = await _frankfurterService.GetHistoricalRatesAsync(startDate, endDate);
            return Ok(historicalRates);
        }

        [HttpPost("loadRates")]
        public async Task<IActionResult> FetchAndStoreRates([FromQuery] string baseCurrency)
        {
            await _frankfurterService.FetchAndStoreRates(baseCurrency);
            return Ok("Rates fetched and stored successfully.");
        }

        [HttpGet("rates/average")]
        [ResponseCache(Duration = 15, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> GetAverageRate(string baseCurrency, string targetCurrency, DateTime start, DateTime end)
        {
            //TODO
            var rates = await _context.ExchangeRates
                .Where(r => r.BaseCurrency == baseCurrency && r.TargetCurrency == targetCurrency && r.Date >= start && r.Date <= end)
                .ToListAsync();

            if (!rates.Any())
            {
                return NotFound();
            }

            var averageRate = rates.Average(r => r.Rate);
            return Ok(averageRate);
        }

        [HttpGet("rates/minmax")]
        [ResponseCache(Duration = 15, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> GetMinMaxRate(string baseCurrency, string targetCurrency, DateTime start, DateTime end)
        {
            //TODO
            var rates = await _context.ExchangeRates
                .Where(r => r.BaseCurrency == baseCurrency && r.TargetCurrency == targetCurrency && r.Date >= start && r.Date <= end)
                .ToListAsync();

            if (!rates.Any())
            {
                return NotFound();
            }

            var minRate = rates.Min(r => r.Rate);
            var maxRate = rates.Max(r => r.Rate);
            return Ok(new { MinRate = minRate, MaxRate = maxRate });
        }
    }
}
