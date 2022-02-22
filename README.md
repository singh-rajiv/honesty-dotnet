# honesty-dotnet
`honesty-dotnet` is a collection of lightweight monads, `Optional<T>` and `Result<T>`, and few simple extension methods of the `Task<T>` type. These constructs can be used to write honest functions and have appropriate extension methods that let you use LINQ syntax on even non-sequence data types. Both synchronous and asynchronous versions of all APIs are available.

## What are pure functions?
In computer programming [pure functions](https://en.wikipedia.org/wiki/Pure_function) are those functions that always return the same result for same input parameter values, the result does not depend on anything other than the input parameter values i.e. it does not depend on any other external state, class or global or static variable, and they do not have side-effects i.e. they do not cause any change to external state, class or global or static variable.

## What are honest functions?
Honest functions are a step over and above pure functions. While they are also pure functions, they additionally let the consuming code handle scenarios like absense of a result or exception in computation of a result more gracefully than just pure functions. The behavior of pure functions is not really pure in these two scenarios. 

Honest functions solve this problem by amplifying the normal return type `T` of a pure function to special monadic types like `Optional<T>` or `Result<T>` which represent potential lack of a result or potential exception in computing the result respectively.

## Installation
It can be installed via Nuget Package Manager in Visual Studio or .NET CLI in Visual Studio Code.

```
PM > Install-Package HonestyDotNet
```

```
.NET CLI > dotnet add package HonestyDotNet
```

## `Optional<T>` type
It is a [monad](https://ericlippert.com/2013/02/21/monads-part-one/) that represents an amplified type T which may or may not contain a value.

There are multiple ways to create an `Optional<T>`.

Using new
```csharp
var o1 = new Optional<int>(5);
var o2 = new Optional<string>("Test");
var o3 = new Optional<object>(null);    //does not contain a value
```

Using static methods
```csharp
var o1 = Optional.Some(5);
var o2 = Optional.None(5);  //does not contain a value
```

Using Try methods which take a pure function as an input. Any exception is caught but ignored.

Note: In the examples below many funcs take in a nullable string `string?` and then use the null-forgiving operator `!` 
to get rid of compiler warning about dereferencing a maybe null object. This is to just show that any exceptions thrown are caught 
by the various Try methods. However, in the real world one would be expected to check nullable types for null before dereferencing or use non-nullable types.

```csharp
//sync version
var o1 = Optional.Try(() => 5);
var o2 = Optional.Try((string? s) => s!.Length, null);    //does not contain a value

//async version
static Task<int> Process(string? s) => Task.Run(() => s!.Length); //force NullReferenceException when s is null
var o1 = await Optional.Try(Process, "Test");
var o2 = await Optional.Try<string?, int>(Process, null);    //does not contain a value
```

Using extension method
```csharp
var s1 = "Hello";
string? s2 = null;
var o1 = s1.ToOptional();
var o2 = s2.ToOptional();   //does not contain a value
```

Based on the result of a boolean expression or variable
```csharp
var flag = true;
var o1 = flag.IfTrue(() => 100);

flag = false;
var o2 = flag.IfTrue(() => 10); //does not contain a value
```

### Functional composition using `Optional<T>`
The real power of monads is in their ability to allow creating arbitrary functional compositions while keeping the code simple and concise, which makes the intent of the programmer conspicuous.

```csharp
var sHello = "Hello";
var sWorld = "World";
string? sNull = null;

//an arbitrary task that yields an int when run to succesful completion
//by calculating some arbitrary tranformation on a given string
async Task<int> AsyncCodeOf(string? i) => await Task.Run(() => { return (int) Math.Sqrt(i!.GetHashCode()); });

//honesty-dotnet enables using LINQ query syntax on Task type and this does not require any extra effort on developer's part
//the return type of Optional.Try call below is Task<Optional<int>>
var r1 =
        await 
        from maybeValue1 in Optional.Try(AsyncCodeOf, sHello)
        from maybeValue2 in Optional.Try(AsyncCodeOf, sWorld)
        from maybeValue3 in Optional.Try(AsyncCodeOf, sHello + sWorld)
        select //this select clause executes only after all tasks above have completed
        (
            from value1 in maybeValue1
            from value2 in maybeValue2
            from value3 in maybeValue3
            select value1 + value2 + value3 //this select clause executes only if all 3 values exist
        );
Assert.True(r1.IsSome);                     //result r1 contains a value

var r2 = 
        await 
        from maybeValue1 in Optional.Try(AsyncCodeOf, sHello)
        from maybeValue2 in Optional.Try(AsyncCodeOf, sNull) //this task will fail with an exception
        from maybeValue3 in Optional.Try(AsyncCodeOf, sHello + sWorld)
        select
        (
            from value1 in maybeValue1
            from value2 in maybeValue2
            from value3 in maybeValue3
            select value1 + value2 + value3 //this select clause does not execute
        );
Assert.False(r2.IsSome);                    //result r2 does not contain a value
```

## `Result<T>` type
It is a monad that represents an amplified type T which either contains a value or an exception that was thrown trying to compute the value.

There are multiple ways to create a `Result<T>`.

Using new
```csharp
var ex = new Exception("Something happened");
var r1 = new Result<int>(5);                     //contains value
var r2 = new Result<int>(ex);                    //contains exception
```

Using static methods
```csharp
var ex = new Exception("Something happened");
var r1 = Result.Value(10);                       //contains value
var r2 = Result.Exception<int>(ex);              //contains exception
```

Using Try methods which take a pure function as an input. Any exception is caught and captured on the return type.
```csharp
//sync version
static int LengthOf(string? s) => s!.Length;          //force NullReferenceException when s in null
var r1 = Result.Try(LengthOf, "Hello");               //contains value
var r2 = Result.Try<string?, int>(LengthOf, null);    //contains exception

//async version
static Task<int> LengthOf(string? s) => Task.Run(() => s!.Length);
var r3 = await Result.Try(LengthOf, "Hello");             //contains value
var r4 = await Result.Try<string?, int>(LengthOf, null);  //contains exception
```

Using extension method
```csharp
var ex = new Exception("Something happened");
var i = 5;
var r1 = i.ToResult();           //contains value
var r2 = ex.ToResult<int>();     //contains exception
```

### Functional composition using `Result<T>`
The real power of monads is in their ability to allow creating arbitrary functional compositions while keeping the code simple and concise, which makes the intent of the programmer conspicuous.

```csharp
var sHello = "Hello";
var sWorld = "World";
string? sNull = null;

//an arbitrary task that yields an int when run to succesful completion
//by calculating some arbitrary tranformation on a given string
async Task<int> AsyncCodeOf(string? s) => await Task.Run(() 
    => { return (int) Math.Sqrt(s!.GetHashCode()); });

//honesty-dotnet enables using LINQ query syntax on Task type and this does not require any extra effort on developer's part
//the return type of Result.Try call below is Task<Result<int>>
var r1 = 
        await
        from exOrVal1 in Result.Try(AsyncCodeOf, sHello)
        from exOrVal2 in Result.Try(AsyncCodeOf, sWorld)
        from exOrVal3 in Result.Try(AsyncCodeOf, sHello + sWorld)
        select //this select clause executes only after all tasks above have completed
        (
            from val1 in exOrVal1
            from val2 in exOrVal2
            from val3 in exOrVal3
            select val1 + val2 + val3   //this select clause executes only if all 3 values exist
        );
Assert.True(r1.IsValue);                //result r1 contains value

var r2 = 
        await
        from exOrVal1 in Result.Try(AsyncCodeOf, sHello)
        from exOrVal2 in Result.Try(AsyncCodeOf, sNull)             //this task will fail with an exception which is captured
        from exOrVal3 in Result.Try(AsyncCodeOf, sHello + sWorld)
        select
        (
            from val1 in exOrVal1
            from val2 in exOrVal2
            from val3 in exOrVal3
            select val1 + val2 + val3   //this select clause does not execute
        );
Assert.False(r2.IsValue);
Assert.NotNull(r2.Exception);           //result r2 contains exception thrown above in task2
```

## Other functional methods
Both the types provide standard methods found in functional programming - `Match`, `Map`, `Bind`, `Where` etc, `DefaultIfNone` and `DefaultIfException`. All methods have asynchronous overloads.



