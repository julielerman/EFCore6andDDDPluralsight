namespace PublisherSystem.SharedKernel.DTOs;

public class NewContractObject
{
    public Guid ContractId { get; set; }
    public List<AuthorDTO> Authors { get; set; }=new List<AuthorDTO>();
    public string BookTitle { get; set; }
    public DateTime ContractedDate { get; set; }

}
