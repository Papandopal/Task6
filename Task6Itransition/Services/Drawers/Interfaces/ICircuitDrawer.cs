using System.Drawing;
using System.Reflection;
using Domain.Enums;
using SkiaSharp;

namespace Task6Itransition.Services.Drawers.Interfaces
{
    public interface ICircuitDrawer
    {
        public List<SKPath> GetBasedCircuit(SKPoint center, float halfSquareSideSize, int inputsCount, int outputsCount, float portsRadius,
            int textSize, CircuitItemType circuitType, out List<SKPoint> inputs, out List<SKPoint> outputs)
        {
            using var builder = new SKPathBuilder();
            List<SKPath> result = new();
            inputs = new();
            outputs = new();

            for (int i = 1; i <= inputsCount; i++)
            {
                var point = new SKPoint(center.X - halfSquareSideSize, center.Y + halfSquareSideSize - 2 * halfSquareSideSize / (inputsCount + 1) * i);
                inputs.Add(point);
            }

            for (int i = 1; i <= outputsCount; i++)
            {
                var point = new SKPoint(center.X + halfSquareSideSize, center.Y + halfSquareSideSize - 2 * halfSquareSideSize / (outputsCount + 1) * i);
                outputs.Add(point);
            }

            var rect = SKRect.Create(center.X - halfSquareSideSize,
                center.Y - halfSquareSideSize,
                2 * halfSquareSideSize,
                2 * halfSquareSideSize);

            builder.AddRect(rect);

            foreach (SKPoint point in inputs)
            {
                builder.AddCircle(point.X, point.Y, portsRadius);
            }

            foreach (SKPoint point in outputs)
            {
                builder.AddCircle(point.X, point.Y, portsRadius);
            }

            result.Add(builder.Detach());

            SKPath textPath = new SKFont(SKTypeface.Default, textSize).GetTextPath(circuitType.ToString());
            SKRect bounds = textPath.Bounds;
            textPath.Offset(center.X - bounds.MidX, center.Y - bounds.MidY);
            builder.AddPath(textPath);
            result.Add(builder.Detach());

            return result;
        }

        public List<SKPaint> GetBasedCircuitPaint()
        {
            List<SKPaint> result =
            [
                new SKPaint { IsAntialias = true, Color = new(255, 0, 0), Style = SKPaintStyle.Stroke, StrokeWidth = 3 },
                new SKPaint { IsAntialias = true, Color = new(0, 0, 0), Style = SKPaintStyle.StrokeAndFill, StrokeWidth = 1 },
            ];
            return result;
        }

        public List<SKPaint> GetBasedPreviewPaint()
        {
            List<SKPaint> result =
            [
                new SKPaint { IsAntialias = true, Color = new(0, 255, 0), Style = SKPaintStyle.Stroke, StrokeWidth = 3 },
                new SKPaint { IsAntialias = true, Color = new(0, 255, 0), Style = SKPaintStyle.StrokeAndFill, StrokeWidth = 1 },
            ];
            return result;
        }

        public List<SKPath> GetBasedSignalForPotencialConnection(SKPoint potentialConnectionPoint)
        {
            using var builder = new SKPathBuilder();
            List<SKPath> result = new();
            builder.AddCircle(
                potentialConnectionPoint.X,
                potentialConnectionPoint.Y,
                30f
            );
            result.Add(builder.Detach());
            return result;
        }

        public List<SKPaint> GetBasedSignalForPotencialConnectionPaint()
        {
            List<SKPaint> result =
            [
                new SKPaint { IsAntialias = true, Color = new(0, 0, 255), Style = SKPaintStyle.Stroke, StrokeWidth = 3 }
            ];
            return result;
        }
    }
}
