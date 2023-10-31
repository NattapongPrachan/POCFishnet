using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
using PlayEveryWare.EpicOnlineServices.Samples;
using Sirenix.OdinInspector;
using PlayEveryWare.EpicOnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Lobby;
using System;
using Epic.OnlineServices;
using QFSW.QC;
using FishNet.Plugins.FishyEOS.Util;

public class FEOSLobby : SerializedMonoBehaviour
{
    EOSLobbyManager _lobbyManager;
    LobbyInterface _lobbyInterface;
    Lobby _lobby;
    ulong idNotifyLoginStatusChanged;
    void Start()
    {
        AddListener();
    }
    void AddListener()
    {
        _lobbyManager = EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>();
        _lobbyManager.AddNotifyMemberUpdateReceived(OnMemberUpdate);
    }
    private void OnMemberUpdate(string LobbyId, ProductUserId MemberId)
    {
        Debug.Log("LobbyId "+LobbyId);
        Debug.Log("MemberId "+MemberId);
        _lobby = _lobbyManager.GetCurrentLobby();
    }
    void OnEnable()
    {
        EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().AddNotifyLobbyChange(OnNotifyLobbyChange);
        //EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().AddNotifyLobbyUpdate(OnNotifyLobbyUpdate);
        //EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>().AddNotifyMemberUpdateReceived(OnNotifyMemberUpdateReceived);
    }

    private void OnNotifyMemberUpdateReceived(string LobbyId, ProductUserId MemberId)
    {
       // Debug.Log("OnNotifyMemberUpdateReceived");
    }

    private void OnNotifyLobbyUpdate()
    {
        //Debug.Log("OnNotifyLobbyUpdate");
    }

    private void OnNotifyLobbyChange()
    {
        Debug.Log("OnNotifyLobbyChange");
    }

    private void OnDisable()
    {
        
    }
}
