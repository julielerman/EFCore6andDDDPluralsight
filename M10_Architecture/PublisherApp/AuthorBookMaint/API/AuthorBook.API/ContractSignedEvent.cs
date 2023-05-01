using PublisherSystem.SharedKernel;
using PublisherSystem.SharedKernel.DTOs;


namespace AuthorBook.API;

public class ContractSignedEventMessage : BaseDomainEvent
{

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ContractId { get; set; }
    public List<AuthorDTO> Authors { get; set; } = new List<AuthorDTO>();
    public string Title { get; set; }
    public DateTime SignedDate { get; set; }




}
