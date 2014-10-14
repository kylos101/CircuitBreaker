using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CircuitBreaker;
using NUnit;
using NUnit.Framework;

namespace CircuitBreakerUT
{
    [TestFixture]
    public class BreakerTest
    {
        private List<SomeTest> someTestList = new List<SomeTest>();
        private Breaker sbBreaker; 
        private StringBuilder sb = new StringBuilder();

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            this.sbBreaker = new Breaker(typeof(StringBuilder));
            someTestList.Add(new SomeTest() { Id = 1, Desc = "Fu" });
            someTestList.Add(new SomeTest() { Id = 2, Desc = "bar" });
            Assert.IsTrue(someTestList.Any(), "Looks like we're lacking test data...");
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            this.someTestList = null;
            this.sbBreaker = null;
        }
        
        [Test]
        public void VerifyBreakerOpens()
        {                
            Action aFailingAction = this.DoSomethingAndFail;            

            if (this.sbBreaker.IsClosed)
            {
                Console.WriteLine("The breaker is closed.");                               
                this.sbBreaker.ExecuteAction(aFailingAction);                                                
                Assert.IsTrue(this.sbBreaker.IsOpen,"The breaker should be open, but is not...");                        
            }          
        }

        [Test]
        public void VerifyOpenBreakerIgnoresCallToAction()
        {
            Action aFailingAction = this.DoSomethingAndFail;

            if (this.sbBreaker.IsClosed)
            {
                Console.WriteLine("The breaker is closed.");
                this.sbBreaker.ExecuteAction(aFailingAction);
                Assert.IsTrue(this.sbBreaker.IsOpen, "The breaker should be open, but is not...");
                // TODO: Don't we want the open breaker exception to fire with the inner exception?
            }

            if (this.sbBreaker.IsOpen)
            {
                // TODO: Code that verifies we throw 
                this.sbBreaker.ExecuteAction(aFailingAction);                                
            }

            // TODO: Do we need to do something for Half Open breakers?
            //if (!this.sbBreaker.IsClosed && !this.sbBreaker.IsOpen)
            //{
            //    throw new Exception("This was not planned, something has gone wrong...or its half open. ;-)");
            //}
        }

        private void DoSomethingAndFail()
        {
            Console.WriteLine("Let's pretend this void on string builder is an outside dependency, like a EF context or web service proxy.");
            throw new NotImplementedException();
        }        
    }
}
