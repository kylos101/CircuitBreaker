﻿using CircuitBreaker;
using System;

namespace CircuitBreakerUT
{
    /// <summary>
    /// A command with an action that always fails
    /// </summary>
    public class TestExceptionCommand : AbstractCommand<SomeTest>
    {
        public TestExceptionCommand(ICircuit circuit) : base(circuit)
        {
            this.Action = DoSomethingAndFail;
        }

        /// <summary>
        /// An action for this TestExceptionCommand
        /// </summary>
        protected override Action Action { get; set; }

        /// <summary>
        /// Fake a failure...this should "trip" the breaker's circuit
        /// </summary>
        private void DoSomethingAndFail()
        {
            throw new Exception("This is a planned exception, deal with it!");
        }

    }
}
