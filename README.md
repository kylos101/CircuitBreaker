CircuitBreaker
==============

A circuit breaker class library

Hello there! This is my first attempt at implementing a circuit breaker pattern. 

I started here:
http://msdn.microsoft.com/en-us/library/dn589784.aspx

As you can see, it was heavily borrowed...the plan is to make changes as I go.

How to use it
=============

Setup a field to store a breaker for a type (in this case, an EF context):
```private Breaker daoBreaker = new Breaker(typeof(FubarContext));    ```

Setup an Action delegate to feed to the breaker:
```Action getAllFubar = this.GetAllFubar;    ```

Fire off the breaker, it'll eat exceptions, anything that bubbles up "should" be the breaker tripping (or failing...what...I'm not perfect):
```try
{
    this.daoBreaker.ExecuteAction(getAllFubar);
}
catch
{
    GetAllFubarBackup();
}
finally
{
    
}

return this.fubars; 
```
