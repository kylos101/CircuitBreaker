using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitBreaker
{
    /// <summary>
    /// Potential results for ICommand objects
    /// </summary>
    public enum CommandResult
    {
        Succeeded,
        Failed,
        TimedOut
    }
}
