using ExchangeRatesAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ExchangeRatesAPI.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Currency>()
                .HasMany<ExchangeRate>()
                .WithOne()
                .HasForeignKey(er => er.BaseCurrency)
                .HasForeignKey(er => er.TargetCurrency);
        }
    }

}
