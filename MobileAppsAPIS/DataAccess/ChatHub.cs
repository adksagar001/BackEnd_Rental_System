using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    public async Task SendMessage(string fromUserId, string toUserId, string message)
    {
        await Clients.Group(toUserId).SendAsync("ReceiveMessage", fromUserId, message);
        await Clients.Group(fromUserId).SendAsync("ReceiveMessage", fromUserId, message);
    }

    public async Task NotifyTyping(string fromUserId, string toUserId)
    {
        await Clients.Group(toUserId).SendAsync("UserTyping", fromUserId);
    }
    public override async Task OnConnectedAsync()
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"];
        Console.WriteLine($"SignalR ConnectionId: {Context.ConnectionId}, UserId: {userId}");

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            Console.WriteLine($"Added ConnectionId {Context.ConnectionId} to group {userId}");
        }
        else
        {
            Console.WriteLine("No userId found in query string.");
        }
        await base.OnConnectedAsync();
    }

}
