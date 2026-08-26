using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using SkiaSharp;

namespace Domain.DTOs
{
    public record CircuitItemDTO
    {
        public Guid Id { get; init; }
        public PointDTO Position { get; set; }
        public List<string> Figures { get; set; } = new();
        public List<PaintDTO> Paints { get; set; } = new();
        public CircuitItemType Type { get; set; }
        public List<PortDTO> Inputs { get; set; } = new();
        public List<PortDTO> Outputs { get; set; } = new();
    }
}
