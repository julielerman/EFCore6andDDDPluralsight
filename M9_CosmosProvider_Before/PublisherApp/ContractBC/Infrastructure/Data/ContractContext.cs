using ContractBC.ContractAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PublisherSystem.SharedKernel;
using System;
using System.Reflection.Metadata;

namespace Infrastructure.Data;

public class ContractContext : DbContext
{

    public ContractContext(DbContextOptions<ContractContext> options) : base(options)
    {
    
    }

    public DbSet<Contract> Contracts => Set<Contract>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contract>().Property(c => c.ContractNumber).HasField("_contractNumber");
        modelBuilder.Entity<Contract>().Property(c => c.DateInitiated).HasField("_initiated");
        new ContractVersionEntityTypeConfiguration()
            .Configure(modelBuilder.Entity<ContractVersion>());
    }

    public class ContractVersionEntityTypeConfiguration
        : IEntityTypeConfiguration<ContractVersion>
    {
        public void Configure(EntityTypeBuilder<ContractVersion> builder)
        {
           builder.OwnsMany(v => v.Authors, prop =>
             {
                 prop.ToTable("ContractVersion_Authors");
                 prop.OwnsOne(a => a.Name);
             });
            builder.OwnsOne(v => v.Specs);
            builder.ToTable("ContractVersions");
            builder.Property("_hasRevisedSpecSet");   
            builder.Property(v=>v.Id).ValueGeneratedNever();
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HaveColumnType("decimal(7,2)");
    }
   

}
