using CircuitBreaker;
using System;
using System.Diagnostics;

namespace CircuitBreakerUT
{
    /// <summary>
    /// A command with an action that always works...
    /// </summary>
    public class TestCommand : AbstractCommand<SomeTest>
    {
        public TestCommand(ICircuit circuit) : base(circuit)
        {
            this.Action = DoSomething;
        }

        /// <summary>
        /// An action for this TestCommand
        /// </summary>
        protected override Action Action { get; set; }

        /// <summary>
        /// This should "succeed" assuming the breaker's circuit is not "tripped"
        /// </summary>
        private void DoSomething()
        {
            Debug.WriteLine("We did something!");
        }
    }
}
