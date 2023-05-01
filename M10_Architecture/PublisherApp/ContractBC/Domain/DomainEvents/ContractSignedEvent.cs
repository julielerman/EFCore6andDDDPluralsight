using ContractBC.ContractAggregate;
using PublisherSystem.SharedKernel;
using PublisherSystem.SharedKernel.DTOs;

namespace ContractBC.Events;

public class ContractSignedEvent : BaseDomainEvent
{
    public ContractSignedEvent(ContractVersion signedVersion, DateTime completionDate)
    {
        ContractId = signedVersion.ContractId;
        Authors = signedVersion.Authors
            .Select(a => new AuthorDTO(a.Name.FirstName, a.Name.LastName,
                                       a.Email, a.Phone, a.Signed, a.SignedAuthorId))
            .ToList();
        Title = signedVersion.WorkingTitle;
        SignedDate = completionDate;
    }
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ContractId { get; set; }
    public List<AuthorDTO> Authors { get; set; } = new List<AuthorDTO>();
    public string Title { get; set; }
    public DateTime SignedDate { get; set; }
}
