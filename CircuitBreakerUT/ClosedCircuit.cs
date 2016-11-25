using CircuitBreaker;
using System;

namespace CircuitBreakerUT
{
    public class ClosedCircuit : AbstractCircuit
    {
        public ClosedCircuit(TimeSpan? timespan) : base(timespan)
        {
            base.Description = "Some fake database I use...";
        }
    }
}
