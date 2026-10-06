using System.Threading.Tasks;
using Domain.DTOs;
using Microsoft.AspNetCore.SignalR;
using Task6Itransition_Server.Services;
using Task6Itransition_Server.Services.Database;

namespace Task6Itransition.Services
{
    public class CenterHub(AppDbContext dbContext) : Hub
    {
        static Dictionary<string, (string mapName, Guid userId)> connections = new();
        public async Task RewriteScheme(List<CircuitItemDTO> items, string mapName)
        {
            await dbContext.RewriteMapAsync(items, mapName);
        }
        public async Task AddItems(List<CircuitItemDTO> items, string mapName)
        {
            await dbContext.AddItems(items, mapName);
            await Clients.OthersInGroup(mapName).SendAsync("AddItems", items); 
        }
        public async Task LoadItems(string mapName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, mapName);
            await Clients.Caller.SendAsync("LoadItems", await dbContext.LoadAsync(mapName));
        }

        public async Task DeleteItems(List<PointDTO> items, string mapName)
        {
            await dbContext.DeleteItemsAsync(items, mapName);
            await Clients.OthersInGroup(mapName).SendAsync("DeleteItems", items);
        }

        public async Task GetAllMapsNames()
        {
            IEnumerable<string> result = dbContext.GetAllMapsNames().ToList();
            await Clients.Caller.SendAsync("GetAllMapsNames", result);
        }

        public async Task MapIsExists(string mapName)
        {
            await Clients.Caller.SendAsync("MapIsExists", dbContext.MapIsExists(mapName));
        }

        public async Task DrawUserCursor(PointDTO point, string mapName, Guid userId)
        {
            await Clients.OthersInGroup(mapName).SendAsync("DrawUserCursor", point, userId);
        }

        public async Task AddUser(string mapName, string userName, Guid userId)
        {
            connections.Add(Context.ConnectionId, (mapName, userId));
            var users = UserNamesService.Add(mapName, userName, userId);
            await Clients.Group(mapName).SendAsync("LoadUsers", users);
        }

        public async Task RemoveUser(string mapName, Guid userId)
        {
            var users = UserNamesService.Remove(mapName, userId);
            await Clients.OthersInGroup(mapName).SendAsync("LoadUsers", users);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            (string mapName, Guid userId) disconnectedUser = connections[Context.ConnectionId];
            var users = UserNamesService.Remove(disconnectedUser.mapName, disconnectedUser.userId);
            await Clients.OthersInGroup(disconnectedUser.mapName).SendAsync("LoadUsers", users);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
