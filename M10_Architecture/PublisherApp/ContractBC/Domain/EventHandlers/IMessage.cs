using ContractBC.Events;
namespace ContractBC.Interfaces
{
    public interface IMessagePublisher
    {
         void Publish(ContractSignedEvent eventToPublish);
    }
}