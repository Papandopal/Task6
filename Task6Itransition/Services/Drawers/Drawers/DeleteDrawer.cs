using Domain.Entities;
using Domain.Models;
using SkiaSharp;
using Task6Itransition.Services.Drawers.Interfaces;

namespace Task6Itransition.Services.Drawers.Drawers
{
    public class DeleteDrawer : IDrawer
    {
        private bool isComplete = false;
        private SKPoint? start;
        private CircuitItem? previewItem = null;
        private CircuitItem? finalItem = null;  
        private SKRect? finalRect = null;   

        public bool IsComplete => isComplete;

        void IDisposable.Dispose()
        {
            finalItem?.Dispose();
            previewItem?.Dispose();
        }

        public CircuitItem? GetItem() => null; 

        public CircuitItem? GetPreviewItem()
        {
            return previewItem;
        }

        public void MouseClick(MouseClickModel model)
        {
            if (start is null)
            {
                start = new SKPoint(model.X, model.Y);
                isComplete = false;
            }
            else
            {
                using var builder = new SKPathBuilder();

                var rect = SKRect.Create(
                    Math.Min(start.Value.X, model.X),
                    Math.Min(start.Value.Y, model.Y),
                    Math.Abs(start.Value.X - model.X),
                    Math.Abs(start.Value.Y - model.Y));

                //builder.AddRect(rect);
                finalRect = rect;
                finalItem = new CircuitItem
                {
                    //Figures = [builder.Snapshot()]
                };
                isComplete = true;
                start = null;
            }
        }

        public void MouseMove(MouseMoveModel model)
        {
            if (start is not null)
            {
                using var builder = new SKPathBuilder();

                var rect = SKRect.Create(
                    Math.Min(start.Value.X, model.X),
                    Math.Min(start.Value.Y, model.Y),
                    Math.Abs(start.Value.X - model.X),
                    Math.Abs(start.Value.Y - model.Y));

                builder.AddRect(rect);

                previewItem?.Dispose();

                previewItem = new CircuitItem
                {
                    Figures = [builder.Snapshot()],
                    Paints = [new SKPaint { IsAntialias = true, Color = new(0, 255, 0), Style = SKPaintStyle.Stroke, StrokeWidth = 3 }]
                };
            }
        }

        public void DropPotencialConnection() { }
        public CircuitItem? GetSignalForPotencialConnection() => null;
        public void SetPotencialConnection(SKPoint point) { }

        public SKRect? GetRect()
        {
            return finalRect;
        }
    }
}
