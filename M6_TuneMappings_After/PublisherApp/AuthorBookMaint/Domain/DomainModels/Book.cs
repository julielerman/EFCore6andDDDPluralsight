using PublisherSystem.SharedKernel;

namespace AuthorAndBookMaintenance.DomainModels;

        public class Book : BaseEntity<Guid>
{
    private Genres _allGenres;

    public Book(Guid authorId, string title, Genres primaryGenre,bool fiction)
    {
        Id=Guid.NewGuid();
        AuthorIds.Add(authorId);
        Title = title;
        PrimaryGenre = primaryGenre;
        _allGenres = primaryGenre;
    }
    
    public Book(List<Guid> authorIds, string title, Guid ContractId,DateTime contractedDate)
    {
        Id = Guid.NewGuid();
        AuthorIds = authorIds;
        Title = title;
    }
    public Book(Guid authorId, string title, Guid ContractId, DateTime contractedDate)
    {
        Id = Guid.NewGuid();
        AuthorIds.Add(authorId);
        Title = title;
    }

    public List<Guid> AuthorIds { get; private set; }=new List<Guid>();
    //public List<Author> Authors { get; private set; } = new List<Author>();
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
        _allGenres|=(genres);
    }
    public override string ToString()
    {
        return Title;
    }
    public void AddAuthorById(Guid authorId)
    {
        if (!AuthorIds.Any(a => a == authorId))
        {
            AuthorIds.Add(authorId);
        }
    }
}



