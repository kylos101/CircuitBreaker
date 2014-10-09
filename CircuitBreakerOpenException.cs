using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CircuitBreaker
{
    public class CircuitBreakerOpenException : Exception
    {
        public CircuitBreakerOpenException() {}
    }
}
