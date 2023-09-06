using System.Collections;
using System.Collections.Generic;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Sessions;
using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;
using QFSW.QC;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UIElements;

public class FEOSSession : IEOSOnAuthLogin,IEOSOnAuthLogout, IEOSOnConnectLogin
{
    SessionsInterface _sessionInterface;
    EOSSessionsManager GetSessionsManager
    {
        get { return EOSManager.Instance.GetOrCreateManager<EOSSessionsManager>(); }
    }
    public void OnAuthLogin(LoginCallbackInfo loginCallbackInfo)
    {
        _sessionInterface = EOSManager.Instance.GetEOSSessionsInterface();
    }

    [Command]
    void SearchSession()
    {

    }

    public void OnAuthLogout(LogoutCallbackInfo logoutCallbackInfo)
    {
        
    }
    public void OnConnectLogin(Epic.OnlineServices.Connect.LoginCallbackInfo loginCallbackInfo)
    {
       
    }
    private void OnEnable()
    {
        EOSManager.Instance.AddAuthLoginListener(this);
        EOSManager.Instance.AddAuthLogoutListener(this);
        EOSManager.Instance.AddConnectLoginListener(this);
    }
    private void OnDisable() {
        EOSManager.Instance.RemoveAuthLoginListener(this);
        EOSManager.Instance.RemoveAuthLogoutListener(this);
        EOSManager.Instance.RemoveConnectLoginListener(this);
    }

    
}
