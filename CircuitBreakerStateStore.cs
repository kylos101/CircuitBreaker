using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CircuitBreaker
{
    public class CircuitBreakerStateStore : ICircuitBreakerStateStore
    {
        private ConcurrentStack<Exception> _exceptionsSinceLastStateChange;        
        public CircuitBreakerStateStore() 
        {
            _exceptionsSinceLastStateChange = new ConcurrentStack<Exception>();            
        }

        private CircuitBreakerStateEnum _State;
        public CircuitBreakerStateEnum State
        {                        
            get 
            {         
                if (this._State.Equals(CircuitBreakerStateEnum.None))
                {
                    this._State = CircuitBreakerStateEnum.Closed;                    
                }
                return this._State;
            }
            private set
            {
                this._State = value;
            }
        }

        public Exception LastException
        {
            get
            {
                Exception lastException = null;
                _exceptionsSinceLastStateChange.TryPeek(out lastException);
                return lastException;
            }
        }

        public DateTime? LastStateChangeDateUtc
        {
            get 
            {                
                return this.LastStateChangeDateUtc;
            }
            private set
            {
                this.LastStateChangeDateUtc = value;
            }
        }

        public void Trip(Exception ex)
        {
            this.ChangeState(CircuitBreakerStateEnum.Open);
            _exceptionsSinceLastStateChange.Push(ex);
            //TODO: Persist the failure, too.
        }

        public void Reset()
        {
            this.ChangeState(CircuitBreakerStateEnum.Closed);
            _exceptionsSinceLastStateChange.Clear();
            //TODO: Persist the reset.
        }

        public void HalfOpen()
        {
            this.ChangeState(CircuitBreakerStateEnum.HalfOpen);
            //TODO: Persist the halfOpen, I assume this is a degraded mode of performance
        }

        public bool IsClosed
        {
            get 
            { 
                return this.State.Equals(CircuitBreakerStateEnum.Closed);                
            }
        }

        public void ChangeState(CircuitBreakerStateEnum state)
        {
            this.State = state;
            this.LastStateChangeDateUtc = DateTime.UtcNow;
        }
    }
}