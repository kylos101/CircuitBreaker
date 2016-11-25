using CircuitBreaker;
using System;

namespace CircuitBreakerUT
{
    /// <summary>
    /// My web server's circuit
    /// </summary>
    public class MyWebServerCircuit : AbstractCircuit
    {
        public MyWebServerCircuit(TimeSpan? timespan) : base(timespan)
        {
            base.Description = "Test...";
        }
    }
}
