using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using Unity.Services.Lobbies.Models;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using Unity.Services.Lobbies;

[System.Serializable]
public class JoinLobbyEvent : UnityEvent<Lobby>
{
}
public class UILobby : SerializedMonoBehaviour
{
    [SerializeField]Transform _contentTransform;
    [SerializeField]LobbyEC _lobbyElementPrefab;
    [SerializeField]int _lobbyShowLimit = 25;
    
    [SerializeField]List<LobbyEC> _lobbies;
    public JoinLobbyEvent joinLobbyEvent;
    
    private void Start()
    {
        _lobbies = new List<LobbyEC>();
        CreatePoolElement();
        TestLobby.OnLobbyUpdated.Subscribe(LobbyUpdated).AddTo(this);
        TestLobby.OnJoinedLobby.Subscribe(JoinedLobby).AddTo(this);
        TestRelay.OnStartConnection.Subscribe(_ =>{
            GetComponent<Canvas>().enabled = false;
        }).AddTo(this);
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
                Debug.Log("Lobby "+lobby);
                Debug.Log("Code "+lobby.LobbyCode);
            _lobbies[i].gameObject.SetActive(true);
            _lobbies[i].SetupLobbyData(lobbiesUpdate[i]);
        }
    }
    void ClearContent()
    {

    }

    private void CreatePoolElement()
    {
        for (int i = 0; i < _lobbyShowLimit ; i++)
        {
            _lobbies.Add(Instantiate(_lobbyElementPrefab,_contentTransform));
        }
    }
}
