using System.Collections;
using System.Collections.Generic;
using Epic.OnlineServices.Auth;
using PlayEveryWare.EpicOnlineServices;
using UnityEngine;

public class EOSAuth : MonoBehaviour , IEOSOnAuthLogin,IEOSOnConnectLogin,IEOSOnAuthLogout
{
    private void Start()
    {
        
    }
    public void OnAuthLogin(LoginCallbackInfo loginCallbackInfo)
    {
        Debug.Log("OnAuthLogin "+loginCallbackInfo);
    }

    public void OnConnectLogin(Epic.OnlineServices.Connect.LoginCallbackInfo loginCallbackInfo)
    {
        Debug.Log("OnConnectLogin "+loginCallbackInfo);
    }
     public void OnAuthLogout(LogoutCallbackInfo logoutCallbackInfo)
    {
         Debug.Log("OnAuthLogout "+logoutCallbackInfo);
    }
    private void OnEnable() {
        EOSManager.Instance.AddAuthLoginListener(this);
        EOSManager.Instance.AddConnectLoginListener(this);
        EOSManager.Instance.AddAuthLogoutListener(this);
    }
    private void OnDisable() {
        EOSManager.Instance.RemoveAuthLoginListener(this);
        EOSManager.Instance.RemoveConnectLoginListener(this);
        EOSManager.Instance.RemoveAuthLogoutListener(this);
    }

   
}
