namespace SimpleChatApp;

public interface IUser : IEquatable<User>
{
    bool IsExpired { get; }
    void Update();
    string ToString();
}

public class User : IUser
{
    private readonly string _name;
    private readonly TimeSpan _expirationPeriod;
    private DateTime _time = DateTime.UtcNow;
    private DateTime _expirationTime => _time + _expirationPeriod;
    public User(string name, TimeSpan expirationPeriod)
    {
        _name = name;
        _expirationPeriod = expirationPeriod;
    }
    public bool IsExpired => DateTime.UtcNow > _expirationTime;
    public void Update() => _time = DateTime.UtcNow;
    public override string ToString() => _name;

    //public static bool operator ==(User a, User b) => a.Equals(b);
    //public static bool operator !=(User a, User b) => !a.Equals(b);
    public bool Equals(User? other) => other is not null && _name == other._name;
    public override bool Equals(object? obj) => Equals(obj as User);
    public override int GetHashCode() => _name.GetHashCode();
}
