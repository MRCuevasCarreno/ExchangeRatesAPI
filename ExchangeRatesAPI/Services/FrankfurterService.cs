using ExchangeRatesAPI.Data;
using ExchangeRatesAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using System.Globalization;
using System.Text.Json;



namespace ExchangeRatesAPI.Services
{
    public class FrankfurterService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;

        public FrankfurterService(HttpClient httpClient, AppDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }


        public async Task FetchAndStoreRates(string baseCurrency)
        {
            var response = await _httpClient.GetStringAsync($"https://api.frankfurter.app/latest?base={baseCurrency}");
            var data = JsonConvert.DeserializeObject<FrankfurterResponse>(response);

            if (data != null && data.Rates != null)
            {
                // Asegúrate de que la moneda base existe en la tabla Currencies
                if (!_context.Currencies.Any(c => c.Id == baseCurrency))
                {
                    _context.Currencies.Add(new Currency
                    {
                        Id = baseCurrency,
                        Symbol = baseCurrency,
                        Name = baseCurrency // Puedes reemplazar esto con el nombre real de la moneda si lo tienes
                    });
                }

                foreach (var rate in data.Rates)
                {
                    // Asegúrate de que las monedas objetivo existen en la tabla Currencies
                    if (!_context.Currencies.Any(c => c.Id == rate.Key))
                    {
                        _context.Currencies.Add(new Currency
                        {
                            Id = rate.Key,
                            Symbol = rate.Key,
                            Name = rate.Key // Puedes reemplazar esto con el nombre real de la moneda si lo tienes
                        });
                    }

                    var exchangeRate = new ExchangeRate
                    {
                        BaseCurrency = baseCurrency,
                        TargetCurrency = rate.Key,
                        Rate = rate.Value,
                        Date = DateTime.Parse(data.Date)
                    };

                    _context.ExchangeRates.Add(exchangeRate);
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task<FrankfurterResponse> GetLatestRatesAsync()
        {
            var response = await _httpClient.GetAsync($"https://api.frankfurter.app/latest");
            response.EnsureSuccessStatusCode();
            var apiResponse = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<FrankfurterResponse>(apiResponse);
            return data;
        }

        public async Task<FrankfurterResponse> GetHistoricalRateAsync(string date)
        {
            string newFormat = DateTime.ParseExact(date, "dd'-'MM'-'yyyy", CultureInfo.InvariantCulture).ToString("yyyy'-'MM'-'dd");
            Console.WriteLine("Today is " + newFormat);
            var response = await _httpClient.GetAsync($"https://api.frankfurter.app/{newFormat}");
            response.EnsureSuccessStatusCode();
            var apiResponse =  await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<FrankfurterResponse>(apiResponse);
            return data;
        }

        public async Task<FrankfurterHistoricalResponse> GetHistoricalRatesAsync(string startDate, string endDate)
        {
            string startDateFormat = DateTime.ParseExact(startDate, "dd'-'MM'-'yyyy", CultureInfo.InvariantCulture).ToString("yyyy'-'MM'-'dd");
            string endDateFormat = DateTime.ParseExact(endDate, "dd'-'MM'-'yyyy", CultureInfo.InvariantCulture).ToString("yyyy'-'MM'-'dd");
            var response = await _httpClient.GetAsync($"https://api.frankfurter.app/{startDateFormat}..{endDateFormat}");
            response.EnsureSuccessStatusCode();
            var apiResponse = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<FrankfurterHistoricalResponse>(apiResponse);
            return data;
        }
    }


}
