using ContractBC.ContractAggregate;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Data;

public class ContractContext : DbContext
{
    public ContractContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Contract> Contracts => Set<Contract>();
}