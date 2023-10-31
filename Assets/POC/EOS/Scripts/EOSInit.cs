using System.Net.Mail;
using Epic.OnlineServices.Auth;
using PlayEveryWare.EpicOnlineServices;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Epic.OnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;
using Epic.OnlineServices.Connect;
using QFSW.QC;
using QFSW.QC.Containers;
using UnityEngine.UI;

public class EOSInit : SerializedMonoBehaviour,IEOSOnAuthLogin,IEOSOnAuthLogout,IEOSOnConnectLogin
{
    // Start is called before the first frame update
    [SerializeField]string productName;
    [SerializeField]string productVersion;
    [SerializeField]string clientID;
    [SerializeField]string clientSecret;
    [SerializeField]string productID;
    [SerializeField]string sandboxID;
    [SerializeField]string deploymentID;


    [SerializeField]LoginCredentialType credentialType;
    [SerializeField]string port;
    [SerializeField]string credentialId;

    [SerializeField]ExternalCredentialType externalCredentialType;
    [SerializeField]string token;
    [SerializeField]string displayName;
    [SerializeField]bool initialized;
    //DAta
    [SerializeField]object _clientData;
    [SerializeField]EpicAccountId _epicAccountId;
    [SerializeField]LoginStatus _loginStatus;
    //connect 
    [SerializeField]object _connectClientData;
    [SerializeField]string _productUserId;
    [SerializeField]LoginStatus _connectStatus;
    [SerializeField] Button b_play;
    ContinuanceToken _continuanceToken;
    /// <summary>
    /// LocalUserId	The ID of the lobby member requesting the removal. This must be the lobby owner.
    //  EOS_ProductUserId TargetUserId	The ID of the lobby member to be removed.
        // LocalUserId = id of authentication
        // ProductUserId = id of connection
    /// </summary>
    void Start()
    {
        AddListener();
    }
    [Command]
    public void AuthenLogin()
    {
        EOSManager.Instance.StartLoginWithLoginTypeAndToken(credentialType,port,credentialId,OnAuthLoginCallback);
    }
    private void OnAuthLoginCallback(Epic.OnlineServices.Auth.LoginCallbackInfo data)
    {
        Debug.Log("ResultCode "+data.ResultCode);
        Debug.Log("OnLoginCallback "+data.ClientData);
        Debug.Log("ContinuanceToken "+data.ContinuanceToken);
        Debug.Log("LocalUserId "+data.LocalUserId.ToString());
        Debug.Log("SelectedAccountId "+data.SelectedAccountId.ToString());
        Debug.Log("PinGrantInfo "+data.PinGrantInfo);
    }
    [Command]
    public void CreateUser()
    {
        CreateUserOptions createUserOptions = new CreateUserOptions{
             ContinuanceToken = _continuanceToken
        };
        
        EOSManager.Instance.GetEOSConnectInterface().CreateUser(ref createUserOptions,null,OnCreateUser);
    }


    [Command]
    public void ConnectLoginWithEpicId()
    {
        
        EOSManager.Instance.StartConnectLoginWithEpicAccount(EOSManager.Instance.GetLocalUserId(),OnConnectLogin);
    }
    [Command]
    public void ConnectLoginWithOption()
    {  
        CreateUserOptions createUserOptions = new CreateUserOptions{
             ContinuanceToken = _continuanceToken
        };
        EOSManager.Instance.GetEOSConnectInterface().CreateUser(ref createUserOptions,null,OnCreateUser);
    }

    private void OnCreateUser(ref CreateUserCallbackInfo data)
    {
        Debug.Log("CreateUser "+data.ResultCode);
        CopyUserAuthTokenOptions tokenOptions = new CopyUserAuthTokenOptions();
        EOSManager.Instance.GetEOSAuthInterface().CopyUserAuthToken(ref tokenOptions,_epicAccountId,out Token? token);
        
        var loginOptions = new Epic.OnlineServices.Connect.LoginOptions() {
            Credentials = new Epic.OnlineServices.Connect.Credentials() {
                Type = externalCredentialType,
                Token = token.Value.AccessToken
            }
        };
        EOSManager.Instance.StartConnectLoginWithOptions(loginOptions,OnConnectWithCreate);
    }
    

    [Command]
    public void ConnectLoginWithDeviceId()
    {
        EOSManager.Instance.StartConnectLoginWithDeviceToken(displayName, result => {

            Debug.Log("OnConnectDeviceId " + result.ResultCode);
            switch (result.ResultCode)
            {
                case Result.NotFound:
                    CreateDeviceId();
                break;
                case Result.Success:
                   // b_play.gameObject.SetActive(true);
                    break;
            }
        });
    }

    [Command]
    public void CreateDeviceId()
    {
        CreateDeviceIdOptions createDeviceIdOptions = new CreateDeviceIdOptions
        {
            DeviceModel = SystemInfo.deviceModel
        };
        EOSManager.Instance.GetEOSConnectInterface().CreateDeviceId(ref createDeviceIdOptions, null, OnCreateDeviceId);
    }

    private void OnCreateDeviceId(ref CreateDeviceIdCallbackInfo data)
    {
        Debug.Log("OnCreateDeviceId " + data.ResultCode);
        switch(data.ResultCode)
        {
            case Result.Success:
                ConnectLoginWithDeviceId();
            break;
        }
    }

    private void OnEOSLoginCallback(Epic.OnlineServices.Auth.LoginCallbackInfo loginCallbackInfo)
    {
        EOSManager.Instance.StartConnectLoginWithDeviceToken(displayName,OnConnectLogin);
    }

    private void OnConnectWithCreate(Epic.OnlineServices.Connect.LoginCallbackInfo loginCallbackInfo)
    {
        Debug.Log("OnConnectLogin "+loginCallbackInfo.ResultCode);
    }
    
    public void OnConnectLogin(Epic.OnlineServices.Connect.LoginCallbackInfo loginCallbackInfo)
    {
        Debug.Log("OnConnectLogin "+loginCallbackInfo.ResultCode);
        Debug.Log("continuance token "+loginCallbackInfo.ContinuanceToken);
        switch(loginCallbackInfo.ResultCode)
        {
            case Result.InvalidUser :
                CreateUser();
                break;
            case Result.AccessDenied:
                CreateUser();
                break; 
        }
    }
    
    

    

    void AddListener()
    {
         var statusChangeOptions = new Epic.OnlineServices.Auth.AddNotifyLoginStatusChangedOptions
        {

        };
        Epic.OnlineServices.Connect.AddNotifyLoginStatusChangedOptions addNotifyLoginStatusChangedOptions = new Epic.OnlineServices.Connect.AddNotifyLoginStatusChangedOptions
        {

        };
        ulong v = EOSManager.Instance.GetEOSAuthInterface().AddNotifyLoginStatusChanged(ref statusChangeOptions,null,OnAuthLoginStatusChangeCallback);
        EOSManager.Instance.GetEOSConnectInterface().AddNotifyLoginStatusChanged(ref addNotifyLoginStatusChangedOptions,null,OnConnectLoginStatusChangedCallback);
    }

    

    

    public void OnAuthLogout(LogoutCallbackInfo logoutCallbackInfo)
    {
        Debug.Log("OnAuthLogout");
    }

    public void OnAuthLogin(Epic.OnlineServices.Auth.LoginCallbackInfo authLoginCallbackInfo)
    {
        Debug.Log("OnAuthLogin");
    }

    //connect

    private void OnConnectLoginCallback(ref Epic.OnlineServices.Connect.LoginCallbackInfo data)
    {
        Debug.Log("OnConnectLoginCallback "+data.ResultCode);
    }
    private void OnAuthLoginStatusChangeCallback(ref Epic.OnlineServices.Auth.LoginStatusChangedCallbackInfo data)
    {
        _loginStatus = data.CurrentStatus;
       // _epicAccountId = data.LocalUserId.ToString();
        _clientData = data.ClientData;
        switch (data.CurrentStatus)
        {
            case  LoginStatus.LoggedIn:  
            break;
            case LoginStatus.NotLoggedIn:
            break;
            case LoginStatus.UsingLocalProfile :
            break;
        }
    }
    private void OnConnectLoginStatusChangedCallback(ref Epic.OnlineServices.Connect.LoginStatusChangedCallbackInfo data)
    {
        Debug.Log("OnConnectLoginStatusChangedCallback " + data);
        _connectClientData = data.ClientData;
        _productUserId = data.LocalUserId.ToString();
        _connectStatus = data.CurrentStatus;
    }
    private void OnEnable() {
       
        //EOSManager.Instance.AddAuthLoginListener(this);
        //EOSManager.Instance.AddAuthLogoutListener(this);
        //EOSManager.Instance.AddConnectLoginListener(this);
    }
    private void OnDisable() {
        //EOSManager.Instance.RemoveAuthLoginListener(this);
        //EOSManager.Instance.RemoveAuthLogoutListener(this);
        //EOSManager.Instance.RemoveConnectLoginListener(this);
    }

    
}
