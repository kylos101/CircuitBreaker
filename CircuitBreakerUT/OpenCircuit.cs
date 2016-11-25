using CircuitBreaker;
using System;

namespace CircuitBreakerUT
{
    public class OpenCircuit : AbstractCircuit
    {
        public OpenCircuit(TimeSpan? timespan) : base(timespan)
        {
            base.Description = "Test...";
        }
    }
}
