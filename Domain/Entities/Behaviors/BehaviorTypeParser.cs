using Domain.Entities.Behaviors.Behaviors;
using Domain.Entities.Behaviors.Interfaces;
using Domain.Enums;

namespace Domain.Entities.Behaviors
{
    public static class BehaviorTypeParser
    {
        public static IBehavior? Parse(CircuitItemType type)
        {
            return type switch
            {
                CircuitItemType.And => new AndBehavior(),
                CircuitItemType.Nand => new NandBehavior(),
                CircuitItemType.Nor => new NorBehavior(),
                CircuitItemType.Not => new NotBehavior(),
                CircuitItemType.Or => new OrBehavior(),
                CircuitItemType.Wire => new WireBehavior(),
                CircuitItemType.Xor => new XorBehavior(),
                CircuitItemType.VoltageOn => new VoltageOnBehavior(),
                CircuitItemType.VoltageOff => new VoltageOffBehavior(),
                CircuitItemType.Lamp => new LampBehavior(),
                _=>null
            };
        }
    }
}
