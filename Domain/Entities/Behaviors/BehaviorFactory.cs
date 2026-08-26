using Domain.Entities.Behaviors.Interfaces;
using Domain.Enums;

namespace Domain.Entities.Behaviors
{
    public static class BehaviorFactory
    {
        public static IBehavior? Create(CircuitItemType type)
        {
            return BehaviorTypeParser.Parse(type);
        }
    }
}
