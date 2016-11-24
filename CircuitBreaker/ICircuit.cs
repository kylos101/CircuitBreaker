using System;

namespace CircuitBreaker
{
    public interface ICircuit
    {
        Guid Id { get; }
        string Name { get; }
        TimeSpan TryAgainAfter { get; }
    }
}