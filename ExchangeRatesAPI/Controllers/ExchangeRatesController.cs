using ExchangeRatesAPI.Data;
using ExchangeRatesAPI.Models;
using ExchangeRatesAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

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
            return Ok("Profitability Updated.");
        }

        //o	GET /rates/currency/{baseCurrency}: Devuelve las tasas de cambio por moneda base. OK
        [HttpGet("/rates/currency/targetCurrencyByBaseCurrency/{baseCurrency}")]
        [ResponseCache(Duration = 15, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> GetAverageRate(string baseCurrency)
        {
            var exchangeRates = await _context.ExchangeRates
                .Where(a => a.BaseCurrency == baseCurrency)
                .OrderBy(a => a.BaseCurrency)
                .ToListAsync();

            var result = exchangeRates.Select(rate => new
            {
                Id = rate.Id,
                BaseCurrency = rate.BaseCurrency,
                TargetCurrency = rate.TargetCurrency,
                Rate = rate.Rate,
                Date = rate.Date.ToString("dd/MM/yyyy")
            }).ToList();

            return Ok(result);
        }

        //Actualiza las tasas de cambio por moneda base (requiere pasar un body con los datos a actualizar). NOK
        [HttpPut("/rates/currency")]
        public async Task<IActionResult> UpdateBaseCurrency([FromBody] List<ExchangeRate> exchangeRates)
        {
            if (exchangeRates == null || !exchangeRates.Any())
            {
                return BadRequest("No exchange rates provided.");
            }

            foreach (var exchangeRate in exchangeRates)
            {
                var existingRate = await _context.ExchangeRates
                    .FirstOrDefaultAsync(er => er.Id == exchangeRate.Id);

                if (existingRate == null)
                {
                    return NotFound($"Exchange rate with ID {exchangeRate.Id} not found.");
                }

                existingRate.BaseCurrency = exchangeRate.BaseCurrency;
                existingRate.TargetCurrency = exchangeRate.TargetCurrency;
                existingRate.Rate = exchangeRate.Rate;
                existingRate.Date = exchangeRate.Date;

                _context.ExchangeRates.Update(existingRate);
            }

            await _context.SaveChangesAsync();

            var result = exchangeRates.Select(rate => new
            {
                Id = rate.Id,
                BaseCurrency = rate.BaseCurrency,
                TargetCurrency = rate.TargetCurrency,
                Rate = rate.Rate,
                Date = rate.Date.ToString("dd-MM-yyyy") // Formato de la fecha
            }).ToList();

            return Ok(result);
        }

    //o	DELETE /rates/currency/{baseCurrency}: Elimina las tasas de cambio por moneda base. OK
    [HttpDelete("/rates/currency/{baseCurrency}")]
        public async Task<IActionResult> DeleteBaseCurrency(string baseCurrency)
        {
            var targetCurrency = await _context.ExchangeRates.Where(x => x.BaseCurrency == baseCurrency).ToListAsync();
            if (!targetCurrency.Any())
            {
                return NotFound();
            }

            _context.ExchangeRates.RemoveRange(targetCurrency);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
