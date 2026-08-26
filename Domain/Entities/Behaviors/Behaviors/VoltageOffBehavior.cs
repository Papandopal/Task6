using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Behaviors.Interfaces;

namespace Domain.Entities.Behaviors.Behaviors
{
    public class VoltageOffBehavior : IBehavior
    {
        int connectedInputsCountForWork = 0;
        int IBehavior.ConnectedInputsCountForWork => connectedInputsCountForWork;
        bool IBehavior.Invoke(CircuitItem item)
        {
            return true;
        }
    }
}
