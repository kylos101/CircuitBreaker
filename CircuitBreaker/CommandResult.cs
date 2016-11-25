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
