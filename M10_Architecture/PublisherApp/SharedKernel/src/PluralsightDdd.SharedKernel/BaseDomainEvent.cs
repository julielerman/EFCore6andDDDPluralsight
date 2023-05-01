using MediatR;

namespace PublisherSystem.SharedKernel
{
  public abstract class BaseDomainEvent : INotification
  {
    public DateTimeOffset DateOccurred { get; protected set; } = DateTimeOffset.UtcNow;
  }
}
