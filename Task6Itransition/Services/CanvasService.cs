using Domain;
using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.SignalR.Client;
using SkiaSharp;
using Task6Itransition.Services.Drawers.Interfaces;
using Domain.Entities.Behaviors;
using Domain.DTOs;
using Task6Itransition.Services.Drawers.Drawers;

namespace Task6Itransition.Services
{
    public class CanvasService(IConfiguration configuration, SignalRSettings signalRSettings, SaveSchemeService saveSchemeService)
        : IAsyncDisposable
    {
        private string mapName = string.Empty;
        private Dictionary<CircuitItemType, List<CircuitItem>> allItems = new();
        private List<CircuitItem> tempItems = new();
        private IDrawer? curFigure;
        private SKPoint curPos = new SKPoint();
        private int basedViewPerimeter = 100;
        private float scale = 1f;
        private HubConnection? hubConnection;

        public List<CircuitItem> AllItems
        {
            get => allItems.Values.SelectMany(x => x).ToList();
        }
        public HubConnection? HubConnection { get => hubConnection; }

        private async Task StartNetworkConnectionAsync()
        {
            signalRSettings.StartBuildConnection();
            signalRSettings.AddServerCommands(this);
            hubConnection = signalRSettings.Build();
            await hubConnection.StartAsync();
        }

        public async Task StartAsync(string mapName)
        {
            this.mapName = mapName;
            await StartNetworkConnectionAsync();
            if (hubConnection is not null) await saveSchemeService.LoadItemsAsync(HubConnection!, mapName);
        }

        public void ChangeAction(IDrawer? drawer)
        {
            curFigure?.Dispose();
            curFigure = drawer;
        }

        private void DrawItem(SKCanvas canvas, CircuitItem figure)
        {
            if (figure.Figures.Count != figure.Paints.Count)
                throw new Exception("each part of figure mush have its own paint");
            for (int i = 0; i < figure.Figures.Count; i++)
            {
                canvas.DrawPath(figure.Figures[i], figure.Paints[i]);
            }
        }

        public void DrawAllItems(SKCanvas canvas)
        {
            foreach (var pair in allItems)
            {
                foreach (var figure in pair.Value)
                {
                    DrawItem(canvas, figure);
                }
            }
        }
        public void DrawPreview(SKCanvas canvas)
        {
            if (curFigure is not null)
            {
                var item = curFigure.GetPreviewItem();
                if (item is not null)
                {
                    DrawItem(canvas, item);
                }
            }
        }
        public void DrawTempItems(SKCanvas canvas)
        {
            foreach (var target in tempItems)
            {
                DrawItem(canvas, target);
            }
        }
        public void SetOutputConnections(CircuitItem new_item)
        {
            foreach (var pair in allItems)
            {
                foreach (var item in pair.Value)
                {
                    foreach (var new_item_output in new_item.Outputs)
                    {
                        foreach (var item_input in item.Inputs)
                        {
                            var distance = (new_item_output.Position - item_input.Position).Length;
                            if (distance <= int.Parse(configuration["DistanceForPotencialConnection"] ?? "0"))
                            {
                                if (item.InputConnectedItems.ContainsKey(item_input.Position)) continue;

                                new_item_output.Position = item_input.Position;
                                bool newItemDontContainsPosition = !new_item.OutputConnectedItems.ContainsKey(item_input.Position);
                                if (newItemDontContainsPosition)
                                {
                                    new_item.OutputConnectedItems.Add(item_input.Position, [item]);
                                }
                                else new_item.OutputConnectedItems[item_input.Position].Add(item);
                                item.InputConnectedItems.Add(item_input.Position, new_item);
                            }
                        }
                    }
                }
            }
        }

        public void SetInputConnection(CircuitItem new_item)
        {
            foreach (var pair in allItems)
            {
                foreach (var item in pair.Value)
                {
                    foreach (var new_item_input in new_item.Inputs)
                    {
                        foreach (var item_output in item.Outputs)
                        {
                            var distance = (new_item_input.Position - item_output.Position).Length;
                            if (distance <= int.Parse(configuration["DistanceForPotencialConnection"] ?? "0"))
                            {
                                if (new_item.InputConnectedItems.ContainsKey(item_output.Position)) continue;

                                new_item_input.Position = item_output.Position;
                                new_item.InputConnectedItems.Add(item_output.Position, item);
                                if (!item.OutputConnectedItems.ContainsKey(item_output.Position))
                                {
                                    item.OutputConnectedItems.Add(item_output.Position, [new_item]);
                                }
                                else
                                {
                                    item.OutputConnectedItems[item_output.Position].Add(new_item);
                                }
                            }
                        }
                    }
                }
            }
            foreach (var item in tempItems) item.Dispose();
            tempItems.Clear();
        }

        public bool CheckOutputConnections()
        {
            bool isFindPotencionalConnection = false;
            foreach (var pair in allItems)
            {
                foreach (var item in pair.Value)
                {
                    foreach (var new_item_output in curFigure?.GetPreviewItem()?.Outputs ?? Enumerable.Empty<Port>())
                    {
                        foreach (var item_input in item.Inputs)
                        {
                            var distance = (new_item_output.Position - item_input.Position).Length;
                            if (distance <= int.Parse(configuration["DistanceForPotencialConnection"] ?? "0"))
                            {
                                if (item.InputConnectedItems.ContainsKey(item_input.Position))
                                {
                                    continue;
                                }

                                curFigure!.SetPotencialConnection(item_input.Position);
                                isFindPotencionalConnection = true;

                                var signal = curFigure.GetSignalForPotencialConnection();
                                if (signal is null)
                                {
                                    isFindPotencionalConnection = true;
                                    continue;
                                }

                                int distanceThreshold = int.Parse(configuration["DistanceForPotencialConnection"] ?? "0");

                                bool needAddSingal = !tempItems.Any(x => (x.Position - signal.Position).Length < distanceThreshold);

                                if (needAddSingal)
                                {
                                    tempItems.Add(signal);
                                }
                            }
                        }
                    }
                }
            }
            return isFindPotencionalConnection;
        }

        public bool CheckInputConnection()
        {
            bool isFindPotencionalConnection = false;
            foreach (var pair in allItems)
            {
                foreach (var item in pair.Value)
                {
                    foreach (var new_item_input in curFigure?.GetPreviewItem()?.Inputs ?? Enumerable.Empty<Port>())
                    {
                        foreach (var item_output in item.Outputs)
                        {
                            var distance = (new_item_input.Position - item_output.Position).Length;
                            if (distance <= int.Parse(configuration["DistanceForPotencialConnection"] ?? "0"))
                            {
                                curFigure!.SetPotencialConnection(item_output.Position);
                                isFindPotencionalConnection = true;

                                var signal = curFigure.GetSignalForPotencialConnection();
                                if (signal is null)
                                {
                                    isFindPotencionalConnection = true;
                                    continue;
                                }

                                int distanceThreshold = int.Parse(configuration["DistanceForPotencialConnection"] ?? "0");

                                if (!tempItems.Any(x => (x.Position - signal.Position).Length < distanceThreshold))
                                {
                                    tempItems.Add(signal);
                                }
                            }
                        }
                    }
                }
            }
            return isFindPotencionalConnection;
        }

        public void AddItem(CircuitItem item)
        {
            ActionsBeforeAddingItem(item);
            allItems[item.Type].Add(item);
            ActionsAfterAddingItem(item);
        }

        private void ActionsBeforeAddingItem(CircuitItem item)
        {
            if (!allItems.ContainsKey(item.Type)) allItems.Add(item.Type, new List<CircuitItem>());
            SetOutputConnections(item);
            SetInputConnection(item);
            var behavior = BehaviorFactory.Create(item.Type);
            if (behavior is not null) item.Behavior = behavior;
        }

        private void ActionsAfterAddingItem(CircuitItem item)
        {
            if (allItems.ContainsKey(CircuitItemType.VoltageOn))
            {
                foreach (var on in allItems[CircuitItemType.VoltageOn])
                {
                    var success = on.Invoke();
                    if (success) on.NotifyNextItems();
                }
            }

            if (allItems.ContainsKey(CircuitItemType.VoltageOff))
            {
                foreach (var off in allItems[CircuitItemType.VoltageOff])
                {
                    var success = off.Invoke();
                    if (success) off.NotifyNextItems();
                }
            }
        }

        public MouseClickModel GetMouseClickModel(double x, double y, double canvas_dpi)
        {
            return new MouseClickModel
            {
                X = (float)(x * canvas_dpi),
                Y = (float)(y * canvas_dpi)
            };
        }

        public void DeleteItemsFromCanvas(List<CircuitItem> itemsForDelete)
        {
            foreach (var item in itemsForDelete)
            {
                allItems[item.Type].Remove(item);
                item.Dispose();
            };
        }

        private async Task DeleteItemsAsync(SKRect rect)
        {
            var itemsForDelete = new List<CircuitItem>();

            foreach (var pair in allItems)
            {
                foreach (var item in pair.Value)
                {
                    if (rect.Contains(item.Position))
                    {
                        itemsForDelete.Add(item);
                    }
                }
            }

            DeleteItemsFromCanvas(itemsForDelete);

            if (hubConnection is not null) await saveSchemeService.DeleteItemsAsync(itemsForDelete, hubConnection, mapName);
        }

        public async Task MouseClickAsync(MouseClickModel model)
        {
            curFigure?.MouseClick(model);
            if (curFigure?.IsComplete ?? false)
            {

                if(curFigure is DeleteDrawer)
                {
                    var rect = ((DeleteDrawer)curFigure).GetRect();
                    if(rect is null) return;
                    await DeleteItemsAsync(rect.Value);
                    return;
                }

                CircuitItem item = curFigure.GetItem() ?? throw new Exception("Item null after click on canvas");
                curFigure.DropPotencialConnection();
                AddItem(item);
                if (hubConnection is not null) await saveSchemeService.AddItemsAsync([item], hubConnection, mapName);
            }
        }

        public MouseMoveModel GetMouseMoveModel(double x, double y, double canvas_dpi)
        {
            return new MouseMoveModel
            {
                X = (float)(x * canvas_dpi),
                Y = (float)(y * canvas_dpi)
            };
        }

        public void MouseMove(MouseMoveModel model)
        {
            if (curFigure is not null)
            {
                foreach (var item in tempItems)
                {
                    item.Dispose();
                }
                tempItems.Clear();
                curFigure.DropPotencialConnection();
                CheckOutputConnections();
                CheckInputConnection();
                curFigure.MouseMove(model);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (hubConnection is not null) await hubConnection.DisposeAsync();
        }
    }
}
