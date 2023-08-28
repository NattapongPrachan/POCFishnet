using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkmanagerUI : MonoBehaviour
{
    [SerializeField]Button b_server;
    [SerializeField]Button b_host;
    [SerializeField]Button b_client;
    // Start is called before the first frame update
    private void Awake() {
        b_server.onClick.AddListener(()=>{
           // NetworkManager.Singleton.StartServer();
        });
        b_host.onClick.AddListener(()=>{
            ///NetworkManager.Singleton.StartHost();
        });
        b_client.onClick.AddListener(()=>{
            //NetworkManager.Singleton.StartClient();
        });
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
