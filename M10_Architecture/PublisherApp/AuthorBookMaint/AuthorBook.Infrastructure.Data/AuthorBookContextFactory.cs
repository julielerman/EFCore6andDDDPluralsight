using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
namespace AuthorBook.Infrastructure.Data;

public class AuthorBookContextFactory : IDesignTimeDbContextFactory<AuthorBookContext>
{
    public AuthorBookContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthorBookContext>();
        optionsBuilder.UseSqlServer(
         "Data Source= (localdb)\\MSSQLLocalDB; Initial Catalog=AuthorBookDataTest"); ;
        return new AuthorBookContext(optionsBuilder.Options);
    }
}
