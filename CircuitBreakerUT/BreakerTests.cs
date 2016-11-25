using CircuitBreaker;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace CircuitBreakerUT
{
    [TestFixture]
    public class BreakerTest
    {
        [Test]
        public void CircuitBreakerOpenException_IsThrown_AfterExecuteActionThrows()
        {
            var testCircuit = new MyWebServerCircuit(null);
            var testCommand = new TestExceptionCommand(testCircuit);

            Assert.IsTrue(testCommand.Breaker.IsClosed);

            try
            {
                var result = testCommand.ExecuteAction().Result;
            }
            catch (AggregateException ex)
            {
                ex.Handle((x) =>
                {
                    Assert.IsTrue(x is CircuitBreakerOpenException);
                    if (x is CircuitBreakerOpenException)
                    {
                        return true; // don't throw, we'll handle the exception
                    }
                    return false; // throw, we didn't encounter the expected exception
                });
            }

            Assert.IsTrue(testCommand.Breaker.IsOpen);
        }

        [Test]
        public void Breaker_IsClosed_OnCircuitWithoutException()
        {
            var aCircuit = new MyDatabaseCircuit(null);
            var aCommand = new TestExceptionCommand(aCircuit);
            Assert.IsTrue(aCommand.Breaker.IsClosed);
        }

        [Test]
        public void Breaker_IsClosed_AfterSuccessfulExecuteAction()
        {
            var aCircuit = new MyDatabaseCircuit(null);
            var aCommand = new TestCommand(aCircuit);
            var result = Task.Run(() => aCommand.ExecuteAction());

            Assert.IsTrue(CommandResult.Succeeded == result.Result);
            Assert.IsTrue(aCommand.Breaker.IsClosed);
        }
    }
}
