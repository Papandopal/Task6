using Domain.Entities;
using Domain;
using Microsoft.AspNetCore.SignalR.Client;
using Domain.Enums;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using Domain.DTOs;
using SkiaSharp;
using Task6Itransition.Pages;

namespace Task6Itransition.Services
{
    public class SignalRSettings
    {
        private HubConnection _hubConnection;
        private readonly NavigationManager _navigationManager;
        public SignalRSettings(NavigationManager navigation, IConfiguration configuration)
        {
            _navigationManager = navigation;
            _hubConnection = new HubConnectionBuilder()
            .WithUrl(_navigationManager.ToAbsoluteUri(configuration["ServerURL"] +"/hub"))
            .WithAutomaticReconnect()
            .AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            })
            .Build();
            _hubConnection.StartAsync();
        }
        public void AddServerCommandsForCanvas(CanvasService canvasService)
        {
            _hubConnection.On<List<CircuitItemDTO>>("AddItems", (dtos) =>
            {
                foreach (var dto in dtos)
                {
                    var item = Serialiser.GetItem(dto);
                    canvasService.AddItem(item);
                }
            });

            _hubConnection.On<List<CircuitItemDTO>>("LoadItems", (items) =>
            {
                foreach (var item in items)
                {
                    var restoredItem = Serialiser.GetItem(item);
                    canvasService.AddItem(restoredItem);
                }
            });

            _hubConnection.On<List<PointDTO>>("DeleteItems", (dtos) =>
            {
                var itemsToDelete = canvasService.AllItems.Where(x => dtos.Contains(Serialiser.GetPointDTO(x.Position)));
                canvasService.DeleteItemsFromCanvas(itemsToDelete);
            });

            _hubConnection.On<PointDTO, Guid>("DrawUserCursor", (cursor, cursorOwnerId) =>
            {
                canvasService.UpdateCursor(Serialiser.GetPoint(cursor), cursorOwnerId);
            });

            _hubConnection.On<Dictionary<Guid, UserName>>("LoadUsers", (users) =>
            {
                canvasService.LoadUsers(users);
            });
        }

        public void RemoveServerCommandsForCanvas()
        {
            _hubConnection.Remove("AddItems");
            _hubConnection.Remove("LoadItems");
            _hubConnection.Remove("DeleteItems");
            _hubConnection.Remove("DrawUserCursor");
            _hubConnection.Remove("LoadUsers");
        }

        public void AddServerCommandsForHomePage(Home home)
        {
            _hubConnection.On<IEnumerable<string>>("GetAllMapsNames", (mapNames) =>
            {
                home.SetMapsNames(mapNames);
            });

            _hubConnection.On<bool>("MapIsExists", (isExists) =>
            {
                home.MapIsExists(isExists);
            });
        }

        public void RemoveServerCommandsForHomePage()
        {
            _hubConnection.Remove("GetAllNames");
        }
        public HubConnection GetConnection()
        {
            return _hubConnection;
        }
    }
}
