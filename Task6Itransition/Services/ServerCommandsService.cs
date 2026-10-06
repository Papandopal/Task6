using Domain.Entities;
using Microsoft.AspNetCore.SignalR.Client;
using Domain;
using SkiaSharp;


namespace Task6Itransition.Services
{
    public class ServerCommandsService
    {
        public HubConnection Connection { private get; set; }
        public async Task RewriteSchemeAsync(List<CircuitItem> items, string mapName)
        {
            await Connection.InvokeAsync("RewriteScheme", items.Select(x=> Serialiser.GetItemDTO(x)).ToList(), mapName);
        }

        public async Task AddItemsAsync(List<CircuitItem> items, string mapName)
        {
            await Connection.InvokeAsync("AddItems", items.Select(x => Serialiser.GetItemDTO(x)).ToList(), mapName);
        }

        public async Task LoadItemsAsync(string mapName)
        {
            await Connection.InvokeAsync("LoadItems", mapName);
        }

        public async Task DeleteItemsAsync(List<CircuitItem> items, string mapName)
        {
            await Connection.InvokeAsync("DeleteItems", items.Select(x => x.Position).ToList(), mapName);
        }

        public async Task GetAllMapsNamesAsync()
        {
            await Connection.InvokeAsync("GetAllMapsNames");
        }

        public async Task MapIsExists(string mapName)
        {
            await Connection.InvokeAsync("MapIsExists", mapName); 
        }

        public async Task DrawUserCursor(SKPoint userPosition, string mapName, Guid userId)
        {
            await Connection.InvokeAsync("DrawUserCursor", Serialiser.GetPointDTO(userPosition), mapName, userId);
        }

        public async Task AddUser(string mapName, string userName, Guid userId)
        {
            await Connection.InvokeAsync("AddUser", mapName, userName, userId); 
        }

        public async Task RemoveUser(string mapName, Guid userId)
        {
            await Connection.InvokeAsync("RemoveUser", mapName, userId);
        }
    }
}
