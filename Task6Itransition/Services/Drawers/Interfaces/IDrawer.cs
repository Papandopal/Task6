using Domain.Entities;
using Domain.Models;
using SkiaSharp;

namespace Task6Itransition.Services.Drawers.Interfaces
{
    public interface IDrawer
    {
        public bool IsComplete { get; }
        public void MouseClick(MouseClickModel model);
        public void MouseMove(MouseMoveModel model);
        public void SetPotencialConnection(SKPoint potentialConnectionPoint);
        public void DropPotencialConnection();
        public CircuitItem? GetItem();
        public CircuitItem? GetPreviewItem();
        public CircuitItem? GetSignalForPotencialConnection();
    }
}
