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
        private readonly ICircuitBreakerStateStore stateStore; 
        
        
        /// <summary>
        /// The type you feed to this constructor will be allocated to a thread-safe, static collection, where the corresponding type's circuit state will be managed.
        /// </summary>
        /// <param name="sharedResource"></param>
        /// TODO: I'm not crazy about feeding the breaker a type. It "might" be better to feed it a method, and then derive the method's class/type.
        public Breaker(Type sharedResource)
        {            
            stateStore =
                CircuitBreakerStateStoreFactory
                .GetCircuitBreakerStateStore(sharedResource.ToString());
        }
                
        private readonly object halfOpenToken = new object();        

        public bool IsClosed { get { return stateStore.IsClosed; } }

        public bool IsOpen { get { return !IsClosed; } }

        // TODO: !!! Consider including Reactive Extensions !!!
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

        private void HandleOpenCircuit(Action action)
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

        private void HandleException(Exception ex)
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
                // TODO we're going to need a way to configure this on a per-class basis
                TimeSpan OpenToHalfOpenWaitTime = new TimeSpan(0, 0, 30);
                return stateStore.LastStateChangeDateUtc + OpenToHalfOpenWaitTime < DateTime.UtcNow;
            }
        }
    }
}