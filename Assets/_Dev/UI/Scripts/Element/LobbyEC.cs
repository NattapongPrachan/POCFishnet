using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Sirenix.OdinInspector;
using PlayEveryWare.EpicOnlineServices.Samples;
using Epic.OnlineServices.Lobby;
using PlayEveryWare.EpicOnlineServices;
using Epic.OnlineServices;
using System;
// EC = ElementContent ตั้งชื่อไงดีวะ
public class LobbyEC : SerializedMonoBehaviour
{
    [SerializeField]TextMeshProUGUI _roomNameTxt;
    [SerializeField]TextMeshProUGUI _gameModeTxt;
    [SerializeField]TextMeshProUGUI _playerCountTxt;
    [SerializeField]Button b_join;
    [SerializeField]EOSLobbyManager lobbyManager;
    [SerializeField]Lobby _lobby;
    [SerializeField]LobbyDetails _lobbyDetails;
    private void Start()
    {
        b_join.OnClickAsObservable().Subscribe(_ =>{
            //lobbyManager.JoinLobbyById(_lobby.Id);
            // JoinLobbyOptions joinLobbyOptions = new JoinLobbyOptions{
            //     LocalUserId = EOSManager.Instance.GetProductUserId(),
            //     PresenceEnabled = true,
            //     LobbyDetailsHandle = _lobbyDetails
            // };
            EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().JoinLobby(_lobby.Id,_lobbyDetails,true,result =>{
                Debug.Log("JoinLobbyCompleted "+result);
                Debug.Log("GetcurrentLobby "+EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().GetCurrentLobby().Id);
                
            });
            //EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().JoinLobby()
            //EOSManager.Instance.GetEOSLobbyInterface().JoinLobby(ref joinLobbyOptions,null,OnJoinLobby);
            //EOSManager.Instance.GetEOSLobbyInterface()
        }).AddTo(this);
    }

    private void OnJoinLobby(ref JoinLobbyCallbackInfo data)
    {
       Debug.Log(data.ResultCode);
    }


    public void SetupLobbyData(Lobby lobby,LobbyDetails lobbyDetails)
    {
        _lobby = lobby;
        _lobbyDetails = lobbyDetails;

        
        _roomNameTxt.text = lobby.LobbyOwnerAccountId.ToString();
        _gameModeTxt.text = lobby.BucketId;
        _playerCountTxt.text =lobby.MaxNumLobbyMembers-lobby.AvailableSlots+"/"+lobby.MaxNumLobbyMembers;

        
    }
}
