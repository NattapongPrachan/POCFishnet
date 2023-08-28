using System.Collections;
using System.Collections.Generic;
using Grandora.Manager;
using UnityEngine;
[RequireComponent(typeof(Camera))]
public class CameraRegisterPoint : MonoBehaviour
{
    private void Start() {
        CameraManager.Instance.SetupCamera(GetComponent<Camera>());
    }
}
