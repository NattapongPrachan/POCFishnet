using Grandora;
using Grandora.Network;

public interface ILokiListener
{
    bool IsInit { get; set; }
    LokiBehaviour LokiBehaviour { get; set; }
    void RegisterComponentWhenAssigned();
    void UnregisterComponentWhenRemoved(); 
    void OnObjectBehaviourAdded(ObjectBehaviour objectBehaviour);
    void OnNetworkObjectBehaviourAdded(NetworkObjectBehaviour networkObjectBehaviour);
    void OnObjectBehaviourRemoved(ObjectBehaviour objectBehaviour);
    void OnNetworkObjectBehaviourRemoved(NetworkObjectBehaviour networkObjectBehaviour);
}
