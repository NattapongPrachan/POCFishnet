using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using FishNet.Managing;
using FishNet.Transporting.UTP;
using QFSW.QC;
using UniRx;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class TestRelay : MonoInstance<TestRelay>
{
    [SerializeField]NetworkManager _networkManager;
    // async void Start()
    // {
    //     await UnityServices.InitializeAsync();
    //     AuthenticationService.Instance.SignedIn += () =>{
    //         Debug.Log("Signed in "+AuthenticationService.Instance.PlayerId);
    //     };
    //     await AuthenticationService.Instance.SignInAnonymouslyAsync();
    // }
    public static Subject<Unit> OnStartConnection = new Subject<Unit>();
    private void Start()
    {
        
    }
    [Command]
    public async UniTask<string> StartHost()
    {
        var utp = (FishyUnityTransport)_networkManager.TransportManager.Transport;
        Allocation hostAllocation = await RelayService.Instance.CreateAllocationAsync(8);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(hostAllocation.AllocationId);
        Debug.Log(joinCode);
        utp.SetRelayServerData(new RelayServerData(hostAllocation,"dtls"));
        _networkManager.ServerManager.StartConnection();
        _networkManager.ClientManager.StartConnection();
        OnStartConnection.OnNext(default);
        return joinCode;
    }
    [Command]
    public async UniTask StartClient(string joinCode)
    {
        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        var utp = (FishyUnityTransport)_networkManager.TransportManager.Transport;
        utp.SetRelayServerData(new RelayServerData(joinAllocation,"dtls"));
        _networkManager.ClientManager.StartConnection();
        OnStartConnection.OnNext(default);
    }
    [Command]
    public async UniTask<string> CreateRelay()
    {
        try{
            //netcode
            //    Allocation allocation =  await RelayService.Instance.CreateAllocationAsync(8);
            //    string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            //    Debug.Log(joinCode);
            var utp = (FishyUnityTransport)_networkManager.TransportManager.Transport;
            Allocation hostAllocation = await RelayService.Instance.CreateAllocationAsync(8);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(hostAllocation.AllocationId);
            utp.SetRelayServerData(new RelayServerData(hostAllocation,"dtls"));
            return joinCode;

        }catch(RelayServiceException e)
        {
            Debug.Log(e);
            return "";
        }
    }
    [Command]
    public async void JoinRelay(string joinCode)
    {
        try
        {
            // netcode ====>  await RelayService.Instance.JoinAllocationAsync(joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            var utp = (FishyUnityTransport)_networkManager.TransportManager.Transport;
            utp.SetRelayServerData(new RelayServerData(joinAllocation,"dtls"));
        }catch(RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
}
