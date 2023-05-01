using PublisherSystem.SharedKernel;

namespace AuthorAndBookMaintenance.DomainModels;

public class Book : BaseEntity<Guid>
{
    private Genres _allGenres;
    private List<Author> _authors; //quietly here to help EF Core mapping

    public Book(Guid authorId, string title, Genres primaryGenre, FictionNonFiction fictionorNon)
    {
        Id = Guid.NewGuid();
        _authorIds.Add(authorId);
        Title = title;
        PrimaryGenre = primaryGenre;
        _allGenres = primaryGenre;
        FictionOrNonFiction = fictionorNon;
    }
    public Book(List<Guid> authorIds, string title, Guid contractId, DateTime contractedDate)
    {
        Id = Guid.NewGuid();
        _authorIds.AddRange(authorIds);
        Title = title;
        ContractId = contractId;
        ContractedDate = contractedDate;

    }
    public Book(Guid authorId, string title, Guid contractId, DateTime contractedDate)
    {
        Id = Guid.NewGuid();
        _authorIds.Add(authorId);
        Title = title;
        ContractId = contractId;
        ContractedDate = contractedDate;
    }
    public Book(string title, Guid contractId, DateTime contractedDate)
    {
        Id = Guid.NewGuid();
        Title = title;
        ContractId = contractId;
        ContractedDate = contractedDate;
    }
    
    public string Title { get; set; }
    public DateTime PublishDate { get; set; }
    public string? ISBN { get; set; }
    public decimal USListPrice { get; set; }
    public FictionNonFiction FictionOrNonFiction { get; private set; }
    public Genres PrimaryGenre { get; private set; }
    public Genres AllGenres => _allGenres;
  public bool DetailsComplete { get; private set; } 
    public DateTime ContractedDate { get; set; }
    public Guid ContractId { get; set; }
    private List<Guid> _authorIds = new List<Guid>();
    public IEnumerable<Guid> AuthorIds => _authorIds;
    public void AddAuthorId(Guid authorId)
    {
        if (!_authorIds.Any(a => a.Equals(authorId)))
        {
            _authorIds.Add(authorId);
        }
    }
    public void RemoveAuthorId(Guid authorId)
    {
        if (_authorIds.Any(a => a.Equals(authorId)))
        {
            _authorIds.Remove(authorId);
        }
    }

    public void AddGenre(Genres genres)
    {
        _allGenres |= (genres);
    }
    public override string ToString()
    {
        return Title;
    }

    public void DetailsCompleted()
    {
        DetailsComplete = true;
    }
    private Book()
    {

    }
}



