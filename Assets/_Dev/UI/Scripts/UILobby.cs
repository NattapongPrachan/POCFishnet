using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Lobby;
using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;
using Sirenix.OdinInspector;
using UnityEngine;
using UniRx;
using TMPro;
using QFSW.QC;
using Epic.OnlineServices;
using FishNet;
using FishNet.Transporting.FishyEOSPlugin;
using System.Linq;
using System.Threading.Tasks;
using System;
using FishNet.Plugins.FishyEOS.Util;
using Mono.CSharp;
using System.Xml.Linq;
using FishNet.Plugins.FishyEOS.Util.Coroutines;

public class UILobby : SerializedMonoBehaviour,IEOSOnAuthLogout, IEOSOnAuthLogin,IEOSOnConnectLogin
{
    [SerializeField]GameObject _uiLobbyGameObject;
    [SerializeField]Transform _contentTransform;
    [SerializeField]PlayerRoomEC _playerRoomPrefab;
    [SerializeField]int _elementLimit = 25;
    [SerializeField]List<PlayerRoomEC> _playerRoomList;

    //Room detail; 
    [SerializeField]TextMeshProUGUI _playerCountTxt;
    public JoinLobbyEvent joinLobbyEvent;
    LobbyInterface _lobbyInterface;
    EOSLobbyManager _lobbyManager;
    [SerializeField]Lobby _lobby;
    
    List<LobbyAttribute> _attributes = new List<LobbyAttribute>();
    //Attribute
    
    private void Start()
    {
        
        
        AddListener();
        CreatePoolElement();
        FEOSLobbies.OnLobbyJoined.Subscribe(lobbyJoined => {
            _lobby = lobbyJoined;
            _uiLobbyGameObject.gameObject.SetActive(true);
            LobbyDetailUpdate();
            LobbyPlayerUpdate();
        });
        this.ObserveEveryValueChanged(_ => _._lobby).Subscribe(_ =>{
            Debug.Log("Lobby observe update "+_.Members.Count);
        }).AddTo(this);
        //_lobbyManager = EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>();
        //_lobbyManager.AddNotifyMemberUpdateReceived(OnNotifyMemberUpdateReceived);
        
    }
    private void Update() {
       //_lobby = _lobbyManager.GetCurrentLobby();
    }
    [Command]
    void LobbyDetailUpdate()
    {
        _playerCountTxt.text = _lobby.AvailableSlots+"/"+_lobby.MaxNumLobbyMembers;
    }

    private void JoinedLobby(Lobby lobby)
    {
        joinLobbyEvent?.Invoke(lobby);
    }
    [Command]
    void LobbyPlayerUpdate()
    {
        _lobby = EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().GetCurrentLobby();
        _playerRoomList.ForEach(playerElement => playerElement.gameObject.SetActive(false));
        for (int i = 0; i < _lobby.Members.Count; i++)
        {
            var player = _playerRoomList[i];
               
            _playerRoomList[i].gameObject.SetActive(true);
            _playerRoomList[i].SetupData(_lobby,_lobby.Members[i]);
        }
    }
    [Command]
    public void LeaveLobby()
    {
        Debug.Log("LeaveLobby "+EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>());
        EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>()?.LeaveLobby(_ =>{
            Debug.Log("LeaveLobby "+_);
            _lobby = null;
        });
    }
    [Command]
    public void StartGame()
    {
        var lobbyManager = EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().GetCurrentLobby();
        var isOwner = _lobby.IsOwner(EOSManager.Instance.GetProductUserId());
        if(isOwner)
        {
            _lobby = EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().GetCurrentLobby();
            foreach (var item in _lobby.Attributes)
            {
                Debug.Log(item.Key + " : " + item);
            }
            _lobby.Attributes.FirstOrDefault( lob => lob.Key == "GAMESTART").AsBool = true;

            EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().ModifyLobby(_lobby,_=>{
                
            });
        }
    }
    [Command]
    public void ChangeInt(int integer)
    {
        _lobby = EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().GetCurrentLobby();
        Debug.Log("attrivute count "+_lobby.Attributes.Count);
        foreach (var item in _lobby.Attributes)
        {
            Debug.Log(item.Key + " : " + item + "value : "+item.ValueType);
        }
        var value = _lobby.Attributes.FirstOrDefault(_ => _.Key == "INT").AsInt64;
        Debug.Log("Attribute Value " + value.Value);
        value = Convert.ToInt64(integer);

        EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().ModifyLobby(_lobby, _ =>
        {

        });
    }
    bool isStart = false;
    void StartServer()
    {

        var networkManager = InstanceFinder.NetworkManager;
        var localUserId = EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().GetCurrentLobby().LobbyOwner;//LobbyVariables.Instance.ProductUserId;
        var fishyEOS = networkManager.GetComponent<FishyEOS>();
        fishyEOS.Initialize(networkManager, 0);
        
        fishyEOS.RemoteProductUserId = localUserId.ToString();
        if (isStart) return;
        if (!_lobby.IsOwner(EOSManager.Instance.GetProductUserId()))
        {
            networkManager.ClientManager.StartConnection();
        }else{
            networkManager.ServerManager.StartConnection();
            networkManager.ClientManager.StartConnection();
        }
        UILobbyManager.Instance.CloseLobby();
        isStart = true;
    }
    void ClearContent()
    {

    }

    private void CreatePoolElement()
    {
        for (int i = 0; i < _elementLimit ; i++)
        {
            _playerRoomList.Add(Instantiate(_playerRoomPrefab,_contentTransform));
        }
    }
    
    private void OnNotifyLobbyChange()
    {
        //user has changed lobbies
        Debug.Log("UILobby OnNotifyLobbyChange");
        _uiLobbyGameObject.gameObject.SetActive(_lobby != null);
        if(_lobby != null)
        {
            LobbyDetailUpdate();
            LobbyPlayerUpdate();
        }
        
    }
    private void OnNotifyLobbyUpdate()
    {
        //when the current lobby data has been updated
        //Debug.Log("UILobby OnNotifyLobbyUpdate");
        //_lobby.InitFromLobbyHandle(_lobby.Id);
        //_lobby = EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().GetCurrentLobby();
        //Debug.Log("attrivbute count " + _lobby.Attributes.Count);

        //CheckGameStart();
        //LobbyDetailUpdate();
        //LobbyPlayerUpdate();
    }
    private void OnNotifyMemberUpdateReceived(string LobbyId, ProductUserId MemberId)
    {
        //receive notification when lobby member update is received
      //  LobbyMember updatedMember = currentLobby.Members.Find((LobbyMember member) => { return member.ProductId == MemberId; });
        LobbyMember updateMember = _lobby.Members.Find((LobbyMember member) => member.ProductId == MemberId);
        Debug.Log("UILobby OnNotifyMemberUpdateReceived "+LobbyId + "member is "+MemberId);
        Debug.Log("updateMember "+updateMember);
        //LobbyDetailUpdate();
        //LobbyPlayerUpdate();
    }


    
    public void OnAuthLogin(LoginCallbackInfo loginCallbackInfo)
    {

    }
    public void OnAuthLogout(LogoutCallbackInfo logoutCallbackInfo)
    {

    }

    public void OnConnectLogin(Epic.OnlineServices.Connect.LoginCallbackInfo loginCallbackInfo)
    {

    }
    private void OnEnable()
    {
        Debug.Log("UILobby OnEnable");
    }
    ulong idNotifyLoginStatusChanged;
    ulong idNotifyLobbyUpdateReceived;
    ulong idNotifyLobbyMemberStatusReceived;
    void AddListener()
    {
        Debug.Log("AddListener*********************************");
        EOSManager.Instance.AddAuthLoginListener(this);
        EOSManager.Instance.AddAuthLogoutListener(this);
        EOSManager.Instance.AddConnectLoginListener(this);

        EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().AddNotifyLobbyChange(OnNotifyLobbyChange);
        EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().AddNotifyLobbyUpdate(OnNotifyLobbyUpdate);
        EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().AddNotifyMemberUpdateReceived(OnNotifyMemberUpdateReceived);


        // AddNotifyJoinLobbyAcceptedOptions addNotifyJoinLobbyAcceptedOptions = new AddNotifyJoinLobbyAcceptedOptions();


        // AddNotifyRTCRoomConnectionChangedOptions addNotifyRTCRoomConnectionChangedOptions = new AddNotifyRTCRoomConnectionChangedOptions();


        Epic.OnlineServices.Connect.AddNotifyLoginStatusChangedOptions loginStatusOption = new Epic.OnlineServices.Connect.AddNotifyLoginStatusChangedOptions();
        idNotifyLoginStatusChanged = EOSManager.Instance.GetEOSConnectInterface().AddNotifyLoginStatusChanged(ref loginStatusOption, null,OnConnectLoginStatusChangeCallback);


        // EOSManager.Instance.GetEOSLobbyInterface().AddNotifyJoinLobbyAccepted(ref addNotifyJoinLobbyAcceptedOptions,null,OnNotifyJoinLobbyAccepted);
        AddNotifyLobbyMemberStatusReceivedOptions addNotifyLobbyMemberStatusReceivedOptions = new AddNotifyLobbyMemberStatusReceivedOptions();
        idNotifyLobbyMemberStatusReceived = EOSManager.Instance.GetEOSLobbyInterface().AddNotifyLobbyMemberStatusReceived(ref addNotifyLobbyMemberStatusReceivedOptions,null,OnNotifyLobbyMemberStatusReceived);
        // EOSManager.Instance.GetEOSLobbyInterface().AddNotifyRTCRoomConnectionChanged(ref addNotifyRTCRoomConnectionChangedOptions,null,OnNotifyRTCRoomConnectionChanged);

        AddNotifyLobbyUpdateReceivedOptions addNotifyLobbyUpdateReceivedOptions = new AddNotifyLobbyUpdateReceivedOptions();
        idNotifyLobbyUpdateReceived = EOS.GetPlatformInterface().GetLobbyInterface().AddNotifyLobbyUpdateReceived(ref addNotifyLobbyUpdateReceivedOptions, null, OnNotifyLobbyUpdateReceivedCallback);

    }

    private void OnDisable() {
        EOSManager.Instance.RemoveAuthLoginListener(this);
        EOSManager.Instance.RemoveAuthLogoutListener(this);
        EOSManager.Instance.RemoveConnectLoginListener(this);
        EOSManager.Instance.GetEOSAuthInterface().RemoveNotifyLoginStatusChanged(idNotifyLoginStatusChanged);
        EOSManager.Instance.GetEOSLobbyInterface().RemoveNotifyLobbyUpdateReceived(idNotifyLobbyUpdateReceived);
        EOSManager.Instance.GetEOSLobbyInterface().RemoveNotifyLobbyMemberStatusReceived(idNotifyLobbyMemberStatusReceived);
    }

    private void OnUpdateLobby(ref UpdateLobbyCallbackInfo data)
    {
        Debug.Log("OnupdateLobby "+data.ResultCode);
         _lobby = EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().GetCurrentLobby();
    }

    private void OnNotifyJoinLobbyAccepted(ref JoinLobbyAcceptedCallbackInfo data)
    {
        Debug.Log("OnNotifyJoinLobbyAccepted "+data.GetResultCode());
        Debug.Log("localuserid "+data.LocalUserId);
    }
    private void OnNotifyLobbyUpdateReceivedCallback(ref LobbyUpdateReceivedCallbackInfo data)
    {
        Debug.Log("OnNotifyLobbyUpdateReceivedCallback "+data.LobbyId);
        Debug.Log(data.LobbyId);
        _lobby.InitFromLobbyHandle(_lobby.Id);
        _lobby = EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().GetCurrentLobby();
        _attributes = _lobby.Attributes;
        CheckGameStart();
    }

    private void CheckGameStart()
    {
        LobbyAttribute gameStartAttribute = _lobby.Attributes.FirstOrDefault( lob => lob.Key.ToUpper() == "GAMESTART");
        if(gameStartAttribute.AsBool == true)
        {
            StartServer();
        }
    }

    private void OnConnectLoginStatusChangeCallback(ref Epic.OnlineServices.Connect.LoginStatusChangedCallbackInfo data)
    {
        if(data.CurrentStatus == LoginStatus.LoggedIn)
        {
            
        }
       
    }
    private void OnNotifyRTCRoomConnectionChanged(ref RTCRoomConnectionChangedCallbackInfo data)
    {
        Debug.Log("OnNotifyRTCRoomConnectionChanged "+data.GetResultCode());
        Debug.Log("IsConnected "+data.IsConnected);
        Debug.Log("IsConnected "+data.IsConnected);
    }
    
    private void OnNotifyLobbyMemberStatusReceived(ref LobbyMemberStatusReceivedCallbackInfo data)
    {
        Debug.Log("Onmember status received lobbyid "+data.LobbyId);
        Debug.Log("targetuserid "+data.TargetUserId);
        Debug.Log("currentStatus "+data.CurrentStatus);
        switch(data.CurrentStatus)
        {
            case LobbyMemberStatus.Kicked:
                //LeaveLobby();
                OnLobbyKicked(ref data);
            break;
            case LobbyMemberStatus.Joined:
                UpdateLobby();
            break;
            case LobbyMemberStatus.Promoted:
                UpdateLobby();
            break;
        }
    }
    void OnLobbyKicked(ref LobbyMemberStatusReceivedCallbackInfo data)
    {
        if(data.TargetUserId == EOSManager.Instance.GetProductUserId())
        {
            LeaveLobby();
            _lobby = null;
            _uiLobbyGameObject.SetActive(false);
        }
    }
    void UpdateLobby()
    {
        _lobby = EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().GetCurrentLobby();
        _lobby.InitFromLobbyHandle(_lobby.Id);
        LobbyDetailUpdate();
        LobbyPlayerUpdate();
    }
    

   
}
