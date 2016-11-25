using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CircuitBreaker;

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
