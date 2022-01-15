#pragma warning disable CS8848
namespace Monads.Tests;

public class TaskLINQTests
{
    [Fact]
    public async Task Task_Select_Happy()
    {
        var completedTask = Task.FromResult(5);

        var v1 = await
                 from i in completedTask
                 select i * 4;
        Assert.Equal(20, v1);
    }

    [Fact]
    public async Task Task_Select_Unhappy()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();
        var ct = cts.Token;
        var cancelledTask = Task.FromCanceled<int>(ct);
        var faultedTask = Task.Run(() => new[] { 0 }[1]);

        await
        Assert.ThrowsAsync<TaskCanceledException>(() =>
            from i in cancelledTask
            select i * 4);

        await
        Assert.ThrowsAsync<TaskCanceledException>(() =>
            from i in faultedTask
            select i * 4);
    }

    [Fact]
    public async Task Task_SelectMany_Happy()
    {
        var completedTask1 = Task.FromResult(5);
        var completedTask2 = Task.FromResult(10);
        var completedTask3 = Task.FromResult("Hello ");
        var completedTask4 = Task.FromResult("World ");

        var v1 = await
                 from i in completedTask1
                 from j in completedTask2
                 select i + j;

        var v2 = await
                 from i in completedTask1
                 from j in completedTask2
                 from h in completedTask3
                 from w in completedTask4
                 select h + w + i + j;

        Assert.Equal(15, v1);
        Assert.Equal("Hello World 510", v2);
    }

    [Fact]
    public async Task Task_SelectMany_Unhappy()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();
        var ct = cts.Token;
        var completedTask = Task.FromResult(5);
        var cancelledTask = Task.FromCanceled<int>(ct);
        var faultedTask = Task.Run(() => new[] { 0 }[1]);

        await 
        Assert.ThrowsAsync<TaskCanceledException>(() => 
            from i in completedTask
            from j in cancelledTask
            select i + j);

        await
        Assert.ThrowsAsync<TaskCanceledException>(() => 
            from i in faultedTask
            from j in completedTask
            select i + j);

        await 
        Assert.ThrowsAsync<TaskCanceledException>(() => 
            from i in completedTask
            from j in cancelledTask
            from h in faultedTask
            select i + j + h);
    }
}
