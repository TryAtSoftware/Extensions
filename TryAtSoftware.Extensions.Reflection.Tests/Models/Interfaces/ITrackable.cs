namespace TryAtSoftware.Extensions.Reflection.Tests.Models.Interfaces;

using System;

public interface ITrackable : IIdentifiable
{
    Guid CreatedBy { get; }
    DateTimeOffset CreatedAt { get; }
    
    Guid LastModifiedBy { get; }
    DateTimeOffset LastModifiedAt { get; }
}