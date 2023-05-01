namespace PublisherSystem.SharedKernel.DTOs;

public class GuidKeyAndDescription
{
    public GuidKeyAndDescription(Guid key, string description)
    {
        KeyValue = key;
        Description = description;
    }
    private GuidKeyAndDescription()
    {
        
    }
    public Guid KeyValue { get; private set; }
    public string? Description { get; private set; }
}