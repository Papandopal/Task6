using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace Domain.Entities
{
    public class UserCursor
    {
        public UserName UserName { get; set; }
        public SKPoint Position { get; set; }
    }
}
