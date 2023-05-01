using AuthorBookBC.Enums;
using PublisherSystem.SharedKernel;

namespace AuthorAndBookMaintenance.DomainModels;

public class Book : BaseEntity<Guid>
{
    private Genres _allGenres;
    private Book(){}

    public Book(Author author, string title, Genres primaryGenre, bool fiction)
    {
        Id = Guid.NewGuid();
        _authors.Add(author);
        Title = title;
        PrimaryGenre = primaryGenre;
        _allGenres = primaryGenre;
    }

    public Book(List<Author> authors, string title, Guid ContractId, DateTime contractedDate)
    {
        Id = Guid.NewGuid();
        _authors.AddRange(authors);
        Title = title;
    }
    public Book(Author author, string title, Guid ContractId, DateTime contractedDate)
    {
        Id = Guid.NewGuid();
        _authors.Add(author);
        Title = title;
    }
     public string Title { get; set; }
    public DateTime PublishDate { get; set; }
    public string? ISBN { get; set; }
    public decimal USListPrice { get; set; }
    public FictionNonFiction fictionOrNonFiction { get; private set; }
    public Genres PrimaryGenre { get; private set; }
    public Genres AllGenres => _allGenres;
    public bool NewlyContractedRequiresDetails { get; private set; }
    public DateTime ContractedDate { get; set; }
    public Guid ContractId { get; set; }

    public void AddGenre(Genres genres)
    {
        _allGenres |= (genres);
    }
    public override string ToString()
    {
        return Title;
    }
    
    private List<Author> _authors = new List<Author>();
    public IEnumerable<Author> Authors => _authors;
    public void AddAuthor(Author author)
    {
        if (!_authors.Any(a => a.Id.Equals(author.Id)))
        {
            _authors.Add(author);
        }
    }
 
}



