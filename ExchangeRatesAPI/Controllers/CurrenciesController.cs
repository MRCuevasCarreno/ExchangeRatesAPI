using ExchangeRatesAPI.Data;
using ExchangeRatesAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRatesAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CurrenciesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CurrenciesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("/rates")]
        [ResponseCache(Duration = 15, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> GetCurrencies()
        {
            var currencies = await _context.Currencies.ToListAsync();
            return Ok(currencies);
        }

        [HttpGet("/rates/{id}")]
        [ResponseCache(Duration = 15, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> GetCurrency(string id)
        {
            var currency = await _context.Currencies.FindAsync(id);
            if (currency == null)
            {
                return NotFound();
            }
            return Ok(currency);
        }

        [HttpPost("/rates")]
        public async Task<IActionResult> CreateCurrency([FromBody] Currency currency)
        {
            _context.Currencies.Add(currency);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCurrency), new { id = currency.Id }, currency);
        }

        [HttpPut("/rates/{id}")]
        public async Task<IActionResult> UpdateCurrency(string id, [FromBody] Currency updatedCurrency)
        {
            var currency = await _context.Currencies.FindAsync(id);
            if (currency == null)
            {
                return NotFound();
            }

            currency.Symbol = updatedCurrency.Symbol;
            currency.Name = updatedCurrency.Name;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("/rates/{id}")]
        public async Task<IActionResult> DeleteCurrency(string id)
        {
            var currency = await _context.Currencies.FindAsync(id);
            if (currency == null)
            {
                return NotFound();
            }

            _context.Currencies.Remove(currency);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

}
