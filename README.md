CircuitBreaker
==============
[![Build status](https://ci.appveyor.com/api/projects/status/24xnocra9wc8hl00/branch/master?svg=true)](https://ci.appveyor.com/project/kylos101/circuitbreaker/branch/master)

A circuit breaker class library that uses a command pattern.

[I started here...](http://msdn.microsoft.com/en-us/library/dn589784.aspx). As you can see, it was heavily borrowed.

## How to use it

### Define a circuit

This represents something you want to protect.

```csharp
    /// <summary>
    /// My database's circuit
    /// </summary>
    public class MyDatabaseCircuit : AbstractCircuit
    {
        public MyDatabaseCircuit(TimeSpan? timespan) : base(timespan)
        {
            base.Description = "Some fake database I use...";
        }
    }
```

### Setup a command

This represents something you want to do.

```csharp
    /// <summary>
    /// A command with an action that always works...
    /// </summary>
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
```

### Try using your command

It'll throw CircuitBreakerOpenException if there's trouble.

Refer to the inner exception for the actual exception.

```csharp

var testCircuit = new MyDatabaseCircuit(null);
var testCommand = new TestCommand(testCiruit);

try
{
    var result = testCommand.ExecuteAction().Result;
}
catch (AggregateException ex)
{
    ex.Handle((x) =>
    {        
        if (x is CircuitBreakerOpenException)
        {
            // do something because the circuit is open / tripped / down
            return true; // don't throw, we're handling the exception
        }
        return false; // throw, we didn't encounter the expected exception
    });
}
```
