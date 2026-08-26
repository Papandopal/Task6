using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using SkiaSharp;
using Task6Itransition.Services.Drawers.Interfaces;

namespace Task6Itransition.Services.Drawers.Drawers
{
    public class LampDrawer : IDrawer, ICircuitDrawer
    {
        private bool isComplete = false;
        private CircuitItem? finalItem = null;
        private CircuitItem? previewItem = null;
        private CircuitItem? signalForPotencialConnection = null;

        bool IDrawer.IsComplete => isComplete;

        void IDrawer.DropPotencialConnection()
        {
            signalForPotencialConnection?.Dispose();
            signalForPotencialConnection = null;
        }

        CircuitItem? IDrawer.GetItem()
        {
            return isComplete ? finalItem : null;
        }

        CircuitItem? IDrawer.GetPreviewItem()
        {
            return previewItem;
        }

        CircuitItem? IDrawer.GetSignalForPotencialConnection()
        {
            return signalForPotencialConnection;
        }

        void IDrawer.MouseClick(MouseClickModel model)
        {
            using var builder = new SKPathBuilder();
            List<SKPath> figure = new();
            List<SKPoint> inputs = new();
            List<SKPoint> outputs = new();
            int halfSquareSideSize = 40;
            float portsRadius = 10;

            var rect = SKRect.Create(model.X - halfSquareSideSize,
                model.Y - halfSquareSideSize,
                2 * halfSquareSideSize,
                2 * halfSquareSideSize);

            builder.AddRect(rect);

            inputs.Add(new SKPoint { X = model.X - halfSquareSideSize, Y = model.Y });

            builder.AddCircle(inputs[0].X, inputs[0].Y, portsRadius);

            figure.Add(builder.Detach());

            rect.Inflate(-3, -3);

            builder.AddRect(rect);

            figure.Add(builder.Detach());

            finalItem = new CircuitItem
            {
                Position = new SKPoint(model.X, model.Y),
                Figures = figure,
                Paints =
                [
                     new SKPaint { IsAntialias = true, Color = new(255, 0, 0), Style = SKPaintStyle.Stroke, StrokeWidth = 3 },
                     new SKPaint { IsAntialias = true, Color = new(255, 255, 255), Style = SKPaintStyle.Fill }
                ],
                Type = CircuitItemType.Lamp,
                Inputs = inputs.Select(x => new Port { Position = x, Value = false }).ToList(),
                Outputs = outputs.Select(x => new Port { Position = x, Value = false }).ToList()
            };

            isComplete = true;
        }

        void IDrawer.MouseMove(MouseMoveModel model)
        {
            using var builder = new SKPathBuilder();
            List<SKPath> figure = new();
            List<SKPoint> inputs = new();
            List<SKPoint> outputs = new();
            int halfSquareSideSize = 40;
            float portsRadius = 10;

            var rect = SKRect.Create(model.X - halfSquareSideSize,
                model.Y - halfSquareSideSize,
                2 * halfSquareSideSize,
                2 * halfSquareSideSize);

            builder.AddRect(rect);

            inputs.Add(new SKPoint { X = model.X - halfSquareSideSize, Y = model.Y });

            builder.AddCircle(inputs[0].X, inputs[0].Y, portsRadius);

            figure.Add(builder.Detach());

            rect.Inflate(-3, -3);

            builder.AddRect(rect);

            figure.Add(builder.Detach());

            previewItem?.Dispose();

            previewItem = new CircuitItem
            {
                Position = new SKPoint(model.X, model.Y),
                Figures = figure,
                Paints = 
                [
                     new SKPaint { IsAntialias = true, Color = new(0, 255, 0), Style = SKPaintStyle.Stroke, StrokeWidth = 3 },
                     new SKPaint { IsAntialias = true, Color = new(255, 255, 255), Style = SKPaintStyle.Fill }
                ],
                Type = CircuitItemType.Lamp,
                Inputs = inputs.Select(x => new Port { Position = x, Value = false }).ToList(),
                Outputs = outputs.Select(x => new Port { Position = x, Value = false }).ToList()
            };
        }

        void IDrawer.SetPotencialConnection(SKPoint potentialConnectionPoint)
        {
            signalForPotencialConnection = new CircuitItem
            {
                Position = potentialConnectionPoint,
                Figures = ((ICircuitDrawer)this).GetBasedSignalForPotencialConnection(potentialConnectionPoint),
                Paints = ((ICircuitDrawer)this).GetBasedSignalForPotencialConnectionPaint()
            };
        }
    }
}
