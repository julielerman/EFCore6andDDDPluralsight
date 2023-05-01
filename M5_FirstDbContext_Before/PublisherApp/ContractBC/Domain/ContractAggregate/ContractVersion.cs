using ContractBC.Enums;
using ContractBC.ValueObjects;
using PublisherSystem.SharedKernel;
using static ContractBC.Services.VersionAttributeFactory;

namespace ContractBC.ContractAggregate;

public class ContractVersion:BaseEntity<Guid>
{
    public static ContractVersion CreateNew(BaseAttributes attribs)
    {
        return new ContractVersion(SpecificationSet.Default(),attribs, null,false);
    }

    public static ContractVersion CreateRevision
        (BaseAttributes attribs, SpecificationSet specs,bool hasRevisedSpecs)
    {
        return new ContractVersion(specs, attribs, null,hasRevisedSpecs); ;
    }

    public static ContractVersion CreateRevisionWithCustomDeadline
        (BaseAttributes attribs, SpecificationSet specs, bool hasRevisedSpecs,DateTime deadline)
    {
        return new ContractVersion(specs, attribs, deadline,hasRevisedSpecs); 
    }

    private ContractVersion(SpecificationSet specs,BaseAttributes attribs,
                            DateTime? deadline,bool revisedSpecs)
    {
        Id = Guid.NewGuid();
        Specs=specs;
        _hasRevisedSpecSet = revisedSpecs;
        DateCreated = DateTime.Today ;
        DateSentToAuthors=DateCreated.AddDays(1);
        if (deadline is null)
        { AcceptanceDeadline = DateCreated.AddDays(10); }
        else
        { AcceptanceDeadline=(DateTime)deadline; }
        ModificationReason = attribs.ModReason;
        ModificationDetails = attribs.ModDescription;
        WorkingTitle=attribs.WorkingTitle;
        _authors.AddRange(attribs.Authors);

    }
    private bool _hasRevisedSpecSet;
    public SpecificationSet Specs { get; private set; }
    public Guid ContractId { get; private set; }
    public string WorkingTitle { get; private set; }
    public DateTime DateCreated { get; private set; }
    public DateTime DateSentToAuthors  { get; private set; }
    public DateTime AcceptanceDeadline { get; private set; }
    public string ModificationDetails { get; private set; }
    public ModReason ModificationReason { get; private set; }
    public bool Accepted { get; private set; }
    
    private readonly List<Author> _authors = new List<Author>();
    public IEnumerable<Author> Authors => _authors.ToList();
    public void SentToAuthors(DateTime datesent)
    { 
        DateSentToAuthors= datesent;
    }
    public void AddAuthor(Author author)
    {
        _authors.Add(author);
    }
    
    public void VersionAccepted()
    {
        Accepted = true;
    }

}
