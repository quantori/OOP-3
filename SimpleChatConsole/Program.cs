namespace SimpleChatApp;

class Program
{
    public async static Task Main()
    {
        var ts = new CancellationTokenSource();
        var chat = new Chat(new UserMonitor(500), ts.Token);

        var expirationPeriod = TimeSpan.FromSeconds(3);

        var user1 = new User("user-1", expirationPeriod);
        chat.Join(user1);

        var user2 = new User("user-2", expirationPeriod);
        chat.Join(user2);

        for (int i = 0; i < 3; i++)
        {
            await Publish(chat, user1, $"message - {i}");
            await Publish(chat, user2, $"message - {i}");
            await Task.Delay(500);
        }
        await Task.Delay(3000);

        user1 = new User("user-1", expirationPeriod);
        chat.Join(user1);

        user2 = new User("user-2", expirationPeriod);
        chat.Join(user2);

        await Publish(chat, user1, $"message - new 1");
        await Publish(chat, user2, $"message - new 2");

        chat.Leave(user1);

        await Task.Run(() => Console.ReadKey());
    }

    private static async Task Publish(IChat chat, IUser user, string text)
    {
        var message = new TextMessage(user, text);
        await chat.Publish(message, (_) => WriteLine($"published {message}"));
    }

    public static async Task WriteLine(string message)
    {
        await Task.Run(() => Console.WriteLine(message));
    }
}
