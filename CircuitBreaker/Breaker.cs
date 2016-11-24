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
        /// A breaker that will govern an ICommand, given the status of the ICircuit
        /// </summary>
        /// <param name="circuit"></param>        
        public Breaker(ICircuit circuit)
        {
            Circuit = circuit;

            stateStore = 
                CircuitBreakerStateStoreFactory
                .GetCircuitBreakerStateStore(Circuit);
        }
                
        private readonly object halfOpenToken = new object();        

        public bool IsClosed { get { return stateStore.IsClosed; } }

        public bool IsOpen { get { return !IsClosed; } }
        
        public void ExecuteAction(Action action)
        {                        
            // open circuits have existing faults, handle them
            if (IsOpen)
            {
                HandleOpenCircuit(action);
                return;
            }

            // The circuit breaker is Closed, execute the action.
            try
            {
                action();
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
                throw new CircuitBreakerOpenException("The circuit was just tripped. Refer to the inner exception for the cause of the trip.", ex);
            }
        }

        public bool OpenCircuitCanBeUsed()
        {
            // protect what is behind the circuit, do not take "action" until it is halfOpen
            if (IsHalfOpenReady)
            {
                bool lockTaken = false;
                try
                {
                    Monitor.TryEnter(halfOpenToken, ref lockTaken);
                    if (lockTaken)
                    {
                        this.stateStore.HalfOpen();
                        action();
                        this.stateStore.Reset();
                        return;
                    }
                }                
                catch (Exception ex)
                {
                    this.HandleException(ex);
                    throw new CircuitBreakerOpenException("The circuit was tripped while half-open. Refer to the inner exception for the cause of the trip.", ex);
                }
                finally
                {
                    if (lockTaken)
                    {
                        Monitor.Exit(halfOpenToken);
                    }
                }
            }
            // if we get here, the circuit was still open...            
            throw new CircuitBreakerOpenException("The circuit is still open. Refer to the inner exception for the cause of the circuit trip.",this.stateStore.LastException);
        }     

        public void HandleException(Exception ex)
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
    }
}