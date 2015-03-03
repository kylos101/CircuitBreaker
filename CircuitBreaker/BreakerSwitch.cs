using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    public class BreakerSwitch : IObserver<Breaker>
    {
        private IDisposable unsubscriber;
        private string instanceName;

        public BreakerSwitch(string name)
        {
            this.instanceName = name;
        }

        public virtual void Subscribe(IObservable<Breaker> provider)
        {
            if (provider != null)
            {
                unsubscriber = provider.Subscribe(this);
            }
        }
        
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(Breaker value)
        {
            throw new NotImplementedException();
        }

        
    }
}
