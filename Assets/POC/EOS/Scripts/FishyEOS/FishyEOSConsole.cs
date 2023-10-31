using System;
using System.Collections;
using System.Collections.Generic;
using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Connect;
using Epic.OnlineServices.Lobby;
using Epic.OnlineServices.P2P;
using FishNet;
using FishNet.Managing;
using FishNet.Plugins.FishyEOS.Util;
using FishNet.Plugins.FishyEOS.Util.Coroutines;
using FishNet.Transporting;
using FishNet.Transporting.FishyEOSPlugin;
using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;
using QFSW.QC;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.AI;
using static PlayEveryWare.EpicOnlineServices.EOSManager;

public class FishyEOSConsole : MonoBehaviour
{
    [SerializeField]NetworkManager _networkManager;
    [SerializeField]FishyEOS _fishyEOS;
    [SerializeField]EOSManager _eosManager;
    [SerializeField]EOSLobbyManager _eosLobbyManager;

    [SerializeField]GameObject[] gameObjects;

    [SerializeField]AuthDataLogin dataLogin;
    [SerializeField]Lobby _lobby;

    [SerializeField]LobbySearch CurrentSearch;
    private Dictionary<Lobby, LobbyDetails> SearchResults = new Dictionary<Lobby, LobbyDetails>();

    [SerializeField] LoginCredentialType credentialType;
    [SerializeField] string port;
    [SerializeField] string credentialId;

    [SerializeField] ExternalCredentialType externalCredentialType;
    [SerializeField] string token;
    [SerializeField] string displayName;
    [SerializeField] bool initialized;

    [SerializeField] TMP_InputField input_roomId;
    private void Awake()
    {
        Application.targetFrameRate = 60;
        _networkManager = InstanceFinder.NetworkManager;
        _fishyEOS = _networkManager.GetComponent<FishyEOS>();
        _eosManager = EOS.GetManager();
        _fishyEOS.Initialize(_networkManager,0);
        _fishyEOS.OnClientConnectionState += OnClientConnectionState;
        _fishyEOS.OnClientReceivedData += OnClientReceivedData;
        _fishyEOS.OnRemoteConnectionState += OnRemoteConnectionState;
        _fishyEOS.OnServerConnectionState += OnServerConnectionState;
        _fishyEOS.OnServerReceivedData += OnServerReceivedData;
        AddListener();
    }

    private void OnServerReceivedData(ServerReceivedDataArgs args)
    {
       // Debug.Log("OnServerReceivedData "+args);
    }

    private void OnServerConnectionState(ServerConnectionStateArgs args)
    {
        Debug.Log("OnServerConnectionState "+args.ConnectionState);
    }

    private void OnRemoteConnectionState(RemoteConnectionStateArgs args)
    {
        Debug.Log("OnRemoteConnectionState "+args.ConnectionState);
    }

    private void OnClientReceivedData(ClientReceivedDataArgs args)
    {
       // Debug.Log("OnClientReceivedData "+args);
    }

    private void OnClientConnectionState(ClientConnectionStateArgs args)
    {
        Debug.Log("OnClientConnectionState "+args.ConnectionState);
    }

    void Start()
    {
        foreach (var item in gameObjects)
        {
            item.SetActive(true);
        }
        //FConnect();
        // Debug.Log("productuserid "+EOS.LocalProductUserId);
    }
    [Command]
    public void AuthWithEpic()
    {
        var loginCredentials = new Epic.OnlineServices.Auth.Credentials
        {
            Type = credentialType,
            ExternalType = externalCredentialType,
            Id = port,
            Token = credentialId
        };

        var defaultScopeFlags = AuthScopeFlags.BasicProfile | AuthScopeFlags.FriendsList | AuthScopeFlags.Presence;

        var loginOptions = new Epic.OnlineServices.Auth.LoginOptions
        {
            Credentials = loginCredentials,
            ScopeFlags = defaultScopeFlags
        };

        EOS.GetCachedAuthInterface().Login(ref loginOptions, null, OnEpicAuthLogin);
    }

    private void OnEpicAuthLogin(ref Epic.OnlineServices.Auth.LoginCallbackInfo data)
    {
        Debug.Log("OnEpicAuthLogin " + data.ResultCode);
        Debug.Log("" + data.AccountFeatureRestrictedInfo.HasValue);
        switch(data.ResultCode)
        {
            case Result.Success:
                FConnectWithEpicId();
                break;
        }
    }
    [Command]
    public void FConnectWithEpicId()
    {

        //var localUserId = EOSManager.Instance.GetLocalUserId();
        //Token? authToken = EOSManager.Instance.GetUserAuthTokenForAccountId(localUserId)


        var epicAccountId = EOS.GetCachedAuthInterface().GetLoggedInAccountByIndex(0);

        CopyUserAuthTokenOptions copyUserAuthTokenOptions = new CopyUserAuthTokenOptions();
        var authToken = EOS.GetCachedAuthInterface().CopyUserAuthToken(ref copyUserAuthTokenOptions,epicAccountId,out Token? outUserAuthToken);

        var connectLoginOptions = new Epic.OnlineServices.Connect.LoginOptions();
        connectLoginOptions.Credentials = new Epic.OnlineServices.Connect.Credentials
        {
            Token = outUserAuthToken.Value.AccessToken,
            Type = ExternalCredentialType.Epic
        };
        EOS.GetCachedConnectInterface().Login(ref connectLoginOptions,null, OnLoginConnect);
    }
    [Command]
    public void FAuthLogin()
    {
        _fishyEOS.AuthConnectData.Connect(out AuthDataLogin authDataLogin);
        Debug.Log("authDataLogin "+authDataLogin);
        var loginCredentials = new Epic.OnlineServices.Auth.Credentials {
                    Type = _fishyEOS.AuthConnectData.loginCredentialType,
                    ExternalType = _fishyEOS.AuthConnectData.externalCredentialType,
                    Id = _fishyEOS.AuthConnectData.id,
                    Token = _fishyEOS.AuthConnectData.token
                };
        var defaultScopeFlags = AuthScopeFlags.BasicProfile | AuthScopeFlags.FriendsList | AuthScopeFlags.Presence;
        var authLoginOption = new Epic.OnlineServices.Auth.LoginOptions
        {
            Credentials = loginCredentials, 
            ScopeFlags = defaultScopeFlags
        };
        EOS.GetCachedAuthInterface().Login(ref authLoginOption,null,OnAuthLogin);
        
    }

    private void OnAuthLogin(ref Epic.OnlineServices.Auth.LoginCallbackInfo data)
    {
       Debug.Log("OnAuthLogin "+data.ResultCode);
       Debug.Log("PinGrantInfo.HasValue " + data.PinGrantInfo.HasValue);
        Debug.Log("data.LocalUserId" + data.LocalUserId);
        Debug.Log("data.ContinuanceToken "+data.ContinuanceToken);
        Debug.Log("data.AccountFeatureRestrictedInfo " + data.AccountFeatureRestrictedInfo.HasValue);
        Debug.Log("-----------------------------------------------------------");
    }
    [Command]
    public void StartConnectLoginWithEpicAccount()
    {
        //Token? authToken = EOSManager.Instance.GetUserAuthTokenForAccountId(EOS.GetPlat);
        //var connectLoginOptions = new Epic.OnlineServices.Connect.LoginOptions();
        //connectLoginOptions.Credentials = new Epic.OnlineServices.Connect.Credentials
        //{
        //    Token = authToken.Value.AccessToken,
        //    Type = ExternalCredentialType.Epic
        //};
    }
    [Command]
    public void FConnect()
    {
       // _fishyEOS.AuthConnectData.automaticallyCreateConnectAccount = true;
    
    //    // _fishyEOS.AuthConnectData.loginCredentialType.
    //     //Debug.Log("_fishyEOS.AutoAuthenticate "+_fishyEOS.AutoAuthenticate);
    //     //_fishyEOS.StartConnection(true);
        //_fishyEOS.AuthConnectData.Connect(out AuthDataLogin authDataLogin);
        var connectCredential = new Epic.OnlineServices.Connect.Credentials()
        {
            Type =  _fishyEOS.AuthConnectData.externalCredentialType,
             Token = _fishyEOS.AuthConnectData.token
        };
        var loginOptions = 
        new Epic.OnlineServices.Connect.LoginOptions() {
                Credentials = connectCredential,
                UserLoginInfo = new UserLoginInfo{
                    DisplayName = "Dielser"
                }
        };
        
        EOS.GetCachedConnectInterface().Login(ref loginOptions,null, OnLoginConnect);
    }

    

    private void OnQueryNATTypeComplete(ref OnQueryNATTypeCompleteInfo data)
    {
        Debug.Log("OnQueryNATTypeComplete "+data.ResultCode);
        Debug.Log("NATType "+data.NATType);
        
    }
    [Command]
    public void StartSingle()
    {
        _fishyEOS.StartConnection(true);
    }
    [Command]
    public void StartServer()
    {
        _networkManager.ServerManager.StartConnection();
        _networkManager.ClientManager.StartConnection();
    }
    [Command]
    public void StartClient()
    {
        Debug.Log("StartClient");
        Debug.Log("IsClient " + _networkManager.IsClient);
        Debug.Log("IsHost " + _networkManager.IsHost);
        Debug.Log("IsServer " + _networkManager.IsServer);
        _networkManager.ClientManager.StartConnection() ;
    }

    private void OnLoginConnect(ref Epic.OnlineServices.Connect.LoginCallbackInfo data)
    {
        Debug.Log("OnLoginConnect "+data.ResultCode);
        Debug.Log("_fishyEOS.LocalProductUserId "+_fishyEOS.LocalProductUserId);
        Debug.Log("EOS.LocalProductUserId "+EOS.LocalProductUserId);
        Debug.Log($"-------------------- Connect {data.ResultCode} ---------------------------");

        _fishyEOS.AuthConnectData.Connect(out AuthDataLogin authDataLogin);
        Debug.Log("authDataLogin " + authDataLogin.loginCallbackInfo);
        

        //EOS.GetPlatformInterface().GetLobbyInterface().CreateLobby(ref createLobbyOptions,null,OnCreateLobby);

    }
    [Command]
    public void FCreateLobby()
    {
        var createLobbyOptions = new CreateLobbyOptions{
            BucketId = "2",
            MaxLobbyMembers = 8,
            AllowInvites = true,
            EnableJoinById = true,
            LocalUserId = EOS.LocalProductUserId
        };
        EOS.GetPlatformInterface().GetLobbyInterface().CreateLobby(ref createLobbyOptions,null,OnCreateLobby);
    }
    private void OnCreateLobby(ref CreateLobbyCallbackInfo data)
    {
        Debug.Log("OnCreateLobby " + data.ResultCode);
        Debug.Log("Lobbyid " + data.LobbyId);
        Debug.Log("GetClientAddress " + _fishyEOS.GetClientAddress());
        Debug.Log("LocalProductUserId " + _fishyEOS.LocalProductUserId);
        //_fishyEOS.RemoteProductUserId = _fishyEOS.LocalProductUserId;
        input_roomId.text = data.LobbyId;
        
        CopyLobbyDetailsHandleOptions copyLobbyDetailsHandleOptions = new CopyLobbyDetailsHandleOptions
        {
            LobbyId = data.LobbyId,
            LocalUserId = EOS.LocalProductUserId
        };

        EOS.GetPlatformInterface().GetLobbyInterface().CopyLobbyDetailsHandle(ref copyLobbyDetailsHandleOptions, out LobbyDetails outLobbyDetailsHandle);
        Debug.Log("outLobbyDetailsHandle " + outLobbyDetailsHandle);
        _lobby = new Lobby();
        _lobby.InitFromLobbyDetails(outLobbyDetailsHandle);
        _fishyEOS.RemoteProductUserId = _lobby.LobbyOwner.ToString();
        var bucketAttribute = new LobbyAttribute
        {
            Key = "bucketid",
            AsString = _lobby.BucketId,
            ValueType = AttributeType.String
        };
        _lobby.Attributes = new List<LobbyAttribute>();
        _lobby.Attributes.Add(bucketAttribute);
        

        UpdateLobbyModificationOptions updateLobbyModificationOptions = new UpdateLobbyModificationOptions
        {
            LobbyId = _lobby.Id,
            LocalUserId = EOS.LocalProductUserId
        };

        

        EOS.GetPlatformInterface().GetLobbyInterface().UpdateLobbyModification(ref updateLobbyModificationOptions, out LobbyModification outLobbyModificationHandle);
        UpdateLobbyOptions updateLobbyOptions = new UpdateLobbyOptions
        {
            LobbyModificationHandle = outLobbyModificationHandle
        };
        EOS.GetPlatformInterface().GetLobbyInterface().UpdateLobby(ref updateLobbyOptions, null, OnUpdateLobby);
    }

    private void OnUpdateLobby(ref UpdateLobbyCallbackInfo data)
    {
        Debug.Log("OnUpdateLobby " + data.ResultCode);
        Debug.Log($"-----------------------{data.ResultCode}--------------------------");
       
    }

    [Command]
    public void FJoinLobby(string lobbyId)
    {
        JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
        {
            LobbyId = lobbyId,
            LocalUserId = EOS.LocalProductUserId
        };
        EOS.GetPlatformInterface().GetLobbyInterface().JoinLobbyById(ref joinLobbyByIdOptions,null,OnJoinLobby);
    }

    private void OnJoinLobby(ref JoinLobbyByIdCallbackInfo data)
    {
        Debug.Log("OnJoinLobby "+data.ResultCode);

        CopyLobbyDetailsHandleOptions copyLobbyDetailsHandleOptions = new CopyLobbyDetailsHandleOptions
        {
            LobbyId = data.LobbyId,
            LocalUserId = EOS.LocalProductUserId
        };

        EOS.GetPlatformInterface().GetLobbyInterface().CopyLobbyDetailsHandle(ref copyLobbyDetailsHandleOptions, out LobbyDetails outLobbyDetailsHandle);
        _lobby = new Lobby();
        _lobby.InitFromLobbyDetails(outLobbyDetailsHandle);
        _fishyEOS.RemoteProductUserId = _lobby.LobbyOwner.ToString();
    }
    #region search
    public void JoinLobbyById()
    {
        if (string.IsNullOrEmpty(input_roomId.text)) return;
        JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
        {
            LobbyId = input_roomId.text,
            LocalUserId = EOS.LocalProductUserId
        };
        EOS.GetPlatformInterface().GetLobbyInterface().JoinLobbyById(ref joinLobbyByIdOptions, null, OnJoinLobbyById);
    }

    private void OnJoinLobbyById(ref JoinLobbyByIdCallbackInfo data)
    {
        Debug.Log("joinLobby " + data.ResultCode);
        Debug.Log($"-------{data.LobbyId}------");
    }

    [Command]
    public void SearchLobby()
    {
        CurrentSearch = new LobbySearch();
        CreateLobbySearchOptions createLobbySearchOptions = new CreateLobbySearchOptions
        {
            MaxResults = 100
        };
        EOS.GetPlatformInterface().GetLobbyInterface().CreateLobbySearch(ref createLobbySearchOptions, out LobbySearch outLobbySearchHandle);





        LobbySearchSetParameterOptions paramOptions = new LobbySearchSetParameterOptions();
        paramOptions.ComparisonOp = ComparisonOp.Equal;
        // Turn SearchString into AttributeData
        AttributeData attrData = new AttributeData();
        attrData.Key = "bucketid".Trim();
        attrData.Value = new AttributeDataValue();
        attrData.Value = "2".Trim();
        paramOptions.Parameter = attrData;

        

        CurrentSearch = outLobbySearchHandle;
        var result = outLobbySearchHandle.SetParameter(ref paramOptions);


        if (result != Result.Success)
        {
            Debug.LogErrorFormat("Lobbies (SearchByAttribute): failed to update SearchByAttribute with parameter. Error code: {0}", result);
            return;
        }
        
        LobbySearchFindOptions lobbySearchFindOptions = new LobbySearchFindOptions
        {
            LocalUserId = EOS.LocalProductUserId
        };
        CurrentSearch.Find(ref lobbySearchFindOptions, null, LobbySearchOnFind);



    }

    private void LobbySearchOnFind(ref LobbySearchFindCallbackInfo data)
    {
        Debug.Log("LobbySearchOnFind " + data.ResultCode);
        Debug.Log("ClientData " + data.ClientData);
        Debug.Log("-----------------------"+data.GetResultCode()+"----------------------");

        //JoinLobbyOptions joinLobbyOptions = new JoinLobbyOptions
        //{

        //}
        //EOS.GetPlatformInterface().GetLobbyInterface().JoinLobby()

        //EOS.GetPlatformInterface().GetLobbyInterface().CreateLobbySearch

        //CopyLobbyDetailsHandleOptions copyLobbyDetailsHandleOptions = new CopyLobbyDetailsHandleOptions
        //{
        //    LocalUserId = EOS.LocalProductUserId
        //};
        //EOS.GetPlatformInterface().GetLobbyInterface().CopyLobbyDetailsHandle(ref copyLobbyDetailsHandleOptions, out LobbyDetails outLobbyDetailsHandle);


        var lobbySearchGetSearchResultCountOptions = new LobbySearchGetSearchResultCountOptions();
        uint searchResultCount = CurrentSearch.GetSearchResultCount(ref lobbySearchGetSearchResultCountOptions);

        Debug.LogFormat("Lobbies (OnLobbySearchCompleted): searchResultCount = {0}", searchResultCount);

        SearchResults.Clear();

       

        LobbySearchCopySearchResultByIndexOptions indexOptions = new LobbySearchCopySearchResultByIndexOptions();

        for (uint i = 0; i < searchResultCount; i++)
        {
            Lobby lobbyObj = new Lobby();

            indexOptions.LobbyIndex = i;

            Result result = CurrentSearch.CopySearchResultByIndex(ref indexOptions, out LobbyDetails outLobbyDetailsHandle);

            if (result == Result.Success && outLobbyDetailsHandle != null)
            {
                lobbyObj.InitFromLobbyDetails(outLobbyDetailsHandle);

                if (lobbyObj == null)
                {
                    Debug.LogWarning("Lobbies (OnLobbySearchCompleted): lobbyObj is null!");
                    continue;
                }

                if (!lobbyObj.IsValid())
                {
                    Debug.LogWarning("Lobbies (OnLobbySearchCompleted): Lobby is invalid, skip.");
                    continue;
                }

                if (outLobbyDetailsHandle == null)
                {
                    Debug.LogWarning("Lobbies (OnLobbySearchCompleted): outLobbyDetailsHandle is null!");
                    continue;
                }

                SearchResults.Add(lobbyObj, outLobbyDetailsHandle);

                Debug.LogFormat("Lobbies (OnLobbySearchCompleted): Added lobbyId: '{0}'", lobbyObj.Id);
            }
        }

    }
    #endregion

    [Command]
    void FSetRemoteProductUserId(string productUserId)
    {
        _fishyEOS.RemoteProductUserId = productUserId;
    }

    void AddListener()
    {
        Epic.OnlineServices.Connect.AddNotifyLoginStatusChangedOptions addNotifyLoginStatusChangedOptions = new Epic.OnlineServices.Connect.AddNotifyLoginStatusChangedOptions();
        EOS.GetPlatformInterface().GetConnectInterface().AddNotifyLoginStatusChanged(ref addNotifyLoginStatusChangedOptions,null,OnNotifyLoginStatusChanged);

        AddNotifyIncomingPacketQueueFullOptions addNotifyIncomingPacketQueueFullOptions = new AddNotifyIncomingPacketQueueFullOptions();
        EOS.GetCachedP2PInterface().AddNotifyIncomingPacketQueueFull(ref addNotifyIncomingPacketQueueFullOptions, null, OnNotifyIncomingPacketQueueFull);

        AddNotifyPeerConnectionEstablishedOptions addNotifyPeerConnectionEstablishedOptions = new AddNotifyPeerConnectionEstablishedOptions();
        EOS.GetCachedP2PInterface().AddNotifyPeerConnectionEstablished(ref addNotifyPeerConnectionEstablishedOptions, null, OnNotifyPeerConnectionEstablished);

        AddNotifyPeerConnectionRequestOptions addNotifyPeerConnectionRequestOptions = new AddNotifyPeerConnectionRequestOptions();
        EOS.GetCachedP2PInterface().AddNotifyPeerConnectionRequest(ref addNotifyPeerConnectionRequestOptions, null, OnNotifyPeerConnectionRequest);

        AddNotifyPeerConnectionInterruptedOptions addNotifyPeerConnectionInterruptedOptions = new AddNotifyPeerConnectionInterruptedOptions();
        EOS.GetCachedP2PInterface().AddNotifyPeerConnectionInterrupted(ref addNotifyPeerConnectionInterruptedOptions, null, OnNotifyPeerConnectionInterrupted);

        AddNotifyPeerConnectionClosedOptions addNotifyPeerConnectionClosedOptions = new AddNotifyPeerConnectionClosedOptions();
        EOS.GetCachedP2PInterface().AddNotifyPeerConnectionClosed(ref addNotifyPeerConnectionClosedOptions, null, OnNotifyPeerConnectionClosed);
    }

    private void OnNotifyPeerConnectionClosed(ref OnRemoteConnectionClosedInfo data)
    {
        Debug.Log("OnNotifyPeerConnectionClosed "+data.GetResultCode());
    }

    private void OnNotifyPeerConnectionInterrupted(ref OnPeerConnectionInterruptedInfo data)
    {
        Debug.Log("OnNotifyPeerConnectionInterrupted "+data.GetResultCode());
    }

    private void OnNotifyPeerConnectionRequest(ref OnIncomingConnectionRequestInfo data)
    {
        Debug.Log("OnNotifyPeerConnectionRequest "+data.GetResultCode());
    }

    private void OnNotifyPeerConnectionEstablished(ref OnPeerConnectionEstablishedInfo data)
    {
        Debug.Log("OnNotifyPeerConnectionEstablished "+data.GetResultCode());
    }

    private void OnNotifyIncomingPacketQueueFull(ref OnIncomingPacketQueueFullInfo data)
    {
       Debug.Log("OnNotifyIncomingPacketQueueFull "+data.GetResultCode());
    }
    private void OnNotifyLoginStatusChanged(ref Epic.OnlineServices.Connect.LoginStatusChangedCallbackInfo data)
    {
       Debug.Log("OnNotifyLoginStatusChanged "+data.GetResultCode());
    }
}
