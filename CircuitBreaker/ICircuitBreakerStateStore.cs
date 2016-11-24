using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CircuitBreaker
{
    // reference: http://msdn.microsoft.com/en-us/library/dn589784.aspx
    public interface ICircuitBreakerStateStore
    {
        string Name { get; }
        CircuitBreakerStateEnum State { get; }
        Exception LastException { get; }
        DateTime? LastStateChangeDateUtc { get; }
        void Trip(Exception ex);
        void Reset();
        void HalfOpen();
        bool IsClosed { get; }
        bool IsHalfOpen { get; }
    }    
}