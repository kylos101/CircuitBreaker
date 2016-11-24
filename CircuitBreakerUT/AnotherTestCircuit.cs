using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CircuitBreaker;

namespace CircuitBreakerUT
{
    public class AnotherTestCircuit : AbstractCircuit
    {
        public AnotherTestCircuit(TimeSpan? timespan) : base(timespan)
        {
            base.Name = "AnotherTest";
        }
    }
}
