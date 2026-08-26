namespace Domain.Entities.Behaviors.Interfaces
{
    public interface IBehavior
    {
        int ConnectedInputsCountForWork { get; }    
        public bool Invoke(CircuitItem item);
        public virtual void NotifyNextItems(CircuitItem item)
        {
            for (int i = 0; i < item.Outputs.Count; i++)
            {
                Port port = item.Outputs[i];

                if (item.OutputConnectedItems.ContainsKey(port.Position))
                {
                    var connectedItem = item.OutputConnectedItems[port.Position];

                    var input = connectedItem.Inputs.FirstOrDefault(x => x.Position == port.Position);

                    if (input is not null)
                    {
                        input.Value = port.Value;
                    }
                }
            }
            foreach(var connectedItem in item.OutputConnectedItems.Values){
                var success = connectedItem.Invoke();
                if (success) connectedItem.NotifyNextItems();
            }
        }
    }
}
