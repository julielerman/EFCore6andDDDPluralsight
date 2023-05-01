using PublisherSystem.SharedKernel;
using System.ComponentModel;

namespace AuthorAndBookMaintenance.DomainModels;

        public class Book : BaseEntity<int>
{
    public Book(int authorId, string title, Category primaryCategory)
    {
        AuthorId = authorId;
        Title = title;
        PrimaryCategory = primaryCategory;
        Categories = new List<Category> { primaryCategory };
    }

    public int AuthorId { get; set; }
    public string Title { get; set; }
    public DateTime PublishDate { get; set; }
    public string? ISBN { get; set; }
    public decimal USListPrice { get; set; }
    public Category PrimaryCategory { get; set; }
    public List<Category> Categories { get; set; } = new List<Category>();

    public override string ToString()
    {
        return Title;
    }
}



