# honesty-dotnet
`honesty-dotnet` is a collection of lightweight monads, `Optional<T>` and `Error<T>`, and few simple extension methods of the `Task<T>` type. These constructs can be used to write honest functions and have appropriate extension methods that let you use LINQ syntax on even non-sequence data types. Both synchronous and asynchronous versions of all APIs are available.

## What are pure functions?
In computer programming [pure functions](https://en.wikipedia.org/wiki/Pure_function) are those functions that always return the same result for same input parameter values, the result does not depend on anything other than the input parameter values i.e. it does not depend on any other external state, class or global or static variable, and they do not have side-effects i.e. they do not cause any change to external state, class or global or static variable.

## What are honest functions?
Honest functions are a step over and above pure functions. While they are also pure functions, they additionally let the consuming code handle scenarios like absense of a result or exception in computation of a result more gracefully than just pure functions. The behavior of pure functions is not really pure in these two scenarios. 

Honest functions solve this problem by amplifying the normal return type `T` of a pure function to special monadic types like `Optional<T>` or `Error<T>` which represent potential lack of a result or potential exception in computing the result respectively.

  
