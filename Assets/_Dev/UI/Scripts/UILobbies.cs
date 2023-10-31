using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using PlayEveryWare.EpicOnlineServices.Samples;
using PlayEveryWare.EpicOnlineServices;
using Epic.OnlineServices.Lobby;
using Epic.OnlineServices;
using TMPro;

[System.Serializable]
public class JoinLobbyEvent : UnityEvent<Lobby>
{
}
public class UILobbies : SerializedMonoBehaviour
{
    [SerializeField] FEOSLobbies _feosLobbies;
    [SerializeField]GameObject _uiLobbies;
    [SerializeField]Transform _contentTransform;
    [SerializeField]LobbyEC _lobbiesElementPrefab;
    [SerializeField]int _lobbiesShowLimit = 25;

    [Header("Create&Search")]
    [SerializeField] TMP_InputField _inputBucketId;
    
    [SerializeField]List<LobbyEC> _lobbies;
    public JoinLobbyEvent joinLobbyEvent;
    EOSLobbyManager lobbyManager;
    private void Awake()
    {
        //UILobbies
        _lobbies = new List<LobbyEC>();
        CreatePoolElement();
        FEOSLobbies.OnLobbiesUpdated.Subscribe(_ => {
            OnLobbiesUpdated(_);
        }).AddTo(this);
    }

    private void OnLobbiesUpdated(Dictionary<Lobby, LobbyDetails> lobbiesUpdate)
    {
        _lobbies.ForEach(lobby => lobby.gameObject.SetActive(false));
        var index = 0;
        foreach (var lobby in lobbiesUpdate)
        {
             _lobbies[index].gameObject.SetActive(true);
             _lobbies[index].SetupLobbyData(lobby.Key,lobby.Value);
             index ++;
        }
        // for (int i = 0; i < lobbiesUpdate.Count; i++)
        // {
        //     var lobby = lobbiesUpdate[i];
               
        //     _lobbies[i].gameObject.SetActive(true);
        //     _lobbies[i].SetupLobbyData(lobbiesUpdate[i]);
        // }
    }

    private void Start()
    {
        
        lobbyManager = EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>();

        
        //TestLobby.OnLobbyUpdated.Subscribe(LobbyUpdated).AddTo(this);
        //TestLobby.OnJoinedLobby.Subscribe(JoinedLobby).AddTo(this);
        // TestRelay.OnStartConnection.Subscribe(_ =>{
        //     GetComponent<Canvas>().enabled = false;
        // }).AddTo(this);
    }

    private void JoinedLobby(Lobby lobby)
    {
        joinLobbyEvent?.Invoke(lobby);
    }

    void LobbyUpdated(List<Lobby> lobbiesUpdate)
    {
        _lobbies.ForEach(lobby => lobby.gameObject.SetActive(false));
        for (int i = 0; i < lobbiesUpdate.Count; i++)
        {
            var lobby = lobbiesUpdate[i];
               
            _lobbies[i].gameObject.SetActive(true);
           // _lobbies[i].SetupLobbyData(lobbiesUpdate[i]);
        }
    }
    void ClearContent()
    {

    }

    private void CreatePoolElement()
    {
        for (int i = 0; i < _lobbiesShowLimit ; i++)
        {
            _lobbies.Add(Instantiate(_lobbiesElementPrefab,_contentTransform));
        }
    }
    private void OnConnectLoginStatusChangeCallback(ref Epic.OnlineServices.Connect.LoginStatusChangedCallbackInfo data)
    {
        Debug.Log("Lobbies OnConnectLoginStatusChangeCallback "+data.CurrentStatus);
        switch(data.CurrentStatus)
        {
            case LoginStatus.LoggedIn:
                _uiLobbies.gameObject.SetActive(true);
            break;
            case LoginStatus.NotLoggedIn:
                _uiLobbies.gameObject.SetActive(false);
            break;
        }
    }
    private void OnEnable() {
        Epic.OnlineServices.Connect.AddNotifyLoginStatusChangedOptions connectLoginOption = new Epic.OnlineServices.Connect.AddNotifyLoginStatusChangedOptions
        {
        
        };
        EOSManager.Instance.GetEOSConnectInterface().AddNotifyLoginStatusChanged(ref connectLoginOption,null,OnConnectLoginStatusChangeCallback);
    }

    public void SearchLobbyByBucketId()
    {
        if (string.IsNullOrEmpty(_inputBucketId.text)) return;
        _feosLobbies.SearchLobbyByAttribute(_inputBucketId.text);
    }
    public void CreateLobbyBuBucketId()
    {
        if (string.IsNullOrEmpty(_inputBucketId.text)) return;
        _feosLobbies.CreateLobby(_inputBucketId.text);
    }
}
