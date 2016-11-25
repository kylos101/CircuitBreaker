using System;

namespace CircuitBreaker
{
    public abstract class AbstractCircuit : ICircuit
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Description { get; protected set; }
        public TimeSpan TryAgainAfter { get; protected set; } = new TimeSpan(0, 0, 30);


        public AbstractCircuit(TimeSpan? tryAgainAfter)
        {
            if (tryAgainAfter.HasValue)
            {
                TryAgainAfter = tryAgainAfter.Value;
            }
        }
    }
}
