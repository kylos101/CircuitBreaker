using System;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    public interface ICommand<T>
    {        
        Action Action { get; }
        Action<T> ActionT { get; }

        Task Task { get; }
        Task<T> TaskT { get; }

        Func<T> FuncT { get; }
    }
}