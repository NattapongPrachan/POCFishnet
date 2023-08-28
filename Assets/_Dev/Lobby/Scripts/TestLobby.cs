using System.Collections;
using System.Collections.Generic;
using QFSW.QC;
using Sirenix.OdinInspector;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UniRx;
using System;

public class TestLobby : MonoInstance<TestLobby>
{
    Lobby _hostLobby,_joinedLobby;
    float _heartbeatTimer;
    float _LobbyUpdateTimer;
    string _playerName;

    public static Subject<Unit> OnSignedComplete = new Subject<Unit>();
    public static Subject<Lobby> OnJoinedLobby = new Subject<Lobby>();
    public static Subject<Lobby> OnLeaveLobby = new Subject<Lobby>();
    public static Subject<List<Lobby>> OnLobbyUpdated = new Subject<List<Lobby>>();
    public bool isUpdateLobby = true;
    async void Start()
    {
        await UnityServices.InitializeAsync();
         _playerName = "YGG"+UnityEngine.Random.Range(0,99);
        AuthenticationService.Instance.SignedIn += () =>
        {
            OnSignedComplete.OnNext(default);
        };
        SignInOptions option = new SignInOptions();
        option.CreateAccount = true;
    
        await AuthenticationService.Instance.SignInAnonymouslyAsync(option);
        
       
        Debug.Log(_playerName);
    }
    private void Update() {
        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdates();
    }
    async void HandleLobbyHeartbeat()
    {
        if(_hostLobby != null)
        {
            _heartbeatTimer -= Time.deltaTime;
            if(_heartbeatTimer <0f)
            {
                float heartbeatTimerMax = 15;
                _heartbeatTimer = heartbeatTimerMax;
                await LobbyService.Instance.SendHeartbeatPingAsync(_hostLobby.Id);
            }
        }
    }
    void HandleLobbyPollForUpdates()
    {
        if(isUpdateLobby && _joinedLobby != null)
        {
            _LobbyUpdateTimer -= Time.deltaTime;
            if(_LobbyUpdateTimer <0f)
            {
                float _LobbyUpdateTimerMax = 15;
                _LobbyUpdateTimer = _LobbyUpdateTimerMax;
                UpdateJoinedLobby();
            }
        }
    }
    public async void UpdateJoinedLobby()
    {
        Lobby lobby = await LobbyService.Instance.GetLobbyAsync(_joinedLobby.Id);
        _joinedLobby = lobby;
        OnJoinedLobby.OnNext(_joinedLobby);
    }
    [Command]
    public async void CreateLobby()
    {
        try
        {
        string lobbyName = "MyLobby";
        int maxplayer = 4;
        CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions {
            IsPrivate = false,
            Player = GetPlayer(),
            Data = new Dictionary<string, DataObject>
            {
                {"GameMode",new DataObject(DataObject.VisibilityOptions.Public,"CaptureTheFlag")},
                {"Map", new DataObject(DataObject.VisibilityOptions.Public,"TeleTubbies")}
            }
        };
        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName,maxplayer,createLobbyOptions);
        _hostLobby = lobby;
        _joinedLobby = _hostLobby;
        Debug.Log("CreateLobby "+lobbyName +" "+lobby.MaxPlayers+ " "+lobby.Id+" "+lobby.LobbyCode);
        PrintPlayers(_hostLobby);
        OnJoinedLobby.OnNext(_joinedLobby);
        }catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    [Command]
    public async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions{
                Count = 25,
                Filters = new List<QueryFilter>{
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots,"0",QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>{
                    new QueryOrder(false,QueryOrder.FieldOptions.Created)
                }
            };
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            Debug.Log("Lobbies found "+queryResponse.Results.Count);
            foreach(Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name + " "+lobby.MaxPlayers + " "+lobby.Data["GameMode"].Value+" code : "+lobby.LobbyCode);
            }
            OnLobbyUpdated.OnNext(queryResponse.Results);
        }catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }
    [Command]
    public async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            //QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions{
                Player = GetPlayer()
            };
            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode,joinLobbyByCodeOptions);
            _joinedLobby = lobby;
            Debug.Log("Joib Lobby "+lobbyCode);
            PrintPlayers(_joinedLobby);
            OnJoinedLobby.OnNext(_joinedLobby);
        }catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    public async void JoinLobbyById(string lobbyId)
    {
        try{
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions{
                Player = GetPlayer()
            };
            Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId,joinLobbyByIdOptions);
            _joinedLobby = lobby;
            PrintPlayers(_joinedLobby);
            OnJoinedLobby.OnNext(_joinedLobby);
        }catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    [Command]
    void QuickJoinLobby()
    {
        try{
            LobbyService.Instance.QuickJoinLobbyAsync();
        }catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    public Player GetPlayer()
    {
        return new Player{
                Data = new Dictionary<string, PlayerDataObject>
                {
                    { "PlayerName",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,_playerName)}
                }
        };
    }
    [Command]
    void PrintPlayers()
    {
        PrintPlayers(_joinedLobby);
    }
    void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Playuers in lobby "+lobby.Name + " "+lobby.Data["GameMode"].Value + " "+lobby.Data["Map"].Value );
        foreach(Player player in lobby.Players)
        {
            Debug.Log(player.Id + " "+player.Data["PlayerName"].Value);
        }
    }
    [Command]
    async void UpdateGameMode(string gameMode)
    {
        try
        {
            _hostLobby = await Lobbies.Instance.UpdateLobbyAsync(_hostLobby.Id,new UpdateLobbyOptions{
                Data = new Dictionary<string, DataObject>
                {
                    {"GameMode", new DataObject(DataObject.VisibilityOptions.Public,gameMode)},
                    {"Map", new DataObject(DataObject.VisibilityOptions.Public,"TeleTubbies")}
                }
            });
            _joinedLobby = _hostLobby;
            PrintPlayers(_hostLobby);
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    [Command]
    public async void UpdatePlayerName(string newPlayerName)
    {
        try{
            _playerName = newPlayerName;
            if(_joinedLobby == null)
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(newPlayerName);
                return;
            }
            await LobbyService.Instance.UpdatePlayerAsync(_joinedLobby.Id,AuthenticationService.Instance.PlayerId,new UpdatePlayerOptions
            {
                Data = new Dictionary<string,PlayerDataObject>
                {
                    { "PlayerName",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,_playerName)}
                }
            });
            PrintPlayers();
        }catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    [Command]
    public void LeaveLobby()
    {
        try{
            LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id,AuthenticationService.Instance.PlayerId);
            OnLeaveLobby.OnNext(_joinedLobby);
            _joinedLobby = null;
        }catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    [Command]
    public async void KickPlayer(string playerId)
    {
        try{
            Debug.Log("kickPlayer "+playerId + "at "+_joinedLobby.Id);
            await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id,playerId);
            PrintPlayers();
        }catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    [Command]
    public async void MigradeLobbyHost(string toPlayerId)
    {
        try{
            Debug.Log("Promote --------");
            Debug.Log("Host "+_joinedLobby.Id);
            Debug.Log("Toplayer "+toPlayerId);
            _joinedLobby = await Lobbies.Instance.UpdateLobbyAsync(_joinedLobby.Id,new UpdateLobbyOptions{
                HostId = toPlayerId
            });
            OnJoinedLobby.OnNext(_joinedLobby);
            //UpdatePlayerOptions updatePlayerOptions = new UpdatePlayerOptions();
            //Lobbies.Instance.UpdatePlayerAsync(_joinedLobby.Id,)
            PrintPlayers();
        }catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    [Command]
    async void DeleteLobby()
    {
        try{
            await LobbyService.Instance.DeleteLobbyAsync(_joinedLobby.Id);
        }catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
