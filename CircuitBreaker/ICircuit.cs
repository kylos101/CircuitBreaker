using System;

namespace CircuitBreaker
{
    /// <summary>
    /// Something many ICommand objects hold in common, like a database or HTTP connection
    /// </summary>
    public interface ICircuit
    {
        /// <summary>
        /// An id for this circuit
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// A description of what this circuit is for
        /// </summary>
        string Description { get; }

        /// <summary>
        /// How long we must wait before trying to close this circuit
        /// </summary>
        TimeSpan TryAgainAfter { get; }
    }
}