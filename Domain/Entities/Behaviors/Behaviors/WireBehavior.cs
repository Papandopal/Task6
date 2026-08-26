using Domain.Entities.Behaviors.Interfaces;

namespace Domain.Entities.Behaviors.Behaviors
{
    public class WireBehavior : IBehavior
    {
        int connectedInputsCountForWork = 1;
        int IBehavior.ConnectedInputsCountForWork => connectedInputsCountForWork;
        bool IBehavior.Invoke(CircuitItem item)
        {
            if (item.Inputs.Count > connectedInputsCountForWork) throw new Exception("Wire can't have many inputs");
            else if (item.Inputs.Count != connectedInputsCountForWork) return false;
            foreach (var output in item.Outputs)
            {
                output.Value = item.Inputs[0].Value;
            }
            return true;
        }
    }
}
