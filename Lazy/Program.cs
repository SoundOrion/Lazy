
AsyncLazy<string> _lazyString = new AsyncLazy<string>(async () =>
{
    await Task.Delay(2000);
    return "遅延初期化された値";
});

Console.WriteLine("値の取得開始...");
string value = await _lazyString.Value;
Console.WriteLine($"取得した値: {value}");


public class AsyncLazy<T>
{
    private readonly Lazy<Task<T>> _lazy;

    public AsyncLazy(Func<Task<T>> factory)
    {
        _lazy = new Lazy<Task<T>>(() => Task.Run(factory));
    }

    public Task<T> Value => _lazy.Value;
}