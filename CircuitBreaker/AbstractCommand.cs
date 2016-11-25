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

        /// <summary>
        /// The breaker
        /// </summary>
        public readonly Breaker Breaker;

        /// <summary>
        /// The circuit
        /// </summary>
        protected readonly ICircuit Circuit;

        /// <summary>
        /// The method we want to try executing
        /// </summary>
        protected abstract Action Action { get; set; }

        /// <summary>
        /// A method we want to fire.
        /// Is executed, depending on the status of the breaker. 
        /// </summary>
        public async Task<CommandResult> ExecuteAction()
        {
            try
            {
                if (this.Breaker.IsClosed)
                {
                    await Task.Run(() => this.Action());
                    return CommandResult.Succeeded;
                }

                if (this.Breaker.IsOpen)
                {
                    await Task.Run(() => this.Breaker.TryHalfOpen());
                }

                if (this.Breaker.IsHalfOpen)
                {
                    await Task.Run(() => this.Action());
                    await Task.Run(() => this.Breaker.Close());
                    return CommandResult.Succeeded;
                }
            }
            catch (Exception ex)
            {
                await Task.Run(() => this.Breaker.Trip(ex));
                throw new CircuitBreakerOpenException("The circuit has been tripped. Refer to the inner exception for the cause of the trip.", ex);
            }

            return CommandResult.Failed;

        } 

        public void ExecuteAsyncAction()
        {
            throw new NotImplementedException();
        }
    }
}
