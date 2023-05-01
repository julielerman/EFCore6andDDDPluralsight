using ContractBC.Enums;
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
    public Dictionary<string,string> SocialMediaAccounts { get; private set; }
        =new Dictionary<string,string>(); 

    public string FullName => Name.FullName;

    public Author Copy()
    {
        var authorCopy = new Author(Name.FirstName, Name.LastName, Email, Phone, Signed, SignedAuthorId);
        authorCopy.SocialMediaAccounts = SocialMediaAccounts;
        return authorCopy;
    }

    public Author FixName(string first, string last)
    {
        var copy = Copy();
        copy.Name = new PersonName(first, last);
        return copy;
    }

    public Author AddPhone(string newphone)
    {
        var copy = Copy();
        copy.Phone = newphone;
        return copy;
    }
    public Author AddSocialMedia(SocialMedia socialMediaType, string account)
    {
        var copy = Copy();
        copy.SocialMediaAccounts.Add(socialMediaType.ToString(), account);
        return copy;
    }



    public override bool Equals(object? obj)
    {
        return obj is Author author &&
               EqualityComparer<PersonName>.Default.Equals(Name, author.Name) &&
               Email == author.Email &&
               Phone == author.Phone &&
               Signed == author.Signed &&
               SignedAuthorId.Equals(author.SignedAuthorId) &&
               EqualityComparer<Dictionary<string, string>>.Default.Equals(SocialMediaAccounts, author.SocialMediaAccounts);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Email, Phone, Signed, SignedAuthorId, SocialMediaAccounts);
    }

    //public override bool Equals(object? obj)
    //{
    //    return obj is Author author &&
    //           Signed == author.Signed &&
    //           SignedAuthorId.Equals(author.SignedAuthorId) &&
    //           Email == author.Email &&
    //           Phone == author.Phone &&
    //           EqualityComparer<PersonName>.Default.Equals(Name, author.Name);
    //}

    //public override int GetHashCode()
    //{
    //    return HashCode.Combine(Signed, SignedAuthorId, Email, Phone, Name);
    //}
}