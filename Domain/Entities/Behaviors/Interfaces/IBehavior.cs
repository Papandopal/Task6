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
                if(item.OutputConnectedItems.ContainsKey(port.Position))
                    item.OutputConnectedItems[port.Position].Inputs.First(x => x.Position == port.Position).Value = port.Value;
            }
            foreach(var connectedItem in item.OutputConnectedItems.Values){
                var success = connectedItem.Invoke();
                if (success) connectedItem.NotifyNextItems();
            }
        }
    }
}
