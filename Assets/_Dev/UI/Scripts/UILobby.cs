using System.Collections;
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
using System;
using System.Data.Common;
using Mono.CSharp;
using Unity.VisualScripting;
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
    ulong idNotifyLoginStatusChanged;
    private void Start()
    {
        _lobbyManager = EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>();
        
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
       _lobbyManager.AddNotifyMemberUpdateReceived(OnNotifyMemberUpdateReceived);
        
    }
    private void Update() {
        _lobby = _lobbyManager.GetCurrentLobby();
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
        Debug.Log("UILobby OnNotifyLobbyUpdate");
        LobbyDetailUpdate();
        LobbyPlayerUpdate();
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
    void AddListener()
    {
        EOSManager.Instance.AddAuthLoginListener(this);
        EOSManager.Instance.AddAuthLogoutListener(this);
        EOSManager.Instance.AddConnectLoginListener(this);

        EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().AddNotifyLobbyChange(OnNotifyLobbyChange);
        EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().AddNotifyLobbyUpdate(OnNotifyLobbyUpdate);
        EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().AddNotifyMemberUpdateReceived(OnNotifyMemberUpdateReceived);
        
        AddNotifyLoginStatusChangedOptions loginStatusOption = new AddNotifyLoginStatusChangedOptions();
        // AddNotifyJoinLobbyAcceptedOptions addNotifyJoinLobbyAcceptedOptions = new AddNotifyJoinLobbyAcceptedOptions();
        AddNotifyLobbyUpdateReceivedOptions addNotifyLobbyUpdateReceivedOptions = new AddNotifyLobbyUpdateReceivedOptions();
        AddNotifyLobbyMemberStatusReceivedOptions addNotifyLobbyMemberStatusReceivedOptions = new AddNotifyLobbyMemberStatusReceivedOptions();
        // AddNotifyRTCRoomConnectionChangedOptions addNotifyRTCRoomConnectionChangedOptions = new AddNotifyRTCRoomConnectionChangedOptions();
        
        EOSManager.Instance.GetEOSAuthInterface().AddNotifyLoginStatusChanged(ref loginStatusOption,null,OnLoginStatusChangeCallback);
        EOSManager.Instance.GetEOSLobbyInterface().AddNotifyLobbyUpdateReceived(ref addNotifyLobbyUpdateReceivedOptions,null,OnNotifyLobbyUpdateReceivedCallback);
        // EOSManager.Instance.GetEOSLobbyInterface().AddNotifyJoinLobbyAccepted(ref addNotifyJoinLobbyAcceptedOptions,null,OnNotifyJoinLobbyAccepted);
        EOSManager.Instance.GetEOSLobbyInterface().AddNotifyLobbyMemberStatusReceived(ref addNotifyLobbyMemberStatusReceivedOptions,null,OnNotifyLobbyMemberStatusReceived);
        // EOSManager.Instance.GetEOSLobbyInterface().AddNotifyRTCRoomConnectionChanged(ref addNotifyRTCRoomConnectionChangedOptions,null,OnNotifyRTCRoomConnectionChanged);
        
    }

    private void OnDisable() {
        EOSManager.Instance.RemoveAuthLoginListener(this);
        EOSManager.Instance.RemoveAuthLogoutListener(this);
        EOSManager.Instance.RemoveConnectLoginListener(this);
        EOSManager.Instance.GetEOSAuthInterface().RemoveNotifyLoginStatusChanged(idNotifyLoginStatusChanged);
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
        Debug.Log(data.ClientData);
        
        
    }
    private void OnLoginStatusChangeCallback(ref LoginStatusChangedCallbackInfo data)
    {
        Debug.Log("UILobby OnLoginStatusChangeCallback "+data.CurrentStatus);
       
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
