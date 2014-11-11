* The command tracked by circuit breaker needs to be more flexible (take more than an Action delegate). 
	-> Look at what Netflix did with Async & Reactive Extensions and their Circuit Breaker. 
	* Rx started with Microsoft, or so I read/heard.

* Threading tests...
We need a way to update the appropriate ICircuitBreakerStateStore with new state information...I'm thinking another method
on CircuitBreakerStateStoreFactory makes sense...this would be called by from StateStore when State changes occur. 
	-> this is likely where the Observable pattern would be helpful. 

* Consider making it so that methods wrapped by the circuit breaker must be members of the Type fed to the constructor. 
An alternative to this might be to make it so that what you feed to the Breaker is has extension methods that implement a contract...
and then it's up to the user to use extensions that are async, or the normal synchronous stuff. 