using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using TMPro;
using PlayEveryWare.EpicOnlineServices.Samples;
using Sirenix.OdinInspector;
using PlayEveryWare.EpicOnlineServices;
using Unity.VisualScripting;
public class PlayerRoomEC : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI _playerNameTxt;
    [SerializeField]Button b_promote,b_kick,b_leave;
    Lobby _lobby;
    LobbyMember _lobbyMember;
    [SerializeField]ReactiveCollection<LobbyAttribute> _lobbyAttributes = new ReactiveCollection<LobbyAttribute>();
    [SerializeField]ReactiveDictionary<string,LobbyAttribute> _attributeDictionary = new ReactiveDictionary<string, LobbyAttribute>();
    [SerializeField]bool _isOwner;
    [SerializeField]bool _isMe;
    //Player _player;
    // public void SetupPlayerData(Player playerData)
    // {
    //    // _player = playerData;
    //    //_playerNameTxt.text = _player.Data["PlayerName"].Value;
    //     //
    //     // _player.ObserveEveryValueChanged(_ => _.).Subscribe(isHost =>{
    //     //     b_promote.gameObject.SetActive(isHost);
    //     //     b_kick.gameObject.SetActive(isHost);
    //     // }).AddTo(this);
    // }
    public void SetupData(Lobby lobby ,LobbyMember lobbyMember)
    {
        _lobby = lobby;
        _lobbyMember = lobbyMember;
        _playerNameTxt.text = lobbyMember.ProductId.ToString();
        _lobbyAttributes = _lobby.Attributes.ToReactiveCollection();
        _attributeDictionary = lobbyMember.MemberAttributes.ToReactiveDictionary();

        Debug.Log("_lobbyMember.ProductId "+_lobbyMember.ProductId);
        Debug.Log("_lobby.LobbyOwner "+_lobby.LobbyOwner.ToString());

        _isOwner = _lobby.IsOwner(EOSManager.Instance.GetProductUserId());
        _isMe = lobbyMember.ProductId == EOSManager.Instance.GetProductUserId();
        
        b_leave.gameObject.SetActive(_isMe);
        b_kick.gameObject.SetActive(_isOwner&&!_isMe);
        b_promote.gameObject.SetActive(_isOwner&&!_isMe);

        foreach (var player in lobbyMember.MemberAttributes)
        {
            Debug.Log("Key : "+player.Key + " value : "+player.Value);
        }
    }
    public void UpdateHost(string hostId,string playerId)
    {
        // var isHost = hostId == playerId;
        // var isMe = _player.Id == playerId;
        // var canEdit = isHost && !isMe;
        // b_promote.gameObject.SetActive(canEdit);
        // b_kick.gameObject.SetActive(canEdit);
        // b_leave.gameObject.SetActive(isMe);
    }
    public void PromotePlayerToHost()
    {
       Debug.Log("PromotePlayerToHost "+_lobbyMember.ProductId);
       EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().PromoteMember(_lobbyMember.ProductId,_ =>{
        Debug.Log("promoteplayer completed "+_);
       });
    }
    public void KickPlayer()
    {
       EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().KickMember(_lobbyMember.ProductId,null);
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
