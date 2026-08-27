using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using SkiaSharp;
using Task6Itransition.Services.Drawers.Interfaces;

namespace Task6Itransition.Services.Drawers.Drawers
{
    public class WireDrawer : IDrawer
    {
        private bool isComplete = false;
        private SKPoint? start = null;
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

        void IDrawer.MouseClick(MouseClickModel model)
        {
            if (start is null)
            {
                start = new SKPoint(model.X, model.Y);
                isComplete = false;
                finalItem = null;
                previewItem?.Dispose();
                previewItem = null;
            }
            else
            {
                var end = new SKPoint(model.X, model.Y);
                using var builder = new SKPathBuilder();
                builder.MoveTo(start.Value);
                builder.LineTo(end);
                finalItem = new CircuitItem
                {
                    Position = new SKPoint((end.X - start.Value.X) / 2, (end.Y - start.Value.Y) / 2),
                    Figures = [builder.Snapshot()],
                    Paints = [new SKPaint { IsAntialias = true, Color = new(255, 0, 0), Style = SKPaintStyle.Stroke, StrokeWidth = 5 }],
                    Type = CircuitItemType.Wire,
                    Inputs = new List<Port> { new Port { Position = start.Value, Value = false } },
                    Outputs = new List<Port> { new Port { Position = end, Value = false } }
                };
                isComplete = true;
                start = null;
            }
        }

        void IDrawer.MouseMove(MouseMoveModel model)
        {
            if (start is not null)
            {
                var end = new SKPoint(model.X, model.Y);
                using var builder = new SKPathBuilder();
                builder.MoveTo(start.Value);
                builder.LineTo(end);
                previewItem?.Dispose();
                previewItem = new CircuitItem
                {
                    Position = new SKPoint((end.X - start.Value.X) / 2, (end.Y - start.Value.Y) / 2),
                    Figures = [builder.Snapshot()],
                    Paints = [new SKPaint { IsAntialias = true, Color = new(0, 255, 0), Style = SKPaintStyle.Stroke, StrokeWidth = 5 }],
                    Type = CircuitItemType.Wire,
                    Inputs = new List<Port> { new Port { Position = start.Value, Value = false } },
                    Outputs = new List<Port> { new Port { Position = end, Value = false } }
                };
            }
            else
            {
                using var builder = new SKPathBuilder();
                builder.AddCircle(model.X, model.Y, 1);
                var point = new SKPoint(model.X, model.Y);
                previewItem?.Dispose();
                previewItem = new CircuitItem
                {
                    Position = point,
                    Figures = [builder.Snapshot()],
                    Paints = [new SKPaint { IsAntialias = true, Color = new(0, 0, 0), Style = SKPaintStyle.Stroke, StrokeWidth = 5 }],
                    Type = CircuitItemType.Wire,
                    Inputs = new List<Port> { new Port { Position = point, Value = false } },
                    Outputs = new List<Port> { }
                };
            }
        }
        void IDrawer.SetPotencialConnection(SKPoint potentialConnectionPoint)
        {
            using var builder = new SKPathBuilder();

            builder.AddCircle(
                potentialConnectionPoint.X,
                potentialConnectionPoint.Y,
                30f
            );

            signalForPotencialConnection = new CircuitItem
            {
                Position = potentialConnectionPoint,
                Figures = [builder.Snapshot()],
                Paints = [new SKPaint { IsAntialias = true, Color = new(0, 0, 255), Style = SKPaintStyle.Stroke, StrokeWidth = 3 }],
                Type = CircuitItemType.Wire
            };

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
    }
}
