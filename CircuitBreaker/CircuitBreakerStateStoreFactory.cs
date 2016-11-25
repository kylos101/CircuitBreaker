using System;
using System.Collections.Concurrent;

namespace CircuitBreaker
{
    public class CircuitBreakerStateStoreFactory
    {
        private static ConcurrentDictionary<Type, ICircuitBreakerStateStore> _stateStores =
            new ConcurrentDictionary<Type, ICircuitBreakerStateStore>();

        internal static ICircuitBreakerStateStore GetCircuitBreakerStateStore(ICircuit circuit)
        {
            // There is only one type of ICircuitBreakerStateStore to return...
            // The ConcurrentDictionary keeps track of ICircuitBreakerStateStore objects (across threads)
            // For example, a store for a db connection, web service client, and NAS storage could exist            

            if (!_stateStores.ContainsKey(circuit.GetType()))
            {
                _stateStores.TryAdd(circuit.GetType(), new CircuitBreakerStateStore(circuit));
            }

            return _stateStores[circuit.GetType()];
        }

        // TODO: Add the ability for Circuit breaker stateStores to update the state in this dictionary?        
    }
}