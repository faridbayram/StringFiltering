using System.Collections.Concurrent;
using System.Text;
using StringFiltering.Infrastructure.Utilities.Queueing;

namespace StringFiltering.UnitTests;

public class BackgroundQueueTests
{
    [Fact]
    public async Task Enqueue_Then_TryDequeue_ReturnsTrueAndCorrectData()
    {
        var queue = new BackgroundQueue();
        queue.Enqueue("id1", ToAsyncEnumerable("text1"));

        var success = queue.TryDequeue(out var uploadId, out var fullText);

        var combinedText = await FlattenAsyncEnumerable(fullText);

        Assert.True(success);
        Assert.Equal("id1", uploadId);
        Assert.Equal("text1", combinedText);
    }

    [Fact]
    public void TryDequeue_WhenEmpty_ReturnsFalseAndDefaultValues()
    {
        var queue = new BackgroundQueue();

        var success = queue.TryDequeue(out var uploadId, out var fullText);

        Assert.False(success);
        Assert.Null(uploadId);
        Assert.Null(fullText);
    }

    [Fact]
    public async Task Enqueue_MultipleItems_PreservesOrder()
    {
        var queue = new BackgroundQueue();

        queue.Enqueue("id1", ToAsyncEnumerable("text1"));
        queue.Enqueue("id2", ToAsyncEnumerable("text2"));

        queue.TryDequeue(out var id1, out var text1);
        queue.TryDequeue(out var id2, out var text2);

        var combined1 = await FlattenAsyncEnumerable(text1);
        var combined2 = await FlattenAsyncEnumerable(text2);

        Assert.Equal("id1", id1);
        Assert.Equal("text1", combined1);
        Assert.Equal("id2", id2);
        Assert.Equal("text2", combined2);
    }

    [Fact]
    public async Task ConcurrentEnqueueAndDequeue_WorksCorrectly()
    {
        var queue = new BackgroundQueue();
        var totalItems = 1000;
        var added = new ConcurrentBag<string>();
        var removed = new ConcurrentBag<string>();

        await Task.WhenAll(Enumerable.Range(0, totalItems).Select(i =>
            Task.Run(() =>
            {
                var id = $"id-{i}";
                queue.Enqueue(id, ToAsyncEnumerable($"text-{i}"));
                added.Add(id);
            })
        ));

        await Task.WhenAll(Enumerable.Range(0, totalItems).Select(_ =>
            Task.Run(async () =>
            {
                if (queue.TryDequeue(out var id, out var asyncText))
                {
                    await FlattenAsyncEnumerable(asyncText); // Drain to avoid unconsumed async streams
                    removed.Add(id);
                }
            })
        ));

        Assert.Equal(totalItems, removed.Count);
        Assert.All(removed, id => Assert.Contains(id, added));
    }

    private static async IAsyncEnumerable<string> ToAsyncEnumerable(params string[] items)
    {
        foreach (var item in items)
        {
            yield return item;
            await Task.Yield();
        }
    }

    private static async Task<string> FlattenAsyncEnumerable(IAsyncEnumerable<string> asyncLines)
    {
        var builder = new StringBuilder();
        await foreach (var line in asyncLines)
            builder.Append(line);
        return builder.ToString();
    }
}
