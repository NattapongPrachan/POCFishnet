using System;
using System.Collections;
using System.Collections.Generic;
using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using FishNet.Managing;
using PlayEveryWare.EpicOnlineServices;
using QFSW.QC;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using PlayEveryWare.EpicOnlineServices.Samples;

public class EOSCommand : MonoBehaviour 
{
    EOSLobbyManager lobbyManager;
    private void Start()
    {
        
    }
    [Command]
    void DeclareLobbymanager()
    {
        lobbyManager = EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>();
    }
    void CreateLobby()
    {
        lobbyManager = EOSManager.Instance.GetOrCreateManager<EOSLobbyManager>();
        var lobby = new Lobby();
        lobby.BucketId = "1";
        lobby.MaxNumLobbyMembers = 8;

        Debug.Log("attribute "+lobby.Attributes.Count);
        var attribute = new LobbyAttribute();
        attribute.Key = "Bavaria";
        attribute.AsString = "Dielser";
        attribute.ValueType = AttributeType.String;
        lobby.Attributes.Add(attribute);
        foreach (var att in lobby.Attributes)
        {
            Debug.Log(att.Key + ": "+att.ValueType);
        }

        lobbyManager.CreateLobby(lobby,OnCreateLobbyCompleted);
        
    }

    private void OnCreateLobbyCompleted(Result result)
    {
        Debug.Log("OnCreateLobbyCompleted "+result);
    }
    [Command]
    void FindLobby(string lobbyId)
    {
        lobbyManager.SearchByLobbyId(lobbyId,_ =>{
            Debug.Log(_);
        });
    
    }
    // [Command]
    // void SearchLobbyByAttribute(string attributeKEy,string attributeValue)
    // {
    //     //lobbyManager.SearchByAttribute()
    //     lobbyManager.SearchByAttribute(attributeKEy,attributeValue,_ =>{
    //         Debug.Log("OnseaRch "+_.ToString());
    //     });
    // }
}
