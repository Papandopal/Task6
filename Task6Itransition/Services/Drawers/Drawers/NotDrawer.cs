using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using SkiaSharp;
using Task6Itransition.Services.Drawers.Interfaces;

namespace Task6Itransition.Services.Drawers.Drawers
{
    public class NotDrawer : IDrawer, ICircuitDrawer
    {
        private bool isComplete = false;
        private CircuitItem? finalItem = null;
        private CircuitItem? previewItem = null;
        private CircuitItem? signalForPotencialConnection = null;
        bool IDrawer.IsComplete => isComplete;

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
            List<SKPoint> inputs = new();
            List<SKPoint> outputs = new();
            finalItem = new CircuitItem
            {
                Position = new SKPoint(model.X, model.Y),
                Figures = ((ICircuitDrawer)this).GetBasedCircuit(new SKPoint(model.X, model.Y), 40, 1, 1, 10, 16, CircuitItemType.Not, out inputs, out outputs),
                Paints = ((ICircuitDrawer)this).GetBasedCircuitPaint(),
                Type = CircuitItemType.Not,
                Inputs = inputs.Select(x => new Port { Position = x, Value = false }).ToList(),
                Outputs = outputs.Select(x => new Port { Position = x, Value = false }).ToList()
            };

            isComplete = true;
        }

        void IDrawer.MouseMove(MouseMoveModel model)
        {
            List<SKPoint> inputs = new();
            List<SKPoint> outputs = new();

            previewItem?.Dispose();

            previewItem = new CircuitItem
            {
                Position = new SKPoint(model.X, model.Y),
                Figures = ((ICircuitDrawer)this).GetBasedCircuit(new SKPoint(model.X, model.Y), 40, 1, 1, 10, 16, CircuitItemType.Not, out inputs, out outputs),
                Paints = ((ICircuitDrawer)this).GetBasedPreviewPaint(),
                Type = CircuitItemType.Not,
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
                Paints = ((ICircuitDrawer)this).GetBasedSignalForPotencialConnectionPaint(),
            };
        }
        void IDrawer.DropPotencialConnection()
        {
            signalForPotencialConnection?.Dispose();
            signalForPotencialConnection = null;
        }
    }
}
