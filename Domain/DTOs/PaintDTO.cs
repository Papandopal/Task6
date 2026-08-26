using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace Domain.DTOs
{
    public record PaintDTO : IParsable<PaintDTO>
    {
        public string ColorHex { get; set; } = "#000000";
        public float StrokeWidth { get; set; }
        public bool IsAntialias { get; set; }
        public SKPaintStyle Style { get; set; } = SKPaintStyle.Stroke;

        public static PaintDTO Parse(string s, IFormatProvider? provider)
        {
            var parts = s.Split(';');
            if(parts.Length == 3)
            {
                var res = new PaintDTO();
                res.ColorHex = parts[0];
                res.StrokeWidth = float.Parse(parts[1]);
                res.Style = Enum.Parse<SKPaintStyle>(parts[2]);
                res.IsAntialias = bool.Parse(parts[3]);
                return res;
            }
            throw new ArgumentException("parsing failed");
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out PaintDTO result)
        {
            if (s is null)
            {
                result = null;
                return false;
            }
            var parts = s.Split(';');
            if (parts.Length == 4)
            {
                result = Parse(s, provider);
                return true;
            }
            result = null;
            return false;
        }

        public override string ToString()
        {
            return $"{ColorHex};{StrokeWidth};{Style};{IsAntialias}";
        }
    }
}
