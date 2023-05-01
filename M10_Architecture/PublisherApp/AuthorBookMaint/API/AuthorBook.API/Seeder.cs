using AuthorAndBookMaintenance.DomainModels;
using AuthorBook.Infrastructure.Data;
using PublisherSystem.SharedKernel.ValueObjects;

namespace AuthorBook.API
{
    public class Seeder
    {
        static AuthorBookContext _context;
        public async static Task SeedTheData(AuthorBookContext context)
        {
            _context = context;
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
            var author1 = new Author(new PersonName("Julie", "Lerman"),
               "Julie", "she/her", "julie@fakesite.com");
            var author2 = new Author(new PersonName("Roland", "Guijt"),
                "Roland", "he/him", "roland@fakesite.com");
            var author3 = new Author(new PersonName("Stonna", "Edelman"),
                "Stonna", "she/her", "stonna@fakesite.com");
            var author4 = new Author(new PersonName("Liz", "Lemon"),
                "Liz", "they/them", "liz@fakesite.com");
            _context.Authors.AddRange(author1, author2, author3, author4);
            await _context.SaveChangesAsync();

        }
    }
}
