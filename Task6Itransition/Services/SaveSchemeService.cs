using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.SignalR.Client;
using Domain;
using System.Threading.Tasks;
using Domain.DTOs;


namespace Task6Itransition.Services
{
    public class SaveSchemeService
    {
        public async Task RewriteScheme(List<CircuitItem> items, HubConnection hubConnection, string mapName)
        {
            await hubConnection.InvokeAsync("RewriteScheme", items.Select(x=> Serialiser.GetItemDTO(x)).ToList(), mapName);
        }

        public async Task AddItems(List<CircuitItem> items, HubConnection hubConnection, string mapName)
        {
            await hubConnection.InvokeAsync("AddItems", items.Select(x => Serialiser.GetItemDTO(x)).ToList(), mapName);
        }

        public async Task LoadItems(HubConnection hubConnection, string mapName)
        {
            await hubConnection.InvokeAsync("LoadItems", mapName);
        }
    }
}
