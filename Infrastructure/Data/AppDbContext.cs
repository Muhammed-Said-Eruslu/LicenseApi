using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;



namespace Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<License> Licenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Lisans tablosu için temel konfigürasyon
            modelBuilder.Entity<License>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.LicenseKey).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CustomerEmail).IsRequired().HasMaxLength(255);

                // EnabledModules alanı için JSON conversion
                entity.Property(e => e.EnabledModules)
         .HasConversion(
             v => JsonSerializer.Serialize(v, new JsonSerializerOptions()), // Listeyi JSON string olarak kaydet
             v => string.IsNullOrEmpty(v)
                     ? new List<string>()
                     : JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions())! // JSON’dan listeye çevir
         )
         .HasColumnType("nvarchar(max)");
            });
        }
    }
}
