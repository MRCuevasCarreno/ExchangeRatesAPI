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

        //Implementar endpoints que permitan obtener tasas de cambio desde la API de Frankfurter y almacenarlas en la base de datos. OK
        [HttpGet("latest")]
        [ResponseCache(Duration = 15, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> GetLatestRates()
        {
            var latestRates = await _frankfurterService.GetLatestRatesAsync();
            return Ok(latestRates);
        }

        //Los endpoints deben ser capaces de manejar diferentes tipos de consultas (tasas de cambio más recientes, históricas, series temporales y conversiones). OK
        [HttpGet("historical/{date}")]
        [ResponseCache(Duration = 15, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> GetHistoricalRate(string date)
        {
            var historicalRate = await _frankfurterService.GetHistoricalRateAsync(date);
            return Ok(historicalRate);
        }

        //Los endpoints deben ser capaces de manejar diferentes tipos de consultas (tasas de cambio más recientes, históricas, series temporales y conversiones). OK
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
            if (baseCurrency is null)
            {
                return NotFound();
            }

            await _frankfurterService.FetchAndStoreRates(baseCurrency);
            return Ok("Rentabilidad de Moneda Actualizada.");
        }

        //o	GET /rates/currency/{baseCurrency}: Devuelve las tasas de cambio por moneda base. OK
        [HttpGet("/rates/currency/targetCurrencyByBaseCurrency{baseCurrency}")]
        [ResponseCache(Duration = 15, Location = ResponseCacheLocation.Client)]
        public async Task<List<ExchangeRate>> GetAverageRate(string baseCurrency)
        {
            return await _context.ExchangeRates
                .Where(a => a.BaseCurrency == baseCurrency)
                .OrderBy(a => a.BaseCurrency)
                .ToListAsync();
        }

        //o	GET /rates/average?base={baseCurrency}&target={targetCurrency}&start={startDate}&end={endDate}: Devuelve el valor promedio de la tasa de cambio entre las fechas especificadas. NOK
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

        //o	GET /rates/minmax?base={baseCurrency}&target={targetCurrency}&start={startDate}&end={endDate}: Devuelve los valores mínimo y máximo de la tasa de cambio entre las fechas especificadas. NOK
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
