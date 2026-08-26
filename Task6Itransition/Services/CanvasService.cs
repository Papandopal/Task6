using Domain;
using System.Reflection;
using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.SignalR.Client;
using SkiaSharp;
using SkiaSharp.Views.Blazor;
using Task6Itransition.Services.Drawers.Interfaces;
using Domain.Entities.Behaviors;

namespace Task6Itransition.Services
{
    public class CanvasService(IConfiguration configuration, SignalRSettings signalRSettings) : IAsyncDisposable
    {
        private Dictionary<CircuitItemType, List<CircuitItem>> allItems = new();
        private List<CircuitItem> tempItems = new();
        private IDrawer? curFigure;
        private SKPoint curPos = new SKPoint();
        private int basedViewPerimeter = 100;
        private float scale = 1f;
        private HubConnection? hubConnection;

        private async Task StartNetworkConnectionAsync()
        {
            signalRSettings.StartBuildConnection();
            signalRSettings.AddServerCommands(this);
            hubConnection = signalRSettings.Build();
            await hubConnection.StartAsync();
        }

        public async Task StartAsync()
        {
            await StartNetworkConnectionAsync();
        }

        public void ChangeAction(IDrawer? drawer)
        {
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
                                new_item.OutputConnectedItems.Add(item_input.Position, item);
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
                                new_item.InputConnectedItems.Add(item_output.Position, item);
                                item.OutputConnectedItems.Add(item_output.Position, new_item);
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
                                curFigure!.MouseMove(new MouseMoveModel { X = item_input.Position.X, Y = item_input.Position.Y });
                                curFigure.SetPotencialConnection(item_input.Position);
                                isFindPotencionalConnection = true;

                                var signal = curFigure.GetSignalForPotencialConnection();
                                if (signal is null) return isFindPotencionalConnection;

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
                                curFigure!.MouseMove(new MouseMoveModel { X = item_output.Position.X, Y = item_output.Position.Y });
                                curFigure.SetPotencialConnection(item_output.Position);
                                isFindPotencionalConnection = true;

                                var signal = curFigure.GetSignalForPotencialConnection();
                                if (signal is null) return isFindPotencionalConnection;

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

        public async Task MouseClickAsync(MouseClickModel model)
        {
            curFigure?.MouseClick(model);
            if (curFigure?.IsComplete ?? false)
            {
                CircuitItem item = curFigure.GetItem() ?? throw new Exception("Item null after click on canvas");
                curFigure.DropPotencialConnection();
                AddItem(item);
                if (hubConnection is not null) await hubConnection.SendAsync("AddItem", Serialiser.GetItemDTO(item));
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
