namespace ExchangeRatesAPI.Models
{
    public class ExchangeRate
    {
        public int Id { get; set; }
        public string BaseCurrency { get; set; } //base
        public string TargetCurrency { get; set; } = null!;//rates.BGN

        public decimal Rate { get; set; } //rates.AUD
        public DateTime Date { get; set; } //date
    }

}
