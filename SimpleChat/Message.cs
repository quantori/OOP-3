namespace SimpleChatApp;

public interface IMessage
{
    IUser FromUser { get; }
    string ToString();
}

public class TextMessage : IMessage
{
    private readonly IUser _fromUser;
    private readonly string _content;
    public IUser FromUser => _fromUser;
    public TextMessage(IUser fromUser, string content)
    {
        if (fromUser.IsExpired)
            throw new ApplicationException($"{fromUser} is expired");

        _fromUser = fromUser;
        _content = content;
    }
    public override string ToString() => $"{_fromUser}: {_content}";
}
