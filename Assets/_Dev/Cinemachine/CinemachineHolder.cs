using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
namespace Grandora.GameCinimachine
{
    public class CinemachineHolder : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField]CinemachineVirtualCameraBase[] virtualCameraBases;

        public CinemachineVirtualCameraBase[] GetVirualCameras
        {
            get{
                return virtualCameraBases;
            }
        }
    }
}
