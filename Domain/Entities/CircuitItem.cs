using System.Text.Json.Serialization;
using Domain.Entities.Behaviors;
using Domain.Entities.Behaviors.Interfaces;
using Domain.Enums;
using SkiaSharp;

namespace Domain.Entities
{
    public class CircuitItem : IDisposable
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public SKPoint Position { get; set; }
        public List<SKPath> Figures { get; set; } = new();
        public List<SKPaint> Paints { get; set; } = new();
        public CircuitItemType Type { get; set; }
        public IBehavior Behavior { get; set; }
        public List<Port> Inputs { get; set; } = new();
        public List<Port> Outputs { get; set; } = new();
        public Dictionary<SKPoint, CircuitItem> InputConnectedItems { get; set; } = new();
        public Dictionary<SKPoint, List<CircuitItem>> OutputConnectedItems { get; set; } = new();

        public bool Invoke()
        {
            return Behavior.Invoke(this);
        }

        public void NotifyNextItems()
        {
            Behavior.NotifyNextItems(this);
        }

        public void Dispose()
        {
            Figures.ForEach(x=>x.Dispose());
            Paints.ForEach(x => x.Dispose());
            foreach (var item in InputConnectedItems)
            {
                foreach(var point in Inputs)
                {
                    if (item.Value.OutputConnectedItems.ContainsKey(point.Position))
                        item.Value.OutputConnectedItems[point.Position].Remove(this);
                }
            }
            foreach (var itemList in OutputConnectedItems)
            {
                foreach(var item in itemList.Value)
                {
                    item.InputConnectedItems.Remove(item.InputConnectedItems.First(x => x.Value == this).Key);
                }
            }
        }
    }
}
