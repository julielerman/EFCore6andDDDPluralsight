using ContractBC.ContractAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PublisherSystem.SharedKernel;

namespace Infrastructure.Data;

public class ContractContext : DbContext
{
    private readonly IMediator _mediator;

    public ContractContext(DbContextOptions options) : base(options)
    {
        SavedChanges += ContractContext_SavedChanges;
    }

    public ContractContext(DbContextOptions<ContractContext> options, IMediator mediator) 
        :this(options)
    {
        _mediator = mediator;
    }
    private void ContractContext_SavedChanges(object? sender, SavedChangesEventArgs e)
    {
        // ignore events if no dispatcher provided
        if (_mediator == null) return;

        var entitiesWithEvents = ChangeTracker
            .Entries()
            .Select(e => e.Entity as BaseEntity<Guid>)
            .Where(e => e?.Events != null && e.Events.Any())
            .ToArray();

        foreach (var entity in entitiesWithEvents)
        {
            var events = entity.Events.ToArray();
            entity.Events.Clear();
            foreach (var domainEvent in events)
            {
                _mediator.Publish(domainEvent).ConfigureAwait(false);
            }
        }
    }

    public DbSet<Contract> Contracts => Set<Contract>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
            modelBuilder.Entity<ContractVersion>().OwnsMany(
                v => v.Authors,
            a => { a.OwnsOne(a => a.Name);
                   a.ToTable("ContractVersion_Authors");
            });


        modelBuilder.Entity<ContractVersion>().OwnsOne(v => v.Specs, s =>
              s.Property(p => p.PriceForAddlAuthorCopiesUSD)
         );
        modelBuilder.Entity<Contract>().Property(c => c.ContractNumber).HasField("_contractNumber");
        modelBuilder.Entity<Contract>().Property(c => c.DateInitiated).HasField("_initiated");
        modelBuilder.Entity<ContractVersion>().Property("_hasRevisedSpecSet");
        modelBuilder.Entity<ContractVersion>().ToTable("ContractVersions");
        modelBuilder.Entity<ContractVersion>().Property(v => v.Id).ValueGeneratedNever();
     }
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HaveColumnType("decimal(18, 2)");
    }
}