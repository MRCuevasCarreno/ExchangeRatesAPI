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

        //o	GET /rates: Devuelve la lista de tasas de cambio almacenadas. OK
        [HttpGet("/rates")]
        [ResponseCache(Duration = 15, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> GetCurrencies()
        {
            var currencies = await _context.Currencies.ToListAsync();
            return Ok(currencies);
        }

        //o	GET /rates/{id}: Devuelve una tasa de cambio específica por Id. OK
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

        //o	POST /rates: Crea nuevas tasas de cambio. OK
        [HttpPost("/rates")]
        public async Task<IActionResult> CreateCurrency([FromBody] Currency currency)
        {
            _context.Currencies.Add(currency);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCurrency), new { id = currency.Id }, currency);
        }

        //o	PUT /rates/{id}: Actualiza una tasa de cambio existente por Id. OKo	PUT /rates/{id}: Actualiza una tasa de cambio existente por Id. OK
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

        //o	DELETE /rates/{id}: Elimina una tasa de cambio por Id. OK
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
