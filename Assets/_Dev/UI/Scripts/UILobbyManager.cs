using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using PlayEveryWare.EpicOnlineServices;
using Epic.OnlineServices.Connect;

public class UILobbyManager : MonoBehaviour ,IEOSOnAuthLogout, IEOSOnConnectLogin
{
    [SerializeField]GameObject _uiLobbies;
    private void Awake()
    {
        // FEOSLobbies.OnLobbyJoined.Subscribe(_ =>{
        //     _uiLobby.gameObject.SetActive(true);
        // }).AddTo(this);
    }

    public void OnAuthLogout(Epic.OnlineServices.Auth.LogoutCallbackInfo logoutCallbackInfo)
    {
       _uiLobbies.gameObject.SetActive(false);
    }

    public void OnConnectLogin(LoginCallbackInfo loginCallbackInfo)
    {
        _uiLobbies.gameObject.SetActive(true);
    }

    private void Start()
    {
        FEOSLobbies.OnLobbyJoined.Subscribe(_ =>{
            
        }).AddTo(this);
    }
    private void OnEnable() {
        EOSManager.Instance.AddAuthLogoutListener(this);
        EOSManager.Instance.AddConnectLoginListener(this);
    }
    private void OnDisable() {
        EOSManager.Instance.RemoveAuthLogoutListener(this);
        EOSManager.Instance.RemoveConnectLoginListener(this);
    }
}
