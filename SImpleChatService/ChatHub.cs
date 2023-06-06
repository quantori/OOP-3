using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SimpleChatApp;

namespace SignalR.SimpleChatService;

public class ChatHub : Hub
{
    private readonly IChat _chat;
    private readonly ILogger<ChatHub> _logger;
    static Dictionary<IUser, string> _connectionIds = new();

    public ChatHub(IChat chat, ILogger<ChatHub> log)
    {
        _chat = chat;
        _logger = log;
    }
    public async Task Join(string userName)
    {
        var user = new User(userName, TimeSpan.FromSeconds(300));
        try
        {
            _chat.Join(user);

            if (_connectionIds.ContainsKey(user))
                _connectionIds.Remove(user);

            _connectionIds.Add(user, Context.ConnectionId);

            await Clients.Caller.SendAsync("ReceiveJoin", "joined");
            await Publish(userName, "joined the chat");

            _logger.LogInformation($"Joined: {userName}");
        }
        catch (ApplicationException e)
        {
            _logger.LogError(e.Message);
        }
    }

    public async Task Publish(string userName, string message)
    {
        try
        {
            var chatUser = _chat.UserByName(userName);
            var textMessage = new TextMessage(chatUser, message);

            await _chat.Publish(textMessage,
                async (users) =>
                {
                    var chatConnectionIds = _connectionIds.Where(x => users.Contains(x.Key)).Select(x => x.Value).ToList();
                    if (chatConnectionIds.Any())
                    {
                        await Clients.Clients(chatConnectionIds).SendAsync("ReceivePublish", textMessage.ToString());

                        _logger.LogInformation($"sent to {chatConnectionIds.Count()} user(s)");
                    }
                    else
                        _logger.LogInformation("there is no one to send");
                });

        }
        catch (ApplicationException e)
        {
            _logger.LogError(e.Message);
        }
    }

    public async Task Leave(string userName)
    {
        try
        {
            await Publish(userName, " left the chat");

            var userConn = _connectionIds.Single(x => x.Key.ToString() == userName);
            _chat.Leave(userConn.Key);

            _logger.LogInformation($"Left: {userName}");
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }

        await Task.FromResult(0);
    }
}
