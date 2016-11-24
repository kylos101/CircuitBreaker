using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace CircuitBreaker
{        
    /// <summary>
    /// A generic circuit breaker. Tracks whether a type's state is usable (closed), malfunctioning (open), or recovering (half-open).
    /// </summary>
    public class Breaker
    {
        /// <summary>
        /// A static, thread safe collection that tracks the status of circuits
        /// </summary>
        private readonly ICircuitBreakerStateStore stateStore;

        /// <summary>
        /// The circuit for this breaker
        /// </summary>
        private readonly ICircuit Circuit;
                
        /// <summary>
        /// A breaker that will govern an ICircuit
        /// </summary>
        /// <param name="circuit"></param>        
        public Breaker(ICircuit circuit)
        {
            Circuit = circuit;

            stateStore = 
                CircuitBreakerStateStoreFactory
                .GetCircuitBreakerStateStore(Circuit);
        }

        /// <summary>
        /// An object to assist with inspecting locks
        /// </summary>
        private readonly object halfOpenToken = new object();

        /// <summary>
        /// Is the circuit closed
        /// </summary>
        public bool IsClosed { get { return stateStore.IsClosed; } }

        /// <summary>
        /// Is the circuit open
        /// </summary>
        public bool IsOpen { get { return !IsClosed; } }

        /// <summary>
        /// Is the circuit half open
        /// </summary>
        public bool IsHalfOpen { get { return stateStore.IsHalfOpen; } }
        
        /// <summary>
        /// Begin to try closing an open circuit
        /// </summary>
        public void TryHalfOpen()
        {            
            if (IsHalfOpenReady)
            {
                bool lockTaken = false;
                try
                {
                    Monitor.TryEnter(halfOpenToken, ref lockTaken);
                    if (lockTaken)
                    {
                        this.stateStore.HalfOpen();                        
                    }
                }                                
                finally
                {
                    if (lockTaken)
                    {
                        Monitor.Exit(halfOpenToken);
                    }
                }
            }            
        }     

        /// <summary>
        /// Some action failed, trip the breaker
        /// </summary>
        /// <param name="ex"></param>
        public void Trip(Exception ex)
        {
            this.stateStore.Trip(ex);            
        }

        /// <summary>
        /// We protect the resource until it is considered halfopen.
        /// This can be protected with a timeout, or set via some other means.
        /// </summary>
        private bool IsHalfOpenReady
        {
            get
            {                             
                return stateStore.LastStateChangeDateUtc + Circuit.TryAgainAfter < DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Close the breaker
        /// </summary>
        public void Close()
        {
            this.stateStore.Reset();            
        }

        /// <summary>
        /// The last exception for this circuit
        /// </summary>
        public Exception LastException
        {
            get
            {
                return this.stateStore.LastException;
            }
        }

        
    }
}