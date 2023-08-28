using System.Collections;
using System.Collections.Generic;
using Grandora.Manager;
using UnityEngine;
[DefaultExecutionOrder(101)]
public class VirtualCameraRegister : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var virtualCamera = GetComponent<Cinemachine.CinemachineVirtualCameraBase>();
        if(virtualCamera != null)
            CameraManager.Register(virtualCamera);
            CameraManager.SwitchVirtualCamera(virtualCamera);
    }
}
