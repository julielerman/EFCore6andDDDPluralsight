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
        modelBuilder.Entity<Contract>().Ignore(c => c.Versions);
        modelBuilder.Entity<Contract>(c => c.OwnsMany(c => c.Versions,
            owned =>
            {
                owned.Property("_hasRevisedSpecSet");
                owned.Property(v => v.Id).ValueGeneratedNever();
            }
            ));
      }

  
}
