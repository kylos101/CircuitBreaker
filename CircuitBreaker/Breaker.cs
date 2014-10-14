using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace CircuitBreaker
{
    //TODO: Make this event based, logic for event changes shouldn't be stored in nested IFs
    public class Breaker
    {        
        private readonly ICircuitBreakerStateStore stateStore; 
        public Breaker(Type sharedResource)
        {            
            stateStore = 
                CircuitBreakerStateStoreFactory
                .GetCircuitBreakerStateStore(sharedResource.ToString());
        }
                
        private readonly object halfOpenToken = new object();        

        public bool IsClosed { get { return stateStore.IsClosed; } }

        public bool IsOpen { get { return !IsClosed; } }

        // TODO: !!! Consider rewrite using Reactive Extensions !!!
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
                this.TrackException(ex);                
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
                // TODO: Catch exceptions that we don't want to trip, handle them differently
                catch (Exception ex)
                {
                    this.TrackException((Exception)ex);
                    throw;
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
            throw new CircuitBreakerOpenException();
        }

        private void TrackException(Exception ex)
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