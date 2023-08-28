using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.Events;
using Sirenix.OdinInspector;
// EC = ElementContent ตั้งชื่อไงดีวะ
public class LobbyEC : SerializedMonoBehaviour
{
    [SerializeField]TextMeshProUGUI _roomNameTxt;
    [SerializeField]TextMeshProUGUI _gameModeTxt;
    [SerializeField]TextMeshProUGUI _playerCountTxt;
    [SerializeField]Button b_join;
    [SerializeField]TestLobby lobbyManager;
    [SerializeField]Lobby _lobby;
    private void Start()
    {
        b_join.OnClickAsObservable().Subscribe(_ =>{
            Debug.Log("Lobbycode "+_lobby.Id);
            lobbyManager.JoinLobbyById(_lobby.Id);
        }).AddTo(this);
    }
    public void SetupLobbyData(Lobby lobby)
    {
        _lobby = lobby;
        _roomNameTxt.text = lobby.Name;
        _gameModeTxt.text = lobby.Data["GameMode"].Value;
        _playerCountTxt.text = lobby.Players.Count+"/"+lobby.MaxPlayers;
    }
}
