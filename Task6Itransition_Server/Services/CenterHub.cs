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
            await Clients.Others.SendAsync("AddItems", items); // add groups
        }
        public async Task LoadItems(string mapName)
        {
            await Clients.Caller.SendAsync("LoadItems", await dbContext.LoadAsync(mapName));
        }
    }
}
