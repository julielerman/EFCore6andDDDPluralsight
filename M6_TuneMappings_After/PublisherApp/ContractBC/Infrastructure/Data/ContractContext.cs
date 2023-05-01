using ContractBC.ContractAggregate;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ContractContext : DbContext
{

    public ContractContext(DbContextOptions<ContractContext> options) : base(options)
    {
    
    }

    public DbSet<Contract> Contracts => Set<Contract>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<ContractVersion>().OwnsMany(v => v.Authors)
        //    .ToTable("ContractVersion_Authors").OwnsOne(a => a.Name);
        modelBuilder.Entity<ContractVersion>().OwnsMany(v => v.Authors, prop =>
        {
            prop.ToTable("ContractVersion_Authors");
            prop.OwnsOne(a => a.Name);
        });

        modelBuilder.Entity<ContractVersion>().OwnsOne(v => v.Specs);
        modelBuilder.Entity<Contract>().Property(c => c.ContractNumber).HasField("_contractNumber");
        modelBuilder.Entity<Contract>().Property(c => c.DateInitiated).HasField("_initiated");
        modelBuilder.Entity<ContractVersion>().Property("_hasRevisedSpecSet");
        modelBuilder.Entity<ContractVersion>().ToTable("ContractVersions");
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HaveColumnType("decimal(7,2)");
    }
   

}
