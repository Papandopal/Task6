using System.Threading.Tasks;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using Task6Itransition_Server.Services.Database;

namespace Task6Itransition.Services
{
    public class CenterHub(AppDbContext dbContext) : Hub
    {
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
    }
}
