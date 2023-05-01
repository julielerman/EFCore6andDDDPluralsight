using System;
using System.Collections.Generic;

namespace PublisherSystem.SharedKernel
{
  /// <summary>
  /// Base types for all Entities which track state using a given Id.
  /// </summary>
  public abstract class BaseEntity<TId>
  {
    public TId Id { get;  protected set; }
    public List<BaseDomainEvent> Events = new List<BaseDomainEvent>();
  }
}
