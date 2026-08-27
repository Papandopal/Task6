using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using SkiaSharp;
using Task6Itransition.Services.Drawers.Interfaces;

namespace Task6Itransition.Services.Drawers.Drawers
{
    public class VoltageOnDrawer : IDrawer
    {
        private bool isComplete = false;
        private CircuitItem? finalItem = null;
        private CircuitItem? previewItem = null;
        private CircuitItem? signalForPotencialConnection = null;

        bool IDrawer.IsComplete => isComplete;

        void IDisposable.Dispose()
        {
            finalItem?.Dispose();
            previewItem?.Dispose();
            signalForPotencialConnection?.Dispose();
        }

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
            int outputRadius = 10;
            int halfSquareSideSize = 15;
            int textSize = 16;
            SKPoint output = new SKPoint(model.X, model.Y + outputRadius + halfSquareSideSize);

            var onFont = new SKFont(SKTypeface.Default, textSize).GetTextPath("1");
            var bounds = onFont.Bounds;
            onFont.Offset(model.X - bounds.MidX, model.Y - bounds.MidY);
            builder.AddPath(onFont);
            figure.Add(builder.Detach());

            var rect = SKRect.Create(model.X - halfSquareSideSize,
                model.Y - halfSquareSideSize,
                2 * halfSquareSideSize,
                2 * halfSquareSideSize);

            builder.AddRect(rect);

            builder.AddCircle(output.X, output.Y, outputRadius);

            SKPath on = builder.Detach();
            figure.Add(on);

            finalItem = new CircuitItem
            {
                Position = new SKPoint(model.X, model.Y),
                Figures = figure,
                Paints =
                [
                    new SKPaint { IsAntialias = true, Color = new(0, 0, 0), Style = SKPaintStyle.Stroke, StrokeWidth = 1 },
                    new SKPaint { IsAntialias = true, Color = new(255, 255, 0), Style = SKPaintStyle.Stroke, StrokeWidth = 3 }
                ],
                Type = CircuitItemType.VoltageOn,
                Inputs = new List<Port>(),
                Outputs = new List<Port> { new Port { Position = output, Value = true } }
            };

            isComplete = true;
        }

        void IDrawer.MouseMove(MouseMoveModel model)
        {
            using var builder = new SKPathBuilder();
            List<SKPath> figure = new();
            int outputRadius = 10;
            int halfSquareSideSize = 15;
            int textSize = 16;
            SKPoint output = new SKPoint(model.X, model.Y + outputRadius + halfSquareSideSize);

            var onFont = new SKFont(SKTypeface.Default, textSize).GetTextPath("1");
            var bounds = onFont.Bounds;
            onFont.Offset(model.X - bounds.MidX, model.Y - bounds.MidY);
            builder.AddPath(onFont);
            figure.Add(builder.Detach());

            var rect = SKRect.Create(model.X - halfSquareSideSize,
                model.Y - halfSquareSideSize,
                2 * halfSquareSideSize,
                2 * halfSquareSideSize);

            builder.AddRect(rect);

            builder.AddCircle(output.X, output.Y, outputRadius);

            SKPath on = builder.Detach();
            figure.Add(on);

            previewItem?.Dispose();

            previewItem = new CircuitItem
            {
                Position = new SKPoint(model.X, model.Y),
                Figures = figure,
                Paints =
                [
                    new SKPaint { IsAntialias = true, Color = new(0, 255, 0), Style = SKPaintStyle.Stroke, StrokeWidth = 1 },
                    new SKPaint { IsAntialias = true, Color = new(0, 255, 0), Style = SKPaintStyle.Stroke, StrokeWidth = 3 }
                ],
                Type = CircuitItemType.VoltageOn,
                Inputs = new List<Port>(),
                Outputs = new List<Port> { new Port { Position = output, Value = true } }
            };
        }

        void IDrawer.SetPotencialConnection(SKPoint potentialConnectionPoint)
        {
            using var builder = new SKPathBuilder();
            List<SKPath> result = new();
            builder.AddCircle(
                potentialConnectionPoint.X,
                potentialConnectionPoint.Y,
                30f
            );

            signalForPotencialConnection = new CircuitItem
            {
                Position = potentialConnectionPoint,
                Figures = [builder.Snapshot()],
                Paints = [new SKPaint { IsAntialias = true, Color = new(0, 0, 255), Style = SKPaintStyle.Stroke, StrokeWidth = 5 }]
            };
        }
    }
}
