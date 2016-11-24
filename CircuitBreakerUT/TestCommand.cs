using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CircuitBreaker;

namespace CircuitBreakerUT
{
    public class TestCommand : AbstractCommand<SomeTest>
    {
        public TestCommand(ICircuit circuit) : base(circuit)
        {
            this.Action = DoSomethingAndFail;
        }
        
        /// <summary>
        /// An action for this TestCommand
        /// </summary>
        protected override Action Action { get; set; }

        /// <summary>
        /// This is a junk method designed to cause an exception.         
        /// </summary>
        private void DoSomethingAndFail()
        {            
            throw new Exception("This is a planned exception, deal with it!");
        }

    }
}
