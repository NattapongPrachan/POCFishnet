using System;
using System.Collections;
using System.Collections.Generic;
using Epic.OnlineServices.Auth;
using TMPro;
using UniRx;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using JetBrains.Annotations;

public class UIRoom : SerializedMonoBehaviour
{
    [SerializeField]Transform _contentTransform;
    [SerializeField]PlayerRoomEC _playerRoomPrefab;
    [SerializeField]TextMeshProUGUI _playerCountTxt;
    [SerializeField]TextMeshProUGUI _roomNameTxt;
    [SerializeField]TextMeshProUGUI _gameModeTxt;
    [SerializeField]int _elementLimit = 25;
    Lobby _lobby;
    [SerializeField]List<PlayerRoomEC> _playerRoomList;
    IDisposable _updateLobbyObservable;
    ILobbyEvents _lobbyEvent;
    LobbyEventCallbacks _lobbyEventCallback = new LobbyEventCallbacks();

    ///DataUpdate
    [SerializeField] Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> playerDataUpdated;
    private void Awake()
    {
         CreatePoolElement();
    }
    private void Start()
    {
        TestLobby.OnLeaveLobby.Subscribe(_ =>{
            Close();
        }).AddTo(this);
    }
    
    public async void StartGame()
    {
        if(IsLobbyHost())
        {
            try{
                //string relayCode = await TestRelay.Instance.CreateRelay();
                string joinCode = await TestRelay.Instance.StartHost();
                _lobby = await Lobbies.Instance.UpdateLobbyAsync(_lobby.Id,
                new UpdateLobbyOptions{
                    Data = new Dictionary<string, DataObject>
                    {
                        {"KeyStartGame",new DataObject(DataObject.VisibilityOptions.Member,joinCode)}
                    }
                });
            }catch(LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
        Close();
    }
    
    private void OnRoomPropertiesUpdate(ILobbyChanges changes)
    {
        Debug.Log("was called!!!"+changes);
    }
    public void UpdateLobby(Lobby lobby)
    {
        Debug.Log("UpdateLobby "+lobby);
        if(lobby == null)
        {
            Close();
            return;
        }
        ClearContent();
        RemoveEvent();
        AddEvents(lobby.Id);
        _lobby = lobby;
        
        _gameModeTxt.text = _lobby.Data["GameMode"].Value;
        _roomNameTxt.text = _lobby.Name;
        _playerCountTxt.text = _lobby.Players.Count + "/"+ _lobby.MaxPlayers;
        UpdatePlayer(_lobby.Players);

    }
    private async void AddEvents(string _lobbyId)
    {
        Debug.Log("AddEvents "+_lobbyId);
        // SUBSCRIBE TO CALLS
        _lobbyEventCallback.LobbyChanged  += OnLobbyChanged;
        _lobbyEventCallback.LobbyEventConnectionStateChanged += OnLobbyEventConnectionStateChanged;
        _lobbyEventCallback.LobbyDeleted += OnLobbyDeleted;
        _lobbyEventCallback.KickedFromLobby += OnKickFormLobby;

        _lobbyEventCallback.DataChanged += OnDataChanged;
        _lobbyEventCallback.DataAdded += OnDataAdded;
        _lobbyEventCallback.DataRemoved += OnDataRemoved;

        _lobbyEventCallback.PlayerJoined += OnPlayerJoined;
        _lobbyEventCallback.PlayerLeft += OnPlayerLeft;
        _lobbyEventCallback.PlayerDataAdded += OnPlayerDataAdded;
        _lobbyEventCallback.PlayerDataChanged += OnPlayerDataChanged;
        _lobbyEventCallback.PlayerDataRemoved += OnPlayerDataRemoved;

        
        try
        {
            _lobbyEvent = await Lobbies.Instance.SubscribeToLobbyEventsAsync(_lobbyId, _lobbyEventCallback).AddTo(this);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

   

   

    async void RemoveEvent()
    {
        if(_lobbyEvent != null)
        {
            await _lobbyEvent.UnsubscribeAsync();
        }
        
        _lobbyEventCallback.LobbyChanged  -= OnLobbyChanged;
        _lobbyEventCallback.LobbyEventConnectionStateChanged -= OnLobbyEventConnectionStateChanged;
        _lobbyEventCallback.LobbyDeleted -= OnLobbyDeleted;
        _lobbyEventCallback.KickedFromLobby -= OnKickFormLobby;

        _lobbyEventCallback.DataChanged -= OnDataChanged;
        _lobbyEventCallback.DataAdded -= OnDataAdded;
        _lobbyEventCallback.DataRemoved -= OnDataRemoved;

        _lobbyEventCallback.PlayerJoined -= OnPlayerJoined;
        _lobbyEventCallback.PlayerLeft -= OnPlayerLeft;
        _lobbyEventCallback.PlayerDataAdded -= OnPlayerDataAdded;
        _lobbyEventCallback.PlayerDataChanged -= OnPlayerDataChanged;
        _lobbyEventCallback.PlayerDataRemoved -= OnPlayerDataRemoved;
        
    }

    void UpdatePlayer(List<Player> players)
    {
        Debug.Log("Player Room "+players.Count);
        Debug.Log("_playerRoomList "+_playerRoomList.Count);
        //_playerRoomList.ForEach(p => p.gameObject.SetActive(false));
        for (int i = 0; i < players.Count; i++)
        {
            AddPlayerRoomEC(players[i]);
        }
    }
    void AddPlayerRoomEC(Player player)
    {
        Debug.Log("AddPlayerRoomEC "+player.Id);
        var room = Instantiate(_playerRoomPrefab,_contentTransform);
            room.gameObject.SetActive(true);
            room.SetupPlayerData(player);
            room.UpdateHost(_lobby.HostId,AuthenticationService.Instance.PlayerId);
            _playerRoomList.Add(room);
    }
    bool IsLobbyHost()
    {
        return _lobby.HostId == AuthenticationService.Instance.PlayerId;
    }
    void UpdateHost()
    {
        Debug.Log("old host "+_lobby.HostId);
        _playerRoomList.ForEach(room => room.UpdateHost(_lobby.HostId,AuthenticationService.Instance.PlayerId));
    }
    void RemovePlayerRoomEC()
    {

    }
    private void OnEnable()
    {
        // updateLobbyObservable = Observable.Interval(TimeSpan.FromSeconds(10)).Subscribe(async _ =>{
        //     _lobby = await LobbyService.Instance.GetLobbyAsync(_lobby.Id);
        //     UpdateLobby(_lobby);
        // }).AddTo(this);
    }
    private void OnDisable() {
        RemoveEvent();
    }
    

    private void CreatePoolElement()
    {
        _playerRoomList = new List<PlayerRoomEC>();
        // for (int i = 0; i < _elementLimit ; i++)
        // {
        //     _playerRoomList.Add(Instantiate(_playerRoomPrefab,_contentTransform));
        // }
    }
    void Close()
    {
        this.gameObject.SetActive(false);
    }

    #region PlayerEvent
    private void OnPlayerJoined(List<LobbyPlayerJoined> list)
    {
        Debug.Log("OnplayerJoined "+list.Count);
        foreach (var player in list)
        {
            AddPlayerRoomEC(player.Player);
        }
    }
    private void OnPlayerLeft(List<int> list)
    {
        foreach (var index in list)
        {
            Debug.Log("item "+index);
            _playerRoomList[index].Destroy();
            _playerRoomList.RemoveAt(index);
        }
    }
    void ClearContent()
    {
        // for (int i = 0; i < _contentTransform.childCount; i++)
        // {
        //     Destroy(_contentTransform.GetChild(i).gameObject);
        // }
        _playerRoomList.ForEach(x => x.Destroy());
        _playerRoomList.Clear();
    }
    private void OnPlayerDataAdded(Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> dictionary)
    {
        Debug.Log("OnPlayerDataAdded "+dictionary);
    }
     private void OnPlayerDataRemoved(Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> dictionary)
    {
        Debug.Log("OnPlayerDataRemoved "+dictionary);
    }

    private void OnPlayerDataChanged(Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> dictionary)
    {
        Debug.Log("OnPlayerDataChanged "+dictionary);
    }
    #endregion
    #region LobbyEvent
    private void OnLobbyEventConnectionStateChanged(LobbyEventConnectionState state)
    {
        Debug.Log("OnLobbyEventConnectionStateChanged "+state);
    }
    private async void OnLobbyChanged(ILobbyChanges changes)
    {
       Debug.Log("OnLobbychanged "+changes);
       Debug.Log("changes.AvailableSlots.Added : "+changes.AvailableSlots.Added);
       Debug.Log("changes.AvailableSlots.Changed : "+changes.AvailableSlots.Changed);
       Debug.Log("changes.AvailableSlots.Value : "+changes.AvailableSlots.Value);
       Debug.Log("--------------------------------------------------------------");
       Debug.Log("PlayerData.Added : "+changes.PlayerData.Added);
       Debug.Log("PlayerData.Changed : "+changes.PlayerData.Changed);
       Debug.Log("PlayerData.Value : "+changes.PlayerData.Value);
       Debug.Log("--------------------------------------------------------------");
       Debug.Log("HostId.Added : "+changes.HostId.Added);
       Debug.Log("HostId.Changed : "+changes.HostId.Changed);
       Debug.Log("HostId.Value : "+changes.HostId.Value);
       Debug.Log("--------------------------------------------------------------");
       Debug.Log("Data.Added : "+changes.Data.Added);
       Debug.Log("Data.Changed : "+changes.Data.Changed);
       Debug.Log("Data.Value : "+changes.Data.Value);

       if(changes.HostId.Changed)
       {
        TestLobby.Instance.UpdateJoinedLobby();
       }
       if(changes.Data.Changed)
       {
        foreach (var item in changes.Data.Value)
        {
            Debug.Log("Data key "+item.Key + "value : "+item.Value);
            if(item.Key == "KeyStartGame")
            {
                Debug.Log("JoinCode "+item.Value.Value.ToString());
                if(IsLobbyHost())
                {
                    //await TestRelay.Instance.StartHost();
                }else{
                    await TestRelay.Instance.StartClient(item.Value.Value.Value.ToString());
                }
                TestLobby.Instance.isUpdateLobby = false;
            }
        }
       }
    }
    private void OnLobbyDeleted()
    {
        Debug.Log("OnLobbyDeleted");
    }
    private void OnKickFormLobby()
    {
        Debug.Log("OnKickFormLobby");
        Close();
    }
    #endregion

    #region DataEvent
    private void OnDataChanged(Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> dictionary)
    {
        Debug.Log("OnDataChanged "+dictionary);
    }
    private void OnDataAdded(Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> dictionary)
    {
        Debug.Log("OnDataAdded "+dictionary);
    }

    private void OnDataRemoved(Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> dictionary)
    {
        Debug.Log("OnDataRemoved "+dictionary);
    }
    #endregion
}
