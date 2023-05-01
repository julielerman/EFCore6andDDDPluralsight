using ContractBC.Enums;
using ContractBC.Events;
using ContractBC.Services;
using ContractBC.ValueObjects;
using PublisherSystem.SharedKernel;

namespace ContractBC.ContractAggregate;
public class Contract : BaseEntity<Guid>
{
    private  Contract() {}
    public Contract(DateTime initDate, List<Author> authors, string workingTitle)
    {
        _initiated = initDate;
        Id = Guid.NewGuid();
        var baseattribs =
            VersionAttributeFactory.Create(Id, workingTitle, authors, ModReason.NewContract, "New Contract");

        ContractVersion version;
        version = ContractVersion.CreateNew(baseattribs);

        _contractNumber = GenerateContractNumber(version);
        AddVersion(version);
    }

    public string ContractNumber => _contractNumber;
    public DateTime DateInitiated => _initiated;
    public IEnumerable<ContractVersion> Versions => _versions.ToList();
    public Guid CurrentVersionId { get; private set; }
    public Guid FinalVersionId { get; private set; }
    public bool Completed { get; private set; }
    public DateTime CompletedDate { get; private set; }
    public DateTime Fullfilled { get;private set; }
    private string _contractNumber;
    private DateTime _initiated;
    private readonly List<ContractVersion> _versions = new List<ContractVersion>();

    public void CreateRevisionUsingSameSpecs
        (ModReason modReason, string modDescription, string title, List<Author> authors, DateTime? customDeadline)
    {
        CreateRevision(modReason, modDescription, title, authors, customDeadline, CurrentVersion().Specs.Copy(), true);
    }
  
    public void CreateRevisionUsingNewSpecs
        (ModReason modReason, string modDescription, string title, List<Author> authors, DateTime? customDeadline, SpecificationSet specs)
    {
        CreateRevision(modReason, modDescription, title, authors, customDeadline, specs, false);
    }

    public void CurrentVersionAcceptedVerbally()
    {
        CurrentVersion().VersionAccepted();
    }
    
    public ContractVersion GetVersion(Guid versionId)
    {
        return Versions.SingleOrDefault(v => v.Id == versionId);
    }

    public void AddAuthor(Author author)
    {
        CurrentVersion().AddAuthor(author);
    }

    public void FinalVersionSignedByAllParties()
    {
        Completed = true;
        CompletedDate= DateTime.Now;
        FinalVersionId = CurrentVersionId;
        var contractSignedEvent = new ContractSignedEvent(CurrentVersion(), CompletedDate);
        Events.Add(contractSignedEvent);
    }

    public ContractVersion CurrentVersion()
    { return Versions.Single(v => v.Id == CurrentVersionId); }

    private string GenerateContractNumber(ContractVersion version)
    {
        var date = DateInitiated.ToShortDateString();
        var authorInits =
            new string(version.Authors.SelectMany(a => a.Name.ComplexInitials).ToArray());
        return $"{date}_{authorInits}";
    }

    private void AddVersion(ContractVersion version)
    {
        _versions.Add(version);
        CurrentVersionId = version.Id;
    }

    private void CreateRevision
    (ModReason modReason, string modDescription, string title, List<Author> authors, DateTime? customDeadline, SpecificationSet specs, bool sameSpecs)
    {
        var baseattribs = VersionAttributeFactory.Create(Id, title, authors, modReason, modDescription);
        ContractVersion revision;
        if (customDeadline == null)
        {
            revision = ContractVersion.CreateRevision(baseattribs, specs, !sameSpecs);
        }
        else
        {
            revision = ContractVersion.CreateRevisionWithCustomDeadline(baseattribs, specs, !sameSpecs, (DateTime)customDeadline);
        }

        AddVersion(revision);
    }

   


}
