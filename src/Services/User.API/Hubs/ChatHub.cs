using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using User.API.Models;

namespace User.API.Hubs
{
    public interface IChatHub
    {
        Task ActiveUser(string userId);
        Task ActiveUsers(List<string> userIds);
        Task PassiveUser(string userId);
        Task UserMessageReceived(string senderUserId, MessageModel messageModel);
    }

    [Authorize(AuthenticationSchemes = ServiceCollectionExtensions.JWTAuthScheme)]
    public class ChatHub : Hub<IChatHub>
    {
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();
        public static ConnectionMapping<string> Connections => _connections;

        private IEnumerable<Claim> GetCurrentUserClaims => Context.User.Identity.IsAuthenticated ? Context.User.Claims : new List<Claim>();
        private string GetCurrentUserId => GetCurrentUserClaims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value;

        public override Task OnConnectedAsync()
        {
            string currentUserId = GetCurrentUserId;
            return base.OnConnectedAsync().ContinueWith((task) =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    if (!_connections.GetConnections(currentUserId).Contains(Context.ConnectionId))
                    {
                        _connections.Add(currentUserId, Context.ConnectionId);
                    }

                    return Clients.All.ActiveUser(currentUserId); 
                }

                return Task.CompletedTask;
            });
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string currentUserId = GetCurrentUserId;
            return base.OnDisconnectedAsync(exception).ContinueWith((task) =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    _connections.Remove(currentUserId, Context.ConnectionId);
                    return Clients.All.PassiveUser(currentUserId);
                }

                return Task.CompletedTask;
            });
        }

        public async Task SendActiveUser(string userId)
        {
            await Clients.All.ActiveUser(userId);
        }

        public async Task SendActiveUsers()
        {
            var activeUserIds = _connections.GetAllConnections().Select(c => c.Key).ToList();
            //var userId = Context.UserIdentifier;
            //await Clients.User(userId).ActiveUsers(activeUserIds);

            await Clients.All.ActiveUsers(activeUserIds);
        }

        public async Task SendPassiveUser(string userId)
        {
            await Clients.All.PassiveUser(userId);
        }

        public async Task SendUserMessage(string userId, MessageModel messageModel)
        {
            var senderUserId = Context.UserIdentifier;
            await Clients.User(userId).UserMessageReceived(senderUserId, messageModel);
        }
    }
}
