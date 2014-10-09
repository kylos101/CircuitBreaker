using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CircuitBreaker
{
    public enum CircuitBreakerStateEnum
    {
        None=0,
        Open=1,
        Closed=2,
        HalfOpen=3
    }
}
