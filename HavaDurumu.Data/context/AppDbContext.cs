
using HavaDurumu.Data.Entities;
using System.Data.Entity; // Bizim kullandığımız kütüphane bu

namespace HavaDurumu.Data.Context
{
    public class AppDbContext : DbContext
    {
        // Veritabanı bağlantı ismini buraya yazıyoruz
        public AppDbContext() : base("HavaDurumuContext")
        {
        }

        // --- TABLOLAR ---
        public DbSet<User> Users { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Measurement> Measurements { get; set; }
        public DbSet<Prediction> Predictions { get; set; }
        public DbSet<Alert> Alerts { get; set; }

        // --- VIEW'LAR ---
        public DbSet<Entities.ViewModels.StationDetailsView> StationDetailsViews { get; set; }
        public DbSet<Entities.ViewModels.UserProfilesView> UserProfilesViews { get; set; }
        public DbSet<Entities.ViewModels.DailyStationAveragesView> DailyStationAveragesViews { get; set; }
        public DbSet<Entities.ViewModels.CriticalUnresolvedAlertsView> CriticalUnresolvedAlertsViews { get; set; }
        public DbSet<Entities.ViewModels.ModelPerformanceCheckView> ModelPerformanceCheckViews { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Station entity için decimal precision ayarları
            modelBuilder.Entity<Station>()
                .Property(s => s.Latitude)
                .HasPrecision(9, 6);

            modelBuilder.Entity<Station>()
                .Property(s => s.Longitude)
                .HasPrecision(9, 6);

            // Measurement entity için decimal precision ayarları
            modelBuilder.Entity<Measurement>()
                .Property(m => m.PM25_Value)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Measurement>()
                .Property(m => m.CO2_Value)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Measurement>()
                .Property(m => m.Temperature)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Measurement>()
                .Property(m => m.Humidity)
                .HasPrecision(5, 2);

            // Prediction entity için decimal precision ayarları
            modelBuilder.Entity<Prediction>()
                .Property(p => p.PredictedValue)
                .HasPrecision(10, 2);

            // View'lar için mapping - View'lar read-only olduğu için ToTable kullanıyoruz
            modelBuilder.Entity<Entities.ViewModels.StationDetailsView>()
                .ToTable("vw_StationDetails")
                .HasKey(s => s.StationID);

            modelBuilder.Entity<Entities.ViewModels.UserProfilesView>()
                .ToTable("vw_UserProfiles")
                .HasKey(u => u.UserID);

            // DailyStationAveragesView için composite key (StationName ve ReportDate)
            modelBuilder.Entity<Entities.ViewModels.DailyStationAveragesView>()
                .ToTable("vw_DailyStationAverages")
                .HasKey(d => new { d.StationName, d.ReportDate });

            modelBuilder.Entity<Entities.ViewModels.CriticalUnresolvedAlertsView>()
                .ToTable("vw_CriticalUnresolvedAlerts")
                .HasKey(c => c.AlertID);

            // ModelPerformanceCheckView için composite key (StationID ve PredictedDate)
            // PredictedDate nullable olduğu için key olarak kullanamayız, sadece StationID kullanıyoruz
            modelBuilder.Entity<Entities.ViewModels.ModelPerformanceCheckView>()
                .ToTable("vw_ModelPerformanceCheck")
                .HasKey(m => m.StationID);
        }
    }
}