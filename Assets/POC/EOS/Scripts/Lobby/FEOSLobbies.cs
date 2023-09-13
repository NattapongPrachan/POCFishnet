using System.Data.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Connect;
using Epic.OnlineServices.Lobby;
using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;
using QFSW.QC;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

public class FEOSLobbies : MonoBehaviour, IEOSOnAuthLogin, IEOSOnAuthLogout, IEOSOnConnectLogin
{

    

    public static Subject<Lobby> OnLobbyJoined = new Subject<Lobby>();
    public static Subject<Dictionary<Lobby,LobbyDetails>> OnLobbiesUpdated = new Subject<Dictionary<Lobby, LobbyDetails>>();

    [SerializeField]Lobby _joinLobby;
    [SerializeField]List<LobbyAttribute> _lobbyAttribute;
    [SerializeField]Dictionary<Lobby,LobbyDetails> _lobbyDetails;
    Action _lobbyChangeCallback;

    LobbySearch _lobbySearch;
    EOSLobbyManager _lobbyManager;
    private void Start()
    {
    }
    
    [Command]
    public void CreateLobby(string bucketId)
    {
        var lobby = new Lobby();
        lobby.BucketId = bucketId;
        lobby.MaxNumLobbyMembers = 8;
        //lobby.LobbyOwnerAccountId = EOSManager.Instance.GetLocalUserId();
        lobby.AllowInvites = true;
        lobby.LobbyPermissionLevel = LobbyPermissionLevel.Publicadvertised;
        lobby.DisableHostMigration = false;
        var attribute = new LobbyAttribute();
        attribute.Key = "Bavaria";
        attribute.AsString = "Dielser";
        attribute.ValueType = AttributeType.String;

        var bucketAttribute = new LobbyAttribute{
            Key = "bucketid",
            AsString = lobby.BucketId,
            ValueType = AttributeType.String
        };

        var gameStartAttribute = new LobbyAttribute{
            Key = "gameStart",
            AsBool = false,
            ValueType = AttributeType.Boolean
        };

        lobby.Attributes.Add(attribute);
        lobby.Attributes.Add(bucketAttribute);
        lobby.Attributes.Add(gameStartAttribute);

        var createLobbyOptions = new CreateLobbyOptions{
            BucketId = "2",
            MaxLobbyMembers = 8,
            AllowInvites = true,
            EnableJoinById = true,
            LocalUserId = EOSManager.Instance.GetProductUserId(),
            PresenceEnabled = true,
        };
        //EOSManager.Instance.GetEOSLobbyInterface().CreateLobby(ref createLobbyOptions,null,OnCreateLobby);

        Debug.Log("EOSLobby "+EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>()._Dirty);
        EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().CreateLobby(lobby,OnCreateLobbyComplete);
        // CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
        // {
        //     BucketId = "1",
        //      EnableJoinById = "true",
        //       PermissionLevel
        // }
        //  EOSManager.Instance.GetEOSLobbyInterface().CreateLobby()

        
    }



    private void OnCreateLobby(ref CreateLobbyCallbackInfo data)
    {
        Debug.Log("OnCreateLobby "+data.ResultCode);
        Debug.Log("EOSManager.Instance.GetProductUserId() "+EOSManager.Instance.GetProductUserId());

        // JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions{
        //     LobbyId = data.LobbyId,
        //     LocalUserId = EOSManager.Instance.GetProductUserId(),
        //     PresenceEnabled = true
        // };
        // EOSManager.Instance.GetEOSLobbyInterface().JoinLobbyById(ref joinLobbyByIdOptions,null,OnJoinLobby);
    }

    private void OnJoinLobby(ref JoinLobbyByIdCallbackInfo data)
    {
        Debug.Log("OnJoinLobby "+data.ResultCode);
    }

    private void OnCreateLobbyComplete(Result result)
    {
        Debug.Log("LobbyCreated "+result);
        var currentLobby = EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().GetCurrentLobby();
        currentLobby.InitFromLobbyHandle(currentLobby.Id);
        currentLobby.IsValid(); 
        Debug.Log("lobby  "+currentLobby.IsValid());
        // _joinLobby = lobby;
        // _lobbyAttribute = _joinLobby.Attributes;
        // OnLobbyJoined?.OnNext(lobby);
    }

    [Command]
    public void CreateSession()
    {
        
    }
    [Command,Button]
    public void SearchLobbyByAttribute(string attributeValue)
    {
        Debug.Log("userid "+EOSManager.Instance.GetProductId());
        EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().SearchByAttribute("bucketid",attributeValue,_ =>{
            Debug.Log("OnSearch  "+_.ToString());
            GetSearchResults();
            
            // _lobbyManager.JoinLobby("",_lobbydetails.,true,_ =>{
            //     Debug.Log("OnJoinLobbycompleted "+_.ToString());
            // });
        });
    }
    public void OnAuthLogin(Epic.OnlineServices.Auth.LoginCallbackInfo loginCallbackInfo)
    {
        Debug.Log("FEOSLobbies OnAuthLogin ");
       
    }

    private void OnMemberUpdateReceiveCallback(string LobbyId, ProductUserId MemberId)
    {
        Debug.Log("OnMemberUpdateReceiveCallback "+LobbyId + "member "+MemberId);
    }
    private void OnLobbyUpdateCallback()
    {
        Debug.Log("OnLobbyUpdateCallback aaaaa");
    }

    void OnLobbyChangeCallback()
    {
        Debug.Log("OnLobbyChangeCallback");
    }

    public void OnAuthLogout(LogoutCallbackInfo logoutCallbackInfo)
    {

    }
    [Command]
    void Search()
    {
        //_lobbyManager.SearchByAttribute()
        //_lobbyManager.SearchByAttribute()
    }
    [Command]
    public void GetCurrentSearch(string localUserID)
    {
       _lobbySearch = EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().GetCurrentSearch();     
        
    //    JoinLobbyOptions joinLobbyOptions = new JoinLobbyOptions{
        
    //    };
    //    EOSManager.Instance.GetEOSLobbyInterface().JoinLobby()
    //    foreach (var item in _lobbySearch)
    //         {
                
    //         }
    }
    [Command]
    public void GetSearchResults()
    {
        _lobbyDetails = EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().GetSearchResults();
        OnLobbiesUpdated?.OnNext(_lobbyDetails);
    }
    public void OnConnectLogin(Epic.OnlineServices.Connect.LoginCallbackInfo loginCallbackInfo)
    {
    //    _lobbyManager = EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>();
    //     _lobbyManager.AddNotifyLobbyUpdate(OnLobbyUpdateCallback);
    //     _lobbyManager.AddNotifyLobbyChange(OnLobbyChangeCallback);
    //     _lobbyManager.AddNotifyMemberUpdateReceived(OnMemberUpdateReceiveCallback);
    }
    
    ulong idNotifyLoginStatusChanged;
    

    private void OnLoginStatusChangeCallback(ref Epic.OnlineServices.Auth.LoginStatusChangedCallbackInfo data)
    {

        // _lobbyManager = EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>();
        // _lobbyManager.AddNotifyLobbyUpdate(OnLobbyUpdateCallback);
        // _lobbyManager.AddNotifyLobbyChange(OnLobbyChangeCallback);
        // _lobbyManager.AddNotifyMemberUpdateReceived(OnMemberUpdateReceiveCallback);
    }

    private void OnConnectLoginStatusChangeCallback(ref Epic.OnlineServices.Connect.LoginStatusChangedCallbackInfo data)
    {
        Debug.Log("Lobbies OnConnectLoginStatusChangeCallback "+data.CurrentStatus);
        if(data.CurrentStatus == LoginStatus.LoggedIn)
        {
            
        }
    }
    private void OnEnable()
    {
        Epic.OnlineServices.Auth.AddNotifyLoginStatusChangedOptions loginStatusOption = new Epic.OnlineServices.Auth.AddNotifyLoginStatusChangedOptions
        {
        };
        Epic.OnlineServices.Connect.AddNotifyLoginStatusChangedOptions connectLoginOption = new Epic.OnlineServices.Connect.AddNotifyLoginStatusChangedOptions
        {
        
        };
        idNotifyLoginStatusChanged = EOSManager.Instance.GetEOSAuthInterface().AddNotifyLoginStatusChanged(ref loginStatusOption,null,OnLoginStatusChangeCallback);
        
        EOSManager.Instance.GetEOSConnectInterface().AddNotifyLoginStatusChanged(ref connectLoginOption,null,OnConnectLoginStatusChangeCallback);
        EOSManager.Instance.AddAuthLoginListener(this);
        EOSManager.Instance.AddAuthLogoutListener(this);
        EOSManager.Instance.AddConnectLoginListener(this);

        EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().AddNotifyLobbyChange(OnNotifyLobbyChange);
        EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().AddNotifyLobbyUpdate(OnNotifyLobbyUpdate);
        EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().AddNotifyMemberUpdateReceived(OnNotifyMemberUpdateReceived);
    }

    private void OnNotifyMemberUpdateReceived(string LobbyId, ProductUserId MemberId)
    {
        Debug.Log("EOSLobby OnNotifyMemberUpdateReceived "+LobbyId + "member "+MemberId);
    }

    private void OnNotifyLobbyUpdate()
    {
        Debug.Log("EOSLobby OnNotifyLobbyUpdate ");
    }

    private void OnNotifyLobbyChange()
    {
        Debug.Log("EOSLobby OnNotifyLobbyChange ");
    }

    private void OnDisable() {
        EOSManager.Instance.RemoveAuthLoginListener(this);
        EOSManager.Instance.RemoveAuthLogoutListener(this);
        EOSManager.Instance.RemoveConnectLoginListener(this);
        EOSManager.Instance.GetEOSAuthInterface().RemoveNotifyLoginStatusChanged(idNotifyLoginStatusChanged);
    }
}
