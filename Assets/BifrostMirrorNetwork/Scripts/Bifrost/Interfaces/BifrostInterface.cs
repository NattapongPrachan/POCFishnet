using System;
using System.Collections;
using System.Collections.Generic;
using Grandora.Bifrost;
using UnityEngine;

public class BifrostInterface
{

}
public interface IBifrostInitialize
{
    void OnCreate();
}
public interface IBifrostOperation : IBifrostInitialize
{
    
}
public interface IBifrostUpdate
{
    void OnUpdate();
    void OnFixedUpdate();
    void OnLateUpdate();
}
public interface IBifrostListener
{
    void RegisterComponentWhenAssigned();
    void UnRegisterComponentWhenRemoved();
    void OnBifrostAdded(BifrostBridge bifrostBridge);
    void OnBifrostRemoved(BifrostBridge bifrostBridge);
}
public interface IBifrostNetwork
{
    void OnStartServer();
    void OnStopClient();
    void OnStartNetwork();
    void OnStopNetwork();
    void OnStopServer();
    void OnDespawnServer(FishNet.Connection.NetworkConnection connection);
    void OnOwnershipClient(FishNet.Connection.NetworkConnection connection);
    void OnOwnershipServer(FishNet.Connection.NetworkConnection connection);
    void OnSpawnServer(FishNet.Connection.NetworkConnection connection);
    void Reset();
    void OnValidate();
}
