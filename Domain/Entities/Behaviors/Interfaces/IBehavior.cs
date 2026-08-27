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
                    var connectedItems = item.OutputConnectedItems[port.Position];

                    foreach (var connectedItem in connectedItems)
                    {
                        var input = connectedItem.Inputs.FirstOrDefault(x => x.Position == port.Position);

                        if (input is not null)
                        {
                            input.Value = port.Value;
                        }
                    }
                }
            }
            foreach (var connectedItems in item.OutputConnectedItems.Values)
            {
                foreach (var connectedItem in connectedItems)
                {
                    var success = connectedItem.Invoke();
                    if (success) connectedItem.NotifyNextItems();
                }
            }
        }
    }
}
