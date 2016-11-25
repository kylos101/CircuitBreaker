using CircuitBreaker;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBreakerUT
{
    [TestFixture]
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
