using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Behaviors.Interfaces;
using SkiaSharp;

namespace Domain.Entities.Behaviors.Behaviors
{
    public class LampBehavior : IBehavior
    {
        int connectedInputsCountForWork = 1;
        int IBehavior.ConnectedInputsCountForWork => connectedInputsCountForWork;
        bool IBehavior.Invoke(CircuitItem item)
        {
            if (item.Inputs[0].Value) item.Paints[1] = new SKPaint { IsAntialias = true, Color = new(255, 255, 0), Style = SKPaintStyle.Fill};
            else item.Paints[1] = new SKPaint { IsAntialias = true, Color = new(255, 255, 255), Style = SKPaintStyle.Fill};
            return true;
        }
    }
}
