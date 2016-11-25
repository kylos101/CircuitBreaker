using CircuitBreaker;
using NUnit.Framework;
using System.Threading.Tasks;

namespace CircuitBreakerUT
{
    [TestFixture]
    public class BreakerTest
    {
        [Test]
        public void CircuitBreakerOpenException_IsThrown_AfterExecuteActionThrows()
        {
            var testCircuit = new OpenCircuit(null);
            var testCommand = new TestExceptionCommand(testCircuit);

            Assert.IsTrue(testCommand.Breaker.IsClosed);

            Assert.That(() => testCommand.ExecuteAction(),
                Throws.Exception
                .TypeOf<CircuitBreakerOpenException>()
                );

            Assert.IsTrue(testCommand.Breaker.IsOpen);
        }

        [Test]
        public void Breaker_IsClosed_OnCircuitWithoutException()
        {
            var aCircuit = new ClosedCircuit(null);
            var aCommand = new TestExceptionCommand(aCircuit);
            Assert.IsTrue(aCommand.Breaker.IsClosed);
        }

        [Test]
        public void Breaker_IsClosed_AfterSuccessfulExecuteAction()
        {
            var aCircuit = new ClosedCircuit(null);
            var aCommand = new TestCommand(aCircuit);
            var result = Task.Run(() => aCommand.ExecuteAction());

            Assert.IsTrue(CommandResult.Succeeded == result.Result);
            Assert.IsTrue(aCommand.Breaker.IsClosed);
        }
    }
}
