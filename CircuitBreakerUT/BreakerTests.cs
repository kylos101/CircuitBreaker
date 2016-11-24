﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System.Text;
using CircuitBreaker;
using NUnit.Framework;

namespace CircuitBreakerUT
{
    [TestFixture]
    public class BreakerTest
    {               
        [Test]            
        public void CircuitBreakerOpenException_IsThrown_UponException()
        {
            var testCircuit = new TestCircuit(null);
            var testCommand = new TestCommand(testCircuit);

            Assert.IsTrue(testCommand.Breaker.IsClosed);

            Assert.That(() => testCommand.ExecuteAction,
                Throws.Exception
                .TypeOf<CircuitBreakerOpenException>()
                );
        }

        [Test]
        public void NewBreaker_OnIsolatedCircuit_IsClosed()
        {
            var aCircuit = new AnotherTestCircuit(null);
            var aCommand = new TestCommand(aCircuit);
            Assert.IsTrue(aCommand.Breaker.IsClosed);
        }
    }
}
