using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public record PointDTO : IParsable<PointDTO>
    {
        public float X { get; set; }
        public float Y { get; set; }

        public static PointDTO Parse(string s, IFormatProvider? provider = null)
        {
            var parts = s.Split(';');
            if (parts.Length == 2)
            {
                var res = new PointDTO();
                res.X = float.Parse(parts[0]);
                res.Y = float.Parse(parts[1]);
                return res;
            }
            throw new ArgumentException("parsing faild");
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out PointDTO result)
        {
            if(s is null)
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
            return $"{X};{Y}";
        }
    }
}
