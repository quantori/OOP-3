using Microsoft.AspNetCore.SignalR.Client;

namespace SignalR.SimpleChatClient;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var url = "http://localhost:5000/simple-chat";
        var hubConnection = new HubConnectionBuilder()
                                 .WithUrl(url)
                                 .Build();
        var userName = args?.Length > 0 ? args[0] : "User1";

        bool isJoined = false;
        hubConnection.On<string>("ReceiveJoin", message =>
            {
                if (message == "joined")
                    isJoined = true;
                else
                    isJoined = false;

                Console.WriteLine(message);
            });

        hubConnection.On<string>("ReceivePublish", message =>
            {
                Console.WriteLine(message);
            });

        Console.CancelKeyPress += new(async (_, args) =>
        {
            try
            {
                await hubConnection.SendAsync("Leave", userName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            isJoined = false;
            args.Cancel = true;
        });

        try
        {
            await hubConnection.StartAsync();
            await Task.Delay(1000);

            await hubConnection.SendAsync("Join", userName);
            await Task.Delay(1000);

            int count = 0;

            while (isJoined)
            {
                await hubConnection.SendAsync("Publish", userName, $"test publish {++count}");
                await Task.Delay(3000);
            }

            await hubConnection.StopAsync();
            await hubConnection.DisposeAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}