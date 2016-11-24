using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace CircuitBreaker
{        
    public class CircuitBreakerStateStoreFactory
    {                
        private static ConcurrentDictionary<string, ICircuitBreakerStateStore> _stateStores =
            new ConcurrentDictionary<string,ICircuitBreakerStateStore>();       
                        
        internal static ICircuitBreakerStateStore GetCircuitBreakerStateStore(ICircuit circuit)
        {
            // There is only one type of ICircuitBreakerStateStore to return...
            // The ConcurrentDictionary keeps track of ICircuitBreakerStateStore objects (across threads)
            // For example, a store for a db connection, web service client, and NAS storage could exist            

            if (!_stateStores.ContainsKey(circuit.Name))
            {
                _stateStores.TryAdd(circuit.Name, new CircuitBreakerStateStore(circuit)); 
            }

            return _stateStores[circuit.Name];
        }

        // TODO: Add the ability for Circuit breaker stateStores to update the state in this dictionary?        
    }
}