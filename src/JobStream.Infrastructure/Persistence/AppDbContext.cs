using JobStream.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobStream.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Job> Jobs => Set<Job>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Job>(entity =>
        {
            entity.ToTable("Jobs");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Location)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Format)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(x => x.Status)
                .IsRequired();

            entity.Property(x => x.Priority)
                .IsRequired();

            entity.Property(x => x.LastError)
                .HasMaxLength(2000);

            entity.Property(x => x.OutputFilePath)
                .HasMaxLength(500);
        });
    }
}