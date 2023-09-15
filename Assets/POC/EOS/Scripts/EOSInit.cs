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
    ContinuanceToken _continuanceToken;
    /// <summary>
    /// LocalUserId	The ID of the lobby member requesting the removal. This must be the lobby owner.
    //  EOS_ProductUserId TargetUserId	The ID of the lobby member to be removed.
        // LocalUserId = id of authentication
        // ProductUserId = id of connection
    /// </summary>
    void Start()
    {
        var authInterface = EOSManager.Instance.GetEOSAuthInterface();
        var statusInterface = EOSManager.Instance.GetEOSStatsInterface();
        var sessionInterface = EOSManager.Instance.GetEOSSessionsInterface();
        var lobbyInterface = EOSManager.Instance.GetEOSLobbyInterface();
        var connectInterface = EOSManager.Instance.GetEOSConnectInterface();
        AddListener();
        // Epic.OnlineServices.Auth.LoginOptions loginOptions = new Epic.OnlineServices.Auth.LoginOptions
        // {
        //      Credentials = new Epic.OnlineServices.Auth.Credentials
        //      {
        //          Id = port,
        //           Token = credentialId,
        //            Type = credentialType,
        //             ExternalType = externalCredentialType
        //      }
        // };
        
        // Epic.OnlineServices.Auth.LoginOptions loginOptions = new Epic.OnlineServices.Auth.LoginOptions
        // {
        //      Credentials = new Epic.OnlineServices.Auth.Credentials
        //      {
        //         Type = LoginCredentialType.DeviceCode,
        //         ExternalType = ExternalCredentialType.DeviceidAccessToken
        //      }
        // };
        // EOSManager.Instance.StartLoginWithLoginOptions(loginOptions,OnAuthLoginCallback);


        ///EOSManager.Instance.StartLoginWithLoginTypeAndToken(credentialType,port,credentialId,OnEOSLoginCallback);
        //EOSManager.Instance.GetEOSAuthInterface().Login(ref loginOptions,null,OnAuthLoginCallback);
        
        //EOSManager.Instance.GetEOSConnectInterface().Login(ref connectLoginOption,null,OnConnectLoginCallback);
        //EOSManager.Instance.StartConnectLoginWithOptions(Epic.OnlineServices.ExternalCredentialType.DeviceidAccessToken)

       // ConnectLogin();
    }
    [Command]
    void AuthenLogin()
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
        //_epicAccountId = data.LocalUserId;
        //_continuanceToken = data.ContinuanceToken;
        //Debug.Log("_continuanceToken "+_continuanceToken);

        // EOSManager.Instance.CreateConnectUserWithContinuanceToken(data.ContinuanceToken, (Epic.OnlineServices.Connect.CreateUserCallbackInfo createUserCallbackInfo) =>
        //         {
        //             print("Creating new connect user "+createUserCallbackInfo.ResultCode);
        //             if (createUserCallbackInfo.ResultCode == Result.Success)
        //             {
        //                 //ConfigureUIForLogout();
        //             }
        //             else
        //             {
        //                 //ConfigureUIForLogin();
        //             }
        //         });
       // EOSManager.Instance.GetEOSAuthInterface().GetSelectedAccountId(data.LocalUserId,out _epicAccountId);
    }
    [Command]
    void CreateUser()
    {
        CreateUserOptions createUserOptions = new CreateUserOptions{
             ContinuanceToken = _continuanceToken
        };
        
        EOSManager.Instance.GetEOSConnectInterface().CreateUser(ref createUserOptions,null,OnCreateUser);
        //EOSManager.Instance.GetEOSConnectInterface().CreateDeviceId(ref createDeviceIdOptions,null,OnCreateDeviceID);
    }


    [Command]
    void ConnectLoginWithEpicId()
    {
        
        EOSManager.Instance.StartConnectLoginWithEpicAccount(EOSManager.Instance.GetLocalUserId(),OnConnectLogin);
    }
    [Command]
    void ConnectLoginWithOption()
    {
        // CopyUserAuthTokenOptions tokenOptions = new CopyUserAuthTokenOptions();
        // EOSManager.Instance.GetEOSAuthInterface().CopyUserAuthToken(ref tokenOptions,_epicAccountId,out Token? token);
        
        // var loginOptions = new Epic.OnlineServices.Connect.LoginOptions() {
        //     Credentials = new Epic.OnlineServices.Connect.Credentials() {
        //         Type = externalCredentialType,
        //         Token = token.Value.AccessToken
        //     }
        // };
        // EOSManager.Instance.StartConnectLoginWithOptions(loginOptions,OnConnectLogin);
        
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
    void CreateDeviceId()
    {
        CreateDeviceIdOptions createDeviceIdOptions = new CreateDeviceIdOptions{
            DeviceModel = SystemInfo.deviceModel
        };
        EOSManager.Instance.GetEOSConnectInterface().CreateDeviceId(ref createDeviceIdOptions,null,OnCreateDeviceId);
    }

    private void OnCreateDeviceId(ref CreateDeviceIdCallbackInfo data)
    {
        Debug.Log("OnCreateDeviceId "+data.ResultCode);
    }

    [Command]
    void ConnectLoginWithDeviceId()
    {
        EOSManager.Instance.StartConnectLoginWithDeviceToken(displayName,OnConnectLogin);
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
        }
    }
    
    

    private void OnCreateDeviceID(ref CreateDeviceIdCallbackInfo data)
    {
        Debug.Log("OnCreateDeviceID "+data.ResultCode);
        Debug.Log(data.ClientData.ToString());

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
