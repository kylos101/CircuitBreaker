using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CircuitBreaker;
using NUnit;
using NUnit.Framework;
using System.Threading;

namespace CircuitBreakerUT
{
    [TestFixture]
    public class BreakerTest
    {
        private List<Test> testData = new List<Test>();
        private Breaker sbBreaker;       
        private Action aFailingAction;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            // wire-up an Action delegate, this is designed to throw exceptions...
            this.aFailingAction = this.DoException;

            // wire-up a circuit breaker
            this.sbBreaker = new Breaker(typeof(StringBuilder));

            // verify core functionality before running other tests
            this.VerifyClosedAndOpenBehavior();
        }

        [SetUp]
        public void TestSetUp()
        {           
            // setup some test data   
            testData.Add(new Test() { Id = 1, Desc = "Fu" });
            testData.Add(new Test() { Id = 2, Desc = "bar" });
            Assert.IsTrue(testData.Any(), "Looks like we're lacking test data...");                                    
        }

        [TearDown]
        public void TestTearDown()
        {
            // ditch the test data
            this.testData.RemoveAll(p => p != null);                        
            Assert.IsFalse(testData.Any());
        }
       
        private void VerifyClosedAndOpenBehavior()
        {
            Assert.IsTrue(this.sbBreaker.IsClosed, "The circuit breaker's state should be Closed...it's new!");
            if (this.sbBreaker.IsClosed)
            {
                Console.WriteLine("The breaker is closed...");

                try
                {
                    this.UseTheBreaker();
                }
                catch (CircuitBreakerOpenException ex)
                {
                    Console.WriteLine(ex);
                }                
            }

            Assert.IsTrue(this.sbBreaker.IsOpen, "The circuit breaker's state should be Open.");
            if (this.sbBreaker.IsOpen)
            {
                Console.WriteLine("The breaker is open...");
                try
                {
                    this.UseTheBreaker();
                }
                catch (CircuitBreakerOpenException ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
        
        [Test] 
        [ExpectedException(ExpectedException=typeof(CircuitBreakerOpenException),ExpectedMessage="The circuit was tripped while half-open. Refer to the inner exception for the cause of the trip.")]
        public void VerifyHalfOpenIsAValidState()
        {
            // wait for the breaker to allow action again...
            var timer = new Stopwatch();
            timer.Start();
            while (timer.ElapsedMilliseconds < 31000) { }    
                  
            // the breaker should be half-open now (allow normal use since the initial trip)
            Console.WriteLine("The breaker should be half-open...");
            this.UseTheBreaker(); // do not eat the CircuitBreakerOpenException, we need to validate the thrown exception
        }        

        /// <summary>
        /// This attempts to use the circuit breaker to perform an action.
        /// </summary>
        private void UseTheBreaker()
        {
            Console.WriteLine("Using the breaker...");
            this.sbBreaker.ExecuteAction(aFailingAction);
        }
              
        /// <summary>
        /// This is a junk method designed to cause an exception. Assign this to an action delegate and use a circuit breaker's ExecuteAction method to call this...
        /// </summary>
        private void DoException()
        {                                 
            //Lets pretend string builder is an outside dependency, like a EF context or web service proxy, that threw this exception...;
            throw new Exception("This is a planned exception, deal with it!");
        }   

        [Test]
        public void VerifyNewBreakersInheritExistingState()
        {
            var aThread = new Thread(DoThreadWork); // trip a breaker in a new thread
            var randoBreaker = new Breaker(typeof(SomeRandomClass)); // setup a new breaker, for a breaker that previously tripped         
            Assert.IsTrue(randoBreaker.IsOpen); // verify the new breaker is open, even though it hasn't tripped

            // TODO: This is a behavior test for synchronous order, thread probably doesn't matter here...
        }

        [Test]
        public void VerifyNewBreakerObservesStateChangesFromOtherClients()
        {
            var randoBreaker = new Breaker(typeof(SomeRandomClass)); // setup a new breaker
            var aThread = new Thread(DoThreadWork); // trip a breaker tracking the same type in a different thread      
            Assert.IsTrue(randoBreaker.IsOpen); // verify the breaker is open, even though a client in another thread tripped it
        }

        private void DoThreadWork()
        {
            var randoBreaker = new Breaker(typeof(SomeRandomClass)); // setup a new breaker           
            Assert.IsTrue(randoBreaker.IsClosed, "This random breaker is open and should be closed!");
            try
            {
                randoBreaker.ExecuteAction(this.DoException);
            }
            catch (CircuitBreakerOpenException)
            {
                // nom;
            }
        }
    }
}
