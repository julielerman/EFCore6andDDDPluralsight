using AuthorAndBookMaintenance.ValueObjects;
using PublisherSystem.SharedKernel;
using PublisherSystem.SharedKernel.ValueObjects;
using System.Collections.Generic;

namespace AuthorAndBookMaintenance.DomainModels;

public class Author : BaseEntity<Guid>
{
    public PersonName Name { get; private set; }
    public string PreferredName { get;  set; }
    public string Pronouns { get;  set; }
    public string EmailAddress { get; set; }
    public string Phone { get; set; }
    public string? Bio { get; set; }
    public IEnumerable<Book> Books => _books.ToList();
    private List<Book> _books=new List<Book>();
    public IEnumerable<Contract> Contracts => _contracts.ToList();
    private List<Contract> _contracts = new List<Contract>();


    public void AssignNewlyContractedBook(string title,Guid contractId,DateTime contractedDate)
    {
        List<string> _titles = _books.Select(book => book.Title).ToList();
        if (!_titles.Any(t => t == title))
        {
            var book = new Book(Id, title, contractId, contractedDate);
            _books.Add(book);
           
        }
     }
    public void AddExistingBook(Book book)
    {
        if (!_books.Any(i => i.Equals(book.Id)))
        {
            _books.Add(book);
        }

    }
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

    public string FullName() => Name.FullName;


}
