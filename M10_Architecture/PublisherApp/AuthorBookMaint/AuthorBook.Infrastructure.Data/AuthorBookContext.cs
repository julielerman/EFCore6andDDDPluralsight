using AuthorAndBookMaintenance.DomainModels;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AuthorBook.Infrastructure.Data;

public class AuthorBookContext : DbContext

{
    public AuthorBookContext(DbContextOptions<AuthorBookContext> options) : base(options)
    {  }

    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>().OwnsOne(a => a.Name);
        modelBuilder.Entity<Book>().Property(b => b.AuthorIds)
           .HasConversion(
             v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
             v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions)null));
        modelBuilder.Entity<Author>().HasMany(a => a.Books).WithMany("_authors");
    }
}