using DalaProject.Models;
using DalaProject.Models;
using Microsoft.EntityFrameworkCore;


namespace DalaProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<OwnerFermer> OwnerFermers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Ground> Grounds { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<MarketProduct> MarketProducts { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>().ToTable("users")
                .HasIndex(u => u.Email)
                .IsUnique();

            // OwnerFermer M:N связь
            modelBuilder.Entity<OwnerFermer>().ToTable("owner_fermers")
                .HasOne(of => of.Owner)
                .WithMany(u => u.Owners)
                .HasForeignKey(of => of.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OwnerFermer>().ToTable("owner_fermers")
                .HasOne(of => of.Fermer)
                .WithMany(u => u.Fermers)
                .HasForeignKey(of => of.FermerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Company
            modelBuilder.Entity<Company>().ToTable("companies")
                .HasOne(c => c.Owner)
                .WithMany()
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ground
            modelBuilder.Entity<Ground>().ToTable("grounds")
                .HasOne(g => g.Company)
                .WithMany(c => c.Grounds)
                .HasForeignKey(g => g.CompanyId);

            modelBuilder.Entity<Ground>().ToTable("grounds")
                .HasOne(g => g.Fermer)
                .WithMany()
                .HasForeignKey(g => g.FermerId)
                .OnDelete(DeleteBehavior.SetNull);

            // Season
            modelBuilder.Entity<Season>().ToTable("seasons")
                .HasOne(s => s.Ground)
                .WithMany(g => g.Seasons)
                .HasForeignKey(s => s.GroundId);

            // Product
            modelBuilder.Entity<Product>().ToTable("products")
                .HasOne(p => p.Season)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SeasonId);

            // MarketProduct
            modelBuilder.Entity<MarketProduct>().ToTable("market_products")
                .HasOne(mp => mp.Fermer)
                .WithMany(u => u.MarketProducts)
                .HasForeignKey(mp => mp.FermerId);

            // Report
            modelBuilder.Entity<Report>().ToTable("reports")
                .HasOne(r => r.Season)
                .WithMany(s => s.Reports)
                .HasForeignKey(r => r.SeasonId);

            modelBuilder.Entity<Report>().ToTable("reports")
                .HasOne(r => r.Fermer)
                .WithMany()
                .HasForeignKey(r => r.FermerId);

            // RefreshToken
            modelBuilder.Entity<RefreshToken>().ToTable("refresh_tokens")
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
