
using PublisherSystem.SharedKernel;

namespace AuthorBookBC.DomainModels;

public class Book : BaseEntity<Guid>
{
    private Genres _allGenres;
    private string booktitle;

 
  

       public string Title { get; set; }
    public DateTime PublishDate { get; set; }
    public string? ISBN { get; set; }
    public decimal USListPrice { get; set; }
    public FictionNonFiction fictionOrNonFiction { get; private set; }
    public Genres PrimaryGenre { get; private set; }
    public Genres AllGenres => _allGenres;
    public List<PubContract> Contracts { get; set; }

     public void AddGenre(Genres genres)
    {
        _allGenres |= (genres);
    }
    public override string ToString()
    {
        return Title;
    }

    private Book()
    {

    }

    public Book(string booktitle)
    {
        Title = booktitle;
        Id=Guid.NewGuid();
    }
}



