namespace SimpleChatApp;

public interface IChat
{
    IUser UserByName(string name);
    void Join(IUser user);
    Task Publish(IMessage message, Func<IList<IUser>, Task> function);

    void Leave(IUser user);
}

public class Chat : IChat
{
    private object _sync = new();
    public HashSet<IUser> _users = new();
    public Chat(IUserMonitor monitor, CancellationToken ct) => monitor.Run(_users, ct);

    public IUser UserByName(string name)
    {
        try
        {
            return _users.Single(x => x.ToString() == name);
        }
        catch (InvalidOperationException)
        {
            throw new ApplicationException($"User: {name} does not exist in chat");
        }
    }

    public Task Publish(IMessage message, Func<IList<IUser>, Task> function)
    {
        var users = _users.Where(x =>
        {
            if (x == message.FromUser)
                x.Update();

            return x != message.FromUser;
        }).ToList();

        return function(users);
    }

    public void Join(IUser user)
    {
        lock (_sync)
        {
            if (_users.Contains(user))
                throw new ApplicationException($"{user} already join the chat");

            _users.Add(user);
        }
    }

    public void Leave(IUser user)
    {
        lock (_sync)
            _users.Remove(user);
    }
}
