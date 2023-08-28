using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Authentication;

public class PlayerRoomEC : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI _playerNameTxt;
    [SerializeField]Button b_promote,b_kick,b_leave;
    Player _player;
    public void SetupPlayerData(Player playerData)
    {
        _player = playerData;
        _playerNameTxt.text = _player.Data["PlayerName"].Value;
        
        // _player.ObserveEveryValueChanged(_ => _.).Subscribe(isHost =>{
        //     b_promote.gameObject.SetActive(isHost);
        //     b_kick.gameObject.SetActive(isHost);
        // }).AddTo(this);
    }
    public void UpdateHost(string hostId,string playerId)
    {
        var isHost = hostId == playerId;
        var isMe = _player.Id == playerId;
        var canEdit = isHost && !isMe;
        b_promote.gameObject.SetActive(canEdit);
        b_kick.gameObject.SetActive(canEdit);
        b_leave.gameObject.SetActive(isMe);
    }
    public void PromotePlayerToHost()
    {
        TestLobby.Instance.MigradeLobbyHost(_player.Id);
    }
    public void LeaveRoom()
    {

    }
    public void KickPlayer()
    {
        TestLobby.Instance.KickPlayer(_player.Id);
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
