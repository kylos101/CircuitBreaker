Overview
* Circuitbreaker pattern - temporarily avoid making calls that use external systems (a web service, a database) due to unexpcted issues

Background
* I'd like to write and deploy some front end and web service code to Azure, but need to be mindful of cost...the
circuit breaker may help me from spinning out of control cost-wise.
* It's all a learning opportunity...

Goals
* Get stuff out to Nuget and start participating in the developer community where applicable
* github.com/kylos101 
* nuget.org/profiles/kylos101
* I'm disconnected now, otherwise I'd show you my sweet profile picture

Platform overview
* Visual studio 2013
* Github.com & Windows Github client (Explorer and VS integration)
* Nunit
* Nuget

* Why?
	* I heard in a podcast that Netflix designs with fault tolerance in mind. 
	* They do this by wrapping commands for external resources using a custom library, Hystrix.

* Hystrix is written in Java, I decided, heck! Let's write it in C#!
	* That was way to much code 
	* Decided to target a smaller goal, and learn about the circuit breaker pattern (which Hystrix uses)

* code 
	* Breaker ctor -> Factory
	* Breaker -> IStateStore & StateStore 
	* Breaker -> Exception
	* unit tests

###############################

Considerations for the future
* The command isolated by circuit breaker needs to be more flexible (take more than just an Action delegate as the ctor parameter). 
	* Look at what Netflix did with Async & Reactive Extensions and their Circuit Breaker & their "HystrixCommand" class. 
	* I recall their breaker accepting a "HytrixCommand", which used ReactiveExtensions to do various async things

* Need more tests (async, threading, etc.)

* Provide a means to update state information for ICircuitBreakerStateStores
	* I'm thinking another method on CircuitBreakerStateStoreFactory makes sense...this would be called by from StateStore when State changes occur. 
	* this is likely where the Observable pattern would be helpful. 