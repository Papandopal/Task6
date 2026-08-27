using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using Task6Itransition.Services.Drawers.Drawers;

namespace Task6Itransition.Services.Drawers
{
    public static class DrawerTypeParser
    {
        private static Dictionary<CircuitItemType, Type> pairs = new Dictionary<CircuitItemType, Type>
        {
            { CircuitItemType.And, typeof(AndDrawer) } ,
            { CircuitItemType.Nand, typeof(NandDrawer) } ,
            { CircuitItemType.Nor, typeof(NorDrawer) },
            { CircuitItemType.Not, typeof(NotDrawer) } ,
            { CircuitItemType.Or, typeof(OrDrawer) } ,
            { CircuitItemType.Wire, typeof(WireDrawer) } ,
            { CircuitItemType.Xor, typeof(XorDrawer) },
            { CircuitItemType.VoltageOff, typeof(VoltageOffDrawer) },
            { CircuitItemType.VoltageOn, typeof(VoltageOnDrawer) },
            { CircuitItemType.Lamp, typeof(LampDrawer) },
            { CircuitItemType.Delete, typeof(DeleteDrawer) }
        };

        public static Type? Parse(string type)
        {
            if (Enum.TryParse<CircuitItemType>(type, true, out var key) && pairs.ContainsKey(key)) { return pairs[key]; }
            else return null;
        }

    }
}
