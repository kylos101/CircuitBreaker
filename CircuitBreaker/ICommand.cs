using System;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    /// <summary>
    /// Something we're trying to do in an external system
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICommand<T>
    {
        /// <summary>
        /// An action in an external system, and it's result
        /// </summary>
        CommandResult ExecuteAction { get; }            
    }
}