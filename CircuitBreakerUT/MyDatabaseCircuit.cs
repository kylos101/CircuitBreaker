using CircuitBreaker;
using System;

namespace CircuitBreakerUT
{
    /// <summary>
    /// My database's circuit
    /// </summary>
    public class MyDatabaseCircuit : AbstractCircuit
    {
        public MyDatabaseCircuit(TimeSpan? timespan) : base(timespan)
        {
            base.Description = "Some fake database I use...";
        }
    }
}
