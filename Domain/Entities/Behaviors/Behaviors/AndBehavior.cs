using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Behaviors.Interfaces;

namespace Domain.Entities.Behaviors.Behaviors
{
    public class AndBehavior : IBehavior
    {
        int connectedInputsCountForWork = 2;
        int IBehavior.ConnectedInputsCountForWork => connectedInputsCountForWork;

        bool IBehavior.Invoke(CircuitItem item)
        {
            if (item.InputConnectedItems.Count == connectedInputsCountForWork)
            {
                var first  = item.Inputs.ElementAt(0);
                var second = item.Inputs.ElementAt(1);

                item.Outputs[0].Value = first.Value && second.Value;
                return true;
            }
            return false;
        }
    }
}
