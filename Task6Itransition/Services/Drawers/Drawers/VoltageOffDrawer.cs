using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using SkiaSharp;
using Task6Itransition.Services.Drawers.Interfaces;

namespace Task6Itransition.Services.Drawers.Drawers
{
    public class VoltageOffDrawer : IDrawer
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
            int outputRadius = 10;
            int halfSquareSideSize = 15;
            int textSize = 16;
            SKPoint output = new SKPoint(model.X, model.Y + outputRadius + halfSquareSideSize);

            var offFont = new SKFont(SKTypeface.Default, textSize).GetTextPath("0");
            var bounds = offFont.Bounds;
            offFont.Offset(model.X - bounds.MidX, model.Y - bounds.MidY);
            builder.AddPath(offFont);
            figure.Add(builder.Detach());

            var rect = SKRect.Create(model.X - halfSquareSideSize,
                model.Y - halfSquareSideSize,
                2 * halfSquareSideSize,
                2 * halfSquareSideSize);

            builder.AddRect(rect);

            builder.AddCircle(output.X, output.Y, outputRadius);

            SKPath off = builder.Detach();
            figure.Add(off);

            finalItem = new CircuitItem
            {
                Position = new SKPoint(model.X, model.Y),
                Figures = figure,
                Paints =
                [
                    new SKPaint { IsAntialias = true, Color = new(0, 0, 0), Style = SKPaintStyle.Stroke, StrokeWidth = 1 },
                    new SKPaint { IsAntialias = true, Color = new(255, 255, 0), Style = SKPaintStyle.Stroke, StrokeWidth = 3 }
                ],
                Type = CircuitItemType.VoltageOff,
                Inputs = new List<Port>(),
                Outputs = new List<Port> { new Port { Position = output, Value = false } }
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

            var offFont = new SKFont(SKTypeface.Default, textSize).GetTextPath("0");
            var bounds = offFont.Bounds;
            offFont.Offset(model.X - bounds.MidX, model.Y - bounds.MidY);
            builder.AddPath(offFont);
            figure.Add(builder.Detach());

            var rect = SKRect.Create(model.X - halfSquareSideSize,
                model.Y - halfSquareSideSize,
                2 * halfSquareSideSize,
                2 * halfSquareSideSize);

            builder.AddRect(rect);

            builder.AddCircle(output.X, output.Y, outputRadius);

            SKPath off = builder.Detach();
            figure.Add(off);

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
                Type = CircuitItemType.VoltageOff,
                Inputs = new List<Port>(),
                Outputs = new List<Port> { new Port { Position = output, Value = false } }
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
