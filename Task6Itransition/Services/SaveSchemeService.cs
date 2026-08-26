using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.SignalR.Client;
using Domain;


namespace Task6Itransition.Services
{
    public class SaveSchemeService
    {
        public void SaveScheme(List<CircuitItem> items, HubConnection hubConnection)
        {
            hubConnection.SendAsync("SaveItems", items.Select(x=> Serialiser.GetItemDTO(x)));
        }
    }
}
