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
        
        public CircuitBreakerStateStore(ICircuit circuit)
        {
            this._exceptionsSinceLastStateChange = new ConcurrentStack<Exception>();            
            this.Name = circuit.Name;
        }

        public string Name {get; private set;}        

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

        private DateTime? _lastStateChangeDateUtc;
        public DateTime? LastStateChangeDateUtc
        {
            get 
            {                
                return this._lastStateChangeDateUtc;
            }
            private set
            {
                this._lastStateChangeDateUtc = value;
            }
        }

        public void Trip(Exception ex)
        {
            this.ChangeState(CircuitBreakerStateEnum.Open);
            _exceptionsSinceLastStateChange.Push(ex);            
        }

        public void Reset()
        {
            this.ChangeState(CircuitBreakerStateEnum.Closed);
            _exceptionsSinceLastStateChange.Clear();            
        }

        public void HalfOpen()
        {
            this.ChangeState(CircuitBreakerStateEnum.HalfOpen);            
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