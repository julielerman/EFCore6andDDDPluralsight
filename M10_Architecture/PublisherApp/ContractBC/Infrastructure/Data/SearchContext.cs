using Microsoft.EntityFrameworkCore;
using PublisherSystem.SharedKernel.DTOs;

namespace Infrastructure.Data;


public class SearchContext : DbContext
{
    public SearchContext(DbContextOptions<SearchContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }
    public DbSet<GuidKeyAndDescription> SearchResults => Set<GuidKeyAndDescription>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GuidKeyAndDescription>().HasNoKey();
    }
}
