using AuthorBookBC.DomainModels;
using Microsoft.EntityFrameworkCore;

namespace AuthorBook.Infrastructure.Data
{
    public class AuthorBookContext:DbContext

    {
        public AuthorBookContext(DbContextOptions<AuthorBookContext> options) : base(options)
        {

        }
        public DbSet<Author> Authors=> Set<Author>();
        public DbSet<Book> Books => Set<Book>();
        public DbSet<PubContract> Contracts => Set<PubContract>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PubContract>().HasKey(c => new { c.AuthorId,c.BookId});
            //modelBuilder.Entity<Author>().HasMany(a => a.Contracts).WithOne();
            //modelBuilder.Entity<Book>().HasMany(a => a.Contracts).WithOne();
            modelBuilder.Entity<Author>().OwnsOne(a => a.Name);
        }
    }
}