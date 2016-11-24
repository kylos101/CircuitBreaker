using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    /// <summary>
    /// An abstract command, constrained by a circuit breaker
    /// </summary>
    /// <typeparam name="T">Something we want to act on</typeparam>
    public abstract class AbstractCommand<T> : ICommand<T>
    {                
        public AbstractCommand(ICircuit circuit)
        {
            Circuit = circuit;
            Breaker = new Breaker(Circuit);
        }

        // things to help govern the command
        protected Breaker Breaker;
        protected ICircuit Circuit { get; set; }

        // things we might want this command to be able to do
        public abstract Action Action { get; protected set; }
        public abstract Action<T> ActionT { get; protected set; }
        public abstract Task Task { get; protected set; }
        public abstract Task<T> TaskT { get; protected set; }
        public abstract Func<T> FuncT { get; protected set; }      
        
        // move some stuff from the Breaker to here
        // TODO: ExecuteAction 
        // TODO: HandleOpenCircuit 
        // leave stuff on the breaker that manages the breaker...
        // this should hit the circuit
        
        public void ExecuteAction()
        {
            try
            {
                if (this.Breaker.IsClosed)
                {
                    this.Action();
                }
                else if (this.Breaker.IsOpen)
                {
                    if (this.Breaker.OpenCircuitCanBeUsed())
                    {
                        this.Action();
                    }                                      
                }
            }
            catch (Exception ex)
            {
                this.Breaker.HandleException(ex);
                throw new CircuitBreakerOpenException("The circuit is tripped. Refer to the inner exception for the cause of the trip.", ex);
            }                                               
        } 

        public void ExecuteAsyncAction()
        {

        }
    }
}
