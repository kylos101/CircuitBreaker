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

namespace CircuitBreakerUT
{
    [TestFixture]
    public class BreakerTest
    {
        private List<Test> testData = new List<Test>();
        private Breaker sbBreaker; 
        private StringBuilder sb = new StringBuilder();
        private Action aFailingAction;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            // wire-up an Action delegate, this is designed to throw exceptions...
            this.aFailingAction = this.DoSomethingAndFail;

            // wire-up a circuit breaker
            this.sbBreaker = new Breaker(typeof(StringBuilder));        
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
        
        [Test]
        /// <summary>
        /// Results vary.
        /// 
        /// For example, if you run this a second time and < 30 seconds after the first time, the breaker will still be open and skip the closed part of this test.        
        /// </summary>         
        public void VerifyBasicCircuitBehavior()
        {
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

                Assert.IsTrue(this.sbBreaker.IsOpen, "The circuit breaker's state should be Open.");
            }  
            
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

            // wait for the breaker to allow action again...
            var timer = new Stopwatch();
            timer.Start();
            while (timer.ElapsedMilliseconds < 31000) { }            

            // the breaker should be ready now
            Console.WriteLine("The breaker should be half-open...");
            try
            {
                this.UseTheBreaker();
            }
            catch (CircuitBreakerOpenException ex)
            {
                Console.WriteLine(ex);
            }            
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
        private void DoSomethingAndFail()
        {                                 
            //Lets pretend string builder is an outside dependency, like a EF context or web service proxy, that threw this exception...;
            throw new Exception("This is a planned exception, deal with it!");
        }   
     
        //[Test]
        public void ThreadTest()
        {
            //TODO: Write a multi-threaded test.
            // Open a thread
            // Get an exception on the first thread
            // Open a second thread
            // Get an exception on the second thread        

            // BackgroundWorker
            var worker = new BackgroundWorker();
            
        }

    }
}
