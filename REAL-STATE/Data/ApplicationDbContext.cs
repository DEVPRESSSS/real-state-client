using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using REAL_STATE.Models;

namespace REAL_STATE.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>

    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Developers> Developers => Set<Developers>();
        public DbSet<Agent> Agents => Set<Agent>();
        public DbSet<Property> Properties => Set<Property>();
        public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
        public DbSet<Bookmark> Bookmarks => Set<Bookmark>();
        public DbSet<Inquiry> Inquiries => Set<Inquiry>();
        public DbSet<ViewingAppointment> ViewingAppointments => Set<ViewingAppointment>();
        public DbSet<Referral> Referrals => Set<Referral>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------------------------------------------------------
            // ApplicationUser (extends IdentityUser)
            // ---------------------------------------------------------
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.FullName)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(u => u.ProfileImageUrl)
                      .HasMaxLength(500);

                entity.Property(u => u.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.HasMany(u => u.Bookmarks)
                      .WithOne(b => b.User)
                      .HasForeignKey(b => b.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.Inquiries)
                      .WithOne(i => i.User)
                      .HasForeignKey(i => i.UserId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(u => u.ViewingAppointments)
                      .WithOne(v => v.User)
                      .HasForeignKey(v => v.UserId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(u => u.AgentProfiles)
                      .WithOne(a => a.User)
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ---------------------------------------------------------
            // Developers
            // ---------------------------------------------------------
            modelBuilder.Entity<Developers>(entity =>
            {
                entity.ToTable("Developers");
                entity.HasKey(d => d.Id);

                entity.Property(d => d.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(d => d.Slug)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(d => d.LogoUrl).HasMaxLength(500);
                entity.Property(d => d.Description).HasMaxLength(4000);
                entity.Property(d => d.WebsiteUrl).HasMaxLength(500);

                entity.Property(d => d.IsActive).HasDefaultValue(true);
                entity.Property(d => d.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(d => d.Slug).IsUnique();
                entity.HasIndex(d => d.Name);

                // Developer -> Properties (Restrict: can't delete a developer that has listings)
                entity.HasMany(d => d.Properties)
                      .WithOne(p => p.Developer)
                      .HasForeignKey(p => p.DeveloperId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ---------------------------------------------------------
            // Agent
            // ---------------------------------------------------------
            modelBuilder.Entity<Agent>(entity =>
            {
                entity.ToTable("Agents");
                entity.HasKey(a => a.Id);

                entity.Property(a => a.FullName)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(a => a.Email)
                      .IsRequired()
                      .HasMaxLength(256);

                entity.Property(a => a.Phone).HasMaxLength(30);
                entity.Property(a => a.PhotoUrl).HasMaxLength(500);
                entity.Property(a => a.Bio).HasMaxLength(4000);
                entity.Property(a => a.LicenseNumber).HasMaxLength(50);

                entity.Property(a => a.Role)
                      .IsRequired()
                      .HasMaxLength(30)
                      .HasDefaultValue("Agent");

                entity.Property(a => a.IsActive).HasDefaultValue(true);

                entity.HasIndex(a => a.Email).IsUnique();
                entity.HasIndex(a => a.UserId);

                // Agent -> Properties (agent removal keeps the listing)
                entity.HasMany(a => a.Properties)
                      .WithOne(p => p.Agent)
                      .HasForeignKey(p => p.AgentId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(a => a.Inquiries)
                      .WithOne(i => i.Agent)
                      .HasForeignKey(i => i.AgentId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(a => a.ViewingAppointments)
                      .WithOne(v => v.Agent)
                      .HasForeignKey(v => v.AgentId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ---------------------------------------------------------
            // Property
            // ---------------------------------------------------------
            modelBuilder.Entity<Property>(entity =>
            {
                entity.ToTable("Properties");
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Title)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(p => p.Slug)
                      .IsRequired()
                      .HasMaxLength(250);

                entity.Property(p => p.Description).HasMaxLength(8000);

                entity.Property(p => p.PropertyType)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(p => p.ListingType)
                      .IsRequired()
                      .HasMaxLength(20)
                      .HasDefaultValue("Sale");

                entity.Property(p => p.Price).HasPrecision(18, 2);
                entity.Property(p => p.FloorAreaSqFt).HasPrecision(18, 2);

                entity.Property(p => p.Address).HasMaxLength(300);

                entity.Property(p => p.City)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(p => p.Province).HasMaxLength(100);

                entity.Property(p => p.Country)
                      .IsRequired()
                      .HasMaxLength(100)
                      .HasDefaultValue("Philippines");

                entity.Property(p => p.Status)
                      .IsRequired()
                      .HasMaxLength(30)
                      .HasDefaultValue("Available");

                entity.Property(p => p.IsFeatured).HasDefaultValue(false);
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Indexes for search / filtering
                entity.HasIndex(p => p.Slug).IsUnique();
                entity.HasIndex(p => new { p.ListingType, p.Status });
                entity.HasIndex(p => p.City);
                entity.HasIndex(p => p.PropertyType);
                entity.HasIndex(p => p.Price);
                entity.HasIndex(p => p.IsFeatured);
                entity.HasIndex(p => p.DeveloperId);
                entity.HasIndex(p => p.AgentId);

                // Property -> Images
                entity.HasMany(p => p.Images)
                      .WithOne(i => i.Property)
                      .HasForeignKey(i => i.PropertyId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Property -> Bookmarks
                entity.HasMany(p => p.Bookmarks)
                      .WithOne(b => b.Property)
                      .HasForeignKey(b => b.PropertyId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Property -> Inquiries
                entity.HasMany(p => p.Inquiries)
                      .WithOne(i => i.Property)
                      .HasForeignKey(i => i.PropertyId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Property -> ViewingAppointments
                entity.HasMany(p => p.ViewingAppointments)
                      .WithOne(v => v.Property)
                      .HasForeignKey(v => v.PropertyId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ---------------------------------------------------------
            // PropertyImage
            // ---------------------------------------------------------
            modelBuilder.Entity<PropertyImage>(entity =>
            {
                entity.ToTable("PropertyImages");
                entity.HasKey(i => i.Id);

                entity.Property(i => i.ImageUrl)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(i => i.AltText).HasMaxLength(250);
                entity.Property(i => i.DisplayOrder).HasDefaultValue(0);
                entity.Property(i => i.IsPrimary).HasDefaultValue(false);

                entity.HasIndex(i => new { i.PropertyId, i.DisplayOrder });

                // Only one primary image per property (SQL Server filtered index)
                entity.HasIndex(i => i.PropertyId)
                      .IsUnique()
                      .HasFilter("[IsPrimary] = 1")
                      .HasDatabaseName("UX_PropertyImages_OnePrimaryPerProperty");
            });

            // ---------------------------------------------------------
            // Bookmark
            // ---------------------------------------------------------
            modelBuilder.Entity<Bookmark>(entity =>
            {
                entity.ToTable("Bookmarks");
                entity.HasKey(b => b.Id);

                entity.Property(b => b.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                // A user can bookmark a property only once
                entity.HasIndex(b => new { b.UserId, b.PropertyId }).IsUnique();
                entity.HasIndex(b => b.PropertyId);
            });

            // ---------------------------------------------------------
            // Inquiry
            // ---------------------------------------------------------
            modelBuilder.Entity<Inquiry>(entity =>
            {
                entity.ToTable("Inquiries");
                entity.HasKey(i => i.Id);

                entity.Property(i => i.Name)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(i => i.Email)
                      .IsRequired()
                      .HasMaxLength(256);

                entity.Property(i => i.Phone).HasMaxLength(30);

                entity.Property(i => i.Message)
                      .IsRequired()
                      .HasMaxLength(2000);

                entity.Property(i => i.Status)
                      .IsRequired()
                      .HasMaxLength(30)
                      .HasDefaultValue("New");

                entity.Property(i => i.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(i => i.PropertyId);
                entity.HasIndex(i => i.UserId);
                entity.HasIndex(i => i.AgentId);
                entity.HasIndex(i => i.Status);
            });

            // ---------------------------------------------------------
            // ViewingAppointment
            // ---------------------------------------------------------
            modelBuilder.Entity<ViewingAppointment>(entity =>
            {
                entity.ToTable("ViewingAppointments");
                entity.HasKey(v => v.Id);

                entity.Property(v => v.ContactName)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(v => v.ContactEmail)
                      .IsRequired()
                      .HasMaxLength(256);

                entity.Property(v => v.ContactPhone).HasMaxLength(30);
                entity.Property(v => v.Notes).HasMaxLength(1000);

                entity.Property(v => v.Status)
                      .IsRequired()
                      .HasMaxLength(30)
                      .HasDefaultValue("Pending");

                entity.Property(v => v.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(v => v.PropertyId);
                entity.HasIndex(v => v.UserId);
                entity.HasIndex(v => v.AgentId);
                entity.HasIndex(v => v.ScheduledAt);
                entity.HasIndex(v => v.Status);
            });

            // ---------------------------------------------------------
            // Referral
            // ---------------------------------------------------------
            modelBuilder.Entity<Referral>(entity =>
            {
                entity.ToTable("Referrals");
                entity.HasKey(r => r.Id);

                entity.Property(r => r.ReferralCode)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(r => r.Status)
                      .IsRequired()
                      .HasMaxLength(30)
                      .HasDefaultValue("Pending");

                entity.Property(r => r.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(r => r.ReferralCode).IsUnique();
                entity.HasIndex(r => r.ReferrerUserId);
                entity.HasIndex(r => r.ReferredUserId);

                // Two FKs to the same table: use Restrict to avoid
                // SQL Server "multiple cascade paths" errors.
                entity.HasOne(r => r.ReferrerUser)
                      .WithMany()
                      .HasForeignKey(r => r.ReferrerUserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.ReferredUser)
                      .WithMany()
                      .HasForeignKey(r => r.ReferredUserId)
                      .IsRequired(false)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
