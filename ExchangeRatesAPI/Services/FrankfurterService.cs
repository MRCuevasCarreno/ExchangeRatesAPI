using ExchangeRatesAPI.Data;
using ExchangeRatesAPI.Models;
using Newtonsoft.Json;
using System.Globalization;


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
            var response = await _httpClient.GetAsync($"https://api.frankfurter.app/latest?base={baseCurrency}");
            Console.WriteLine("endpoint scoped https://api.frankfurter.app/latest?base=" + baseCurrency);
            response.EnsureSuccessStatusCode();
            var apiResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine("apiResponse: " + apiResponse);
            var data = JsonConvert.DeserializeObject<FrankfurterResponse>(apiResponse);
            Console.WriteLine("data: " + data);
            Console.WriteLine("data.Base: " + data.Base);
            Console.WriteLine("data.Date: " + data.Date);
            foreach (var rate in data.Rates)
            {
                var exchangeRate = new ExchangeRate
                {
                    
                    BaseCurrency = data.Base,
                    TargetCurrency = data.Rates,
                    Rate = rate.Value,
                    Date = DateTime.Parse(data.Date)
                };
                _context.ExchangeRates.Add(exchangeRate);
            }

            await _context.SaveChangesAsync();
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
