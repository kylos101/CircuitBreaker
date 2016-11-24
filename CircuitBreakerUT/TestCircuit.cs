using CircuitBreaker;
using System;

namespace CircuitBreakerUT
{
    public class TestCircuit : AbstractCircuit
    {
        public TestCircuit(TimeSpan timespan) : base(timespan)
        {
            base.Name = "Test...";
        }
    }
}
