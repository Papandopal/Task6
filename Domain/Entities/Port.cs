using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace Domain.Entities
{
    public class Port
    {
        public SKPoint Position { get; set; }
        public bool Value { get; set; }
    }
}
