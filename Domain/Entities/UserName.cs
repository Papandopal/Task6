using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public record UserName
    {
        public string Name { get; set; } = string.Empty;
        public int Postfix { get; set; } = 0;
    }
}
