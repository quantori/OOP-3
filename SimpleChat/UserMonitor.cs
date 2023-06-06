namespace SimpleChatApp;

public interface IUserMonitor
{
    Task Run(HashSet<IUser> set, CancellationToken ct);
}

public class UserMonitor : IUserMonitor
{
    private object _sync = new();
    private int _delayToCheckUsersMs;

    public UserMonitor(int delayToCheckUsersMs) => _delayToCheckUsersMs = delayToCheckUsersMs;

    public Task Run(HashSet<IUser> set, CancellationToken ct)
    {
        return Task.Factory.StartNew(async (x) =>
            {
                while (!ct.IsCancellationRequested)
                {
                    lock (_sync)
                    {
                        int removedCount = set.RemoveWhere(x => x.IsExpired);
                        if (removedCount > 0)
                        {
                            Console.WriteLine($"removed expired: {removedCount}");
                        }
                    }
                    await Task.Delay(_delayToCheckUsersMs);
                }
            },
            TaskCreationOptions.LongRunning,
            ct);
    }
}
