using AuthorAndBookMaintenance.DomainModels;
using Microsoft.EntityFrameworkCore;

namespace AuthorBook.Infrastructure.Data
{
    public class AuthorBookContext:DbContext

    {
        public DbSet<Author> Authors=> Set<Author>();
        public DbSet<Book> Books => Set<Book>();
    }
}