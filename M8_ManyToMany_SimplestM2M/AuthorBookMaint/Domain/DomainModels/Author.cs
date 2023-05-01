using PublisherSystem.SharedKernel;
using PublisherSystem.SharedKernel.ValueObjects;

namespace AuthorAndBookMaintenance.DomainModels;

public class Author : BaseEntity<Guid>
{
    public Author(PersonName name,
    string preferredName,
    string salutation,
    string emailAddress)
    {
        Id = Guid.NewGuid();
        Name = name;
        PreferredName = preferredName;
        Pronouns = salutation;
        EmailAddress = emailAddress;
    }

    public PersonName Name { get; private set; }
    public string PreferredName { get;  set; }
    public string Pronouns { get;  set; }
    public string EmailAddress { get; set; }
    public string? Phone { get; set; }
    public string? Bio { get; set; }

    public string FullName() => Name.FullName;

    public IEnumerable<Book> Books => _books.ToList();
    private List<Book> _books = new List<Book>();
    public void AddExistingBook(Book book)
    {
        if (!_books.Any(i => i.Equals(book.Id)))
        {
            _books.Add(book);
        }
    }

    public IEnumerable<Guid> ContractIds => _contractIds.ToList();
    private List<Guid> _contractIds = new List<Guid>();
    public void AddContractViaId(Guid contractId)
    {
        if (!_contractIds.Any(i => i.Equals(contractId)))
        {
            _contractIds.Add(contractId);
        }
    }

    private  Author()
    {

    }

}
