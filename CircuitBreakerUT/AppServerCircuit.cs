using CircuitBreaker;
using System;

namespace CircuitBreakerUT
{
    public class AppServerCircuit : AbstractCircuit
    {
        public AppServerCircuit(TimeSpan? timespan) : base(timespan)
        {
            base.Description = "Test...";
        }
    }
}
