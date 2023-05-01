
using ContractBC.Events;
using ContractBC.Interfaces;
using MediatR;

namespace ContractBC.Handlers;

    /// <summary>
    /// Post ContractSigned to message bus/queue to allow new authors and books to be added to Maint BC
    /// </summary>
    public class RelayContractSignedHandler : INotificationHandler<ContractSignedEvent>
    {
    private IMessagePublisher _messagePublisher;

    public RelayContractSignedHandler(
           IMessagePublisher messagePublisher)
        {
          
            _messagePublisher = messagePublisher;
        
        }

        public async Task Handle(ContractSignedEvent contractSignedEvent,
          CancellationToken cancellationToken)
        {
               _messagePublisher.Publish(contractSignedEvent);
           
        }
    }

