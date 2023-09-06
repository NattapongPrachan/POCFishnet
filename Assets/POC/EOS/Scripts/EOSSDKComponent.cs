// This code is provided for demonstration purposes and is not intended to represent ideal practices.
using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Lobby;
using Epic.OnlineServices.Logging;
using Epic.OnlineServices.Platform;
using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class EOSSDKComponent : MonoBehaviour
{
    // Set these values as appropriate. For more information, see the Developer Portal documentation.
    public string m_ProductName = "MyUnityApplication";
    public string m_ProductVersion = "1.0";
    public string m_ProductId = "";
    public string m_SandboxId = "";
    public string m_DeploymentId = "";
    public string m_ClientId = "";
    public string m_ClientSecret = "";
    public LoginCredentialType m_LoginCredentialType = LoginCredentialType.AccountPortal;
    public ExternalAccountType externalAccountType = ExternalAccountType.Epic;
    // These fields correspond to \<see cref="Credentials.Id" /> and \<see cref="Credentials.Token" />,
    // and their use differs based on the login type. For more information, see \<see cref="Credentials" />
    // and the Auth Interface documentation.
    public string m_LoginCredentialId = null;
    public string m_LoginCredentialToken = null;

    private static PlatformInterface s_PlatformInterface;
    private const float c_PlatformTickInterval = 0.1f;
    private float m_PlatformTickTimer = 0f;

    // If we're in editor, we should dynamically load and unload the SDK between play sessions.
    // This allows us to initialize the SDK each time the game is run in editor.
#if UNITY_EDITOR
    [DllImport("Kernel32.dll")]
    private static extern IntPtr LoadLibrary(string lpLibFileName);

    [DllImport("Kernel32.dll")]
    private static extern int FreeLibrary(IntPtr hLibModule);

    [DllImport("Kernel32.dll")]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

    private IntPtr m_LibraryPointer;
#endif

    private void Awake()
    {
        return;
#if UNITY_EDITOR
        var libraryPath = "Assets/EOS/Runtime/Windows/x64/" + Config.LibraryName;

        m_LibraryPointer = LoadLibrary(libraryPath);
        if (m_LibraryPointer == IntPtr.Zero)
        {
            throw new Exception("Failed to load library" + libraryPath);
        }

        Bindings.Hook(m_LibraryPointer, GetProcAddress);
#endif
    }

    private void OnApplicationQuit()
    {
        if (s_PlatformInterface != null)
        {
            s_PlatformInterface.Release();
            s_PlatformInterface = null;
            PlatformInterface.Shutdown();
        }

#if UNITY_EDITOR
        if (m_LibraryPointer != IntPtr.Zero)
        {
            Bindings.Unhook();

            // Free until the module ref count is 0
            while (FreeLibrary(m_LibraryPointer) != 0) { }
            m_LibraryPointer = IntPtr.Zero;
        }
#endif
    }

    void Start()
    {

    }

    private void OnStartConnectComplete(Epic.OnlineServices.Connect.LoginCallbackInfo loginCallbackInfo)
    {
         Debug.Log("COnStartConnectComplete "+loginCallbackInfo.ResultCode);
    }

    private void OnCreateCompleted(Result result)
    {
        Debug.Log("Create Room Completed "+result);
    }

    // Calling tick on a regular interval is required for callbacks to work.
    private void Update()
    {
        if (s_PlatformInterface != null)
        {
            m_PlatformTickTimer += Time.deltaTime;

            if (m_PlatformTickTimer >= c_PlatformTickInterval)
            {
                m_PlatformTickTimer = 0;
                s_PlatformInterface.Tick();
            }
        }
    }
}