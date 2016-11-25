using System;

namespace CircuitBreaker
{
    /// <summary>
    /// Something ICommands hold in common, like a database or HTTP connection
    /// </summary>
    public interface ICircuit
    {
        Guid Id { get; }
        string Description { get; }
        TimeSpan TryAgainAfter { get; }
    }
}