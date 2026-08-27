using Domain.Entities;
using Domain;
using Microsoft.AspNetCore.SignalR.Client;
using Domain.Enums;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using Domain.DTOs;
using SkiaSharp;

namespace Task6Itransition.Services
{
    public class SignalRSettings(NavigationManager navigation)
    {
        private HubConnection? _hubConnection = null;
        public void StartBuildConnection()
        {
            _hubConnection = new HubConnectionBuilder()
            .WithUrl(navigation.ToAbsoluteUri("https://localhost:7042/hub"))
            .WithAutomaticReconnect()
            .AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            })
            .Build();
        }
        public void AddServerCommands(CanvasService canvasService)
        {
            if (_hubConnection is null) throw new Exception("Start build connection first");
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
        }

        public HubConnection Build()
        {
            return _hubConnection ?? throw new Exception("Start build connection first");
        }
    }
}
