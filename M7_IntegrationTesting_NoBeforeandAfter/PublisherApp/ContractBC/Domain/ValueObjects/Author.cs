using PublisherSystem.SharedKernel.ValueObjects;

namespace ContractBC.ValueObjects;

public class Author
{
    private Author() { }
    public static Author UnsignedAuthor(string firstName, string lastName, 
                                        string email, string phone)
    {
        return new Author(firstName, lastName, email, phone, false, Guid.Empty);
    }

    public static Author SignedAuthor(string firstName, string lastName, 
                                      string email, string phone, Guid signedAuthorId)
    {
        return new Author(firstName, lastName, email, phone, true, signedAuthorId);
    }

    private Author(string firstName, string lastName, string email, 
                   string phone, bool signed, Guid signedId)
    {
        Name = new PersonName(firstName, lastName);
        if (signed)
        { SignedAuthorId = signedId; }
        Signed = signed;
        Phone = phone;
        Email = email;
    }

    public PersonName Name { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public bool Signed { get; private set; }
    public Guid SignedAuthorId { get; private set; }

    public string FullName => Name.FullName;

    public Author FixName(string first, string last)
    {
        return new Author(first, last, Email, Phone, Signed, SignedAuthorId);
    }

    public Author AddPhone(string newphone)
    {
        return new Author(Name.FirstName, Name.LastName, Email, newphone, Signed, SignedAuthorId);
    }

    public Author Copy()
    {
        return new Author(Name.FirstName, Name.LastName, Email, Phone, Signed, SignedAuthorId);
    }

    public override bool Equals(object? obj)
    {
        return obj is Author author &&
               Signed == author.Signed &&
               SignedAuthorId.Equals(author.SignedAuthorId) &&
               Email == author.Email &&
               Phone == author.Phone &&
               EqualityComparer<PersonName>.Default.Equals(Name, author.Name);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Signed, SignedAuthorId, Email, Phone, Name);
    }
}