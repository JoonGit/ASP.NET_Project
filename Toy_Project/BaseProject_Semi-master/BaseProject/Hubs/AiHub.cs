using Microsoft.AspNetCore.SignalR;

namespace BaseProject.Hubs
{
    public class AiHub : Hub
    {
        public async Task SendMessage1(string message)
        {
            await Console.Out.WriteLineAsync(message);
            await Clients.All.SendAsync("ReceiveAiMessage", message);
        }

    }
}
