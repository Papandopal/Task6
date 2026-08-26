using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.DTOs;
using Domain.Entities;
using Domain.Entities.Behaviors;
using SkiaSharp;

namespace Domain
{
    public static class Serialiser
    {
        public static PaintDTO GetPaintDTO(SKPaint paint)
        {
            return new PaintDTO()
            {
                ColorHex = paint.Color.ToString(),
                StrokeWidth = paint.StrokeWidth,
                IsAntialias = paint.IsAntialias,
                Style = paint.Style
            };
        }

        public static SKPaint GetPaint(PaintDTO dto)
        {
            return new SKPaint()
            {
                Color = SKColor.Parse(dto.ColorHex),
                StrokeWidth = dto.StrokeWidth,
                IsAntialias = dto.IsAntialias,
                Style = dto.Style
            };
        }

        public static PointDTO GetPointDTO(SKPoint point)
        {
            return new PointDTO() { X = point.X, Y = point.Y };
        }

        public static SKPoint GetPoint(PointDTO pointDTO)
        {
            return new SKPoint() { X = pointDTO.X, Y = pointDTO.Y };
        }

        public static PortDTO GetPortDTO(Port port)
        {
            return new PortDTO() { Position = GetPointDTO(port.Position), Value = port.Value };
        }

        public static Port GetPort(PortDTO dto)
        {
            return new Port { Position = GetPoint(dto.Position), Value = dto.Value };
        }

        public static CircuitItemDTO GetItemDTO(CircuitItem item)
        {
            return new CircuitItemDTO()
            {
                Position = GetPointDTO(item.Position),
                Figures = item.Figures.Select(x=>x.ToSvgPathData()).ToList(),
                Paints = item.Paints.Select(x=>GetPaintDTO(x)).ToList(),
                //InputConnectedItems = item.InputConnectedItems.ToDictionary(x => GetPointDTO(x.Key).ToString(), x => GetItemDTO(x.Value)),
                Inputs = item.Inputs.Select(x=>GetPortDTO(x)).ToList(),
                //OutputConnectedItems = item.OutputConnectedItems.ToDictionary(x => GetPointDTO(x.Key).ToString(), x => GetItemDTO(x.Value)),
                Outputs = item.Outputs.Select(x => GetPortDTO(x)).ToList(),
                Type = item.Type
            };
        }

        public static CircuitItem GetItem(CircuitItemDTO dto)
        {
            return new CircuitItem()
            {
                Position = GetPoint(dto.Position),
                Figures = dto.Figures.Select(x=>SKPath.ParseSvgPathData(x)).ToList(),
                Paints = dto.Paints.Select(x => GetPaint(x)).ToList(),
                Behavior = BehaviorFactory.Create(dto.Type),
                //InputConnectedItems = dto.InputConnectedItems.ToDictionary(x => GetPoint(PointDTO.Parse(x.Key)), x => GetItem(x.Value)),
                Inputs = dto.Inputs.Select(x=>GetPort(x)).ToList(),
                //OutputConnectedItems = dto.OutputConnectedItems.ToDictionary(x => GetPoint(PointDTO.Parse(x.Key)), x => GetItem(x.Value)),
                Outputs = dto.Outputs.Select(x => GetPort(x)).ToList(),
                Type = dto.Type
            };
        }
    }
}
