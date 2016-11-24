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

        public override Action Action { get; protected set; }

        /// <summary>
        /// This is a junk method designed to cause an exception. Assign this to an action delegate and use a circuit breaker's ExecuteAction method to call this...
        /// </summary>
        private void DoSomethingAndFail()
        {
            //Lets pretend string builder is an outside dependency, like a EF context or web service proxy, that threw this exception...;
            throw new Exception("This is a planned exception, deal with it!");
        }

        public override Action<SomeTest> ActionT
        {
            get
            {
                throw new NotImplementedException();
            }

            protected set
            {
                throw new NotImplementedException();
            }
        }

        public override Func<SomeTest> FuncT
        {
            get
            {
                throw new NotImplementedException();
            }
            protected set
            {
                throw new NotImplementedException();
            }
        }

        public override Task Task
        {
            get
            {
                throw new NotImplementedException();
            }

            protected set
            {
                throw new NotImplementedException();
            }
        }

        public override Task<SomeTest> TaskT
        {
            get
            {
                throw new NotImplementedException();
            }

            protected set
            {
                throw new NotImplementedException();
            }
        }
    }
}
