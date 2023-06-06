using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimpleChatApp;

namespace SignalR.SimpleChatService;

class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        using var cts = new CancellationTokenSource();
        
        builder.Services.AddSingleton<IChat>(c => new Chat(new UserMonitor(1_000), cts.Token));
        builder.Services.AddSignalR();

        var app = builder.Build();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.MapHub<ChatHub>("/simple-chat");

        app.Run();
        cts.Cancel();
    }
}
