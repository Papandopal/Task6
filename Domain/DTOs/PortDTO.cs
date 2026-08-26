using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public record PortDTO : IParsable<PortDTO>
    {
        public PointDTO Position { get; set; }
        public bool Value { get; set; }

        public static PortDTO Parse(string s, IFormatProvider? provider)
        {
            var parts = s.Split(';');
            if (parts.Length == 2)
            {
                var res = new PortDTO();
                res.Position = PointDTO.Parse(parts[0], provider);
                res.Value = bool.Parse(parts[1]);
                return res;
            }
            throw new ArgumentException("parsing failed");
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out PortDTO result)
        {
            if (s is null)
            {
                result = null;
                return false;
            }
            var parts = s.Split(';');
            if (parts.Length == 2)
            {
                result = Parse(s, provider);
                return true;
            }
            result = null;
            return false;
        }

        public override string ToString()
        {
            return $"{Position};{Value}";
        }
    }
}
