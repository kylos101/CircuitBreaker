using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    /// <summary>
    /// Model from https://msdn.microsoft.com/en-us/library/dd990377%28v=vs.110%29.aspx
    /// </summary>
    public class BreakerObservable : IObservable<Breaker>
    {
        private IList<IObserver<Breaker>> observers;

        public BreakerObservable()
        {
            observers = new List<IObserver<Breaker>>();
        }

        /// <summary>
        /// Register a subscriber with a publisher
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<Breaker> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }

            return new Unsubscriber(observers, observer);
        }

        /// <summary>
        /// Share data with other subscribers
        /// </summary>
        /// <param name="stateStore"></param>
        public void TrackBreaker(Nullable<Breaker> stateStore)
        {
            foreach (var observer in observers)
            {
                if (!stateStore.HasValue)
                {
                    throw new BreakerUnknownException();
                }
                else
                {
                    observer.OnNext(stateStore.Value);
                }
            }
        }

        /// <summary>
        /// Notify observers that transmission has finished
        /// </summary>
        public void EndTransmission()
        {
            foreach(var observer in observers.ToArray())
            {
                if (observers.Contains(observer))
                {
                    observer.OnCompleted();
                }

                observers.Clear();
            }
        }

        private class Unsubscriber : IDisposable
        {
            private IList<IObserver<Breaker>> _observers;
            private IObserver<Breaker> _observer;

            public Unsubscriber(IList<IObserver<Breaker>> observers, IObserver<Breaker> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                {
                    _observers.Remove(_observer);   
                }
            }
        }
    }
}
