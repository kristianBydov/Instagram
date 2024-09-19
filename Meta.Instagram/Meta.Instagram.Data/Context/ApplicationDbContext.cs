using Meta.Instagram.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Meta.Instagram.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            CreateAccountModelBuilder(modelBuilder);
        }

        private void CreateAccountModelBuilder(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(a => a.AccountId);

                entity.Property(a => a.AccountId)
                      .IsRequired();

                entity.Property(a => a.ExternalId)
                      .IsRequired();

                entity.Property(a => a.FirstName)
                      .IsRequired();

                entity.Property(a => a.LastName)
                      .IsRequired();

                entity.Property(a => a.Username)
                      .IsRequired();

                entity.Property(a => a.Email)
                      .IsRequired();

                entity.Property(a => a.Phone)
                      .IsRequired();

                entity.Property(a => a.CreatedAt)
                      .IsRequired()
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(a => a.UpdatedAt)
                      .IsRequired(false);
            });
        }
    }
}
