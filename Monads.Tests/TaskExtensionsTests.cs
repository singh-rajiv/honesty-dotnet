namespace Monads.Tests;

public class TaskExtensionsTests
{
    [Fact]
    public async Task Task_Map()
    {
        var completedTask = Task.FromResult(5);
        var faultedTask = Task.Run(() => new[] { 0 }[1]);

        var v1 = await
                 completedTask.
                 Map(i => i * 4).
                 Map(i => i + 2);
        Assert.Equal(5 * 4 + 2, v1);

        await
        Assert.ThrowsAsync<TaskCanceledException>(() =>
            faultedTask.Map(i => i * 4));
    }

    [Fact]
    public async Task Task_Bind()
    {
        var completedTask = Task.FromResult(5);
        var faultedTask = Task.Run(() => new[] { 0 }[1]);

        var v1 = await
                 completedTask.
                 Bind(i => Task.FromResult(i * i)).
                 Bind(i => Task.FromResult(i + 100));
        Assert.Equal(5 * 5 + 100, v1);

        await
        Assert.ThrowsAsync<TaskCanceledException>(() =>
            faultedTask.Bind(i => Task.FromResult(i * 4)));
    }
}
