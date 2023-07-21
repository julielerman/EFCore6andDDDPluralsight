using PublisherSystem.SharedKernel;
using PublisherSystem.SharedKernel.ValueObjects;

namespace AuthorBookBC.DomainModels;

public class Author : BaseEntity<Guid>
{
    public PersonName Name { get; private set; }
    public string? PreferredName { get;  set; }
    public string? Pronouns { get;  set; }
    public string EmailAddress { get; set; }
    public string? Phone { get; set; }
    public string? Bio { get; set; }
     public List<PubContract> Contracts { get; set; }
  
    public Author(PersonName name,
      string preferredName,
      string salutation,
      string emailAddress,string phone)
    {
        Id = Guid.NewGuid();
        Name = name;
        PreferredName = preferredName;
        Pronouns = salutation;
        EmailAddress = emailAddress;
        Phone = phone;
    }

    public Author(PersonName name, string emailAddress,string phone)
    {
        Id = Guid.NewGuid();
        Name = name;
        Phone = phone;
        EmailAddress = emailAddress;
    }

    public string FullName() => Name.FullName;
    private  Author()
    {

    }

}
