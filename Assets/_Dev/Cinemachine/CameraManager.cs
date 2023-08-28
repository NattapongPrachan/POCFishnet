using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
namespace Grandora.Manager
{
    [DefaultExecutionOrder(50)]
    public class CameraManager : MonoInstance<CameraManager>
    {
        public static List<CinemachineVirtualCameraBase> _cinemachineVirtualCameras = new List<CinemachineVirtualCameraBase>();
        public static CinemachineVirtualCameraBase ActiveCamera = null;
        public Camera Camera = null;
        public static int cameraIndex;
        public void SetupCamera(Camera camera)
        {
            Camera = camera;
        }
        public static void Register(CinemachineVirtualCameraBase virtualCameraBase)
        {
            if(!_cinemachineVirtualCameras.Contains(virtualCameraBase))
            {
                EnableVirtualCamera(virtualCameraBase);
                _cinemachineVirtualCameras.Add(virtualCameraBase);
            }
        }
        public static void Unregister(CinemachineVirtualCameraBase virtualCameraBase)
        {
            if(_cinemachineVirtualCameras.Contains(virtualCameraBase))
            {
               DisableVirtualCamera(virtualCameraBase);
                _cinemachineVirtualCameras.Remove(virtualCameraBase);
            }
            ForceAutoCamera();
        }
        public static bool IsActive(CinemachineVirtualCameraBase virtualCameraBase)
        {
            return ActiveCamera == virtualCameraBase;
        }
        public static void ForceAutoCamera()
        {
            if(ActiveCamera == null && _cinemachineVirtualCameras.Count > 0)
                SwitchVirtualCamera(_cinemachineVirtualCameras[0]);
        }
        public static void NextVirtualCamera()
        {
            cameraIndex = (cameraIndex >= _cinemachineVirtualCameras.Count-1) ? 0 :cameraIndex++;
            SwitchVirtualCamera(_cinemachineVirtualCameras[cameraIndex]);
        }
        public static void PreviousVirtualCamera()
        {
            cameraIndex = (cameraIndex <= 0) ? _cinemachineVirtualCameras.Count - 1  :cameraIndex--;
            SwitchVirtualCamera(_cinemachineVirtualCameras[cameraIndex]);
        }
        public static void SwitchVirtualCamera(CinemachineVirtualCameraBase virtualCameraBase)
        {
            if(IsActive(virtualCameraBase))return;
            ActiveCamera = virtualCameraBase;
            SetAllCameraInactivePriority();
            virtualCameraBase.Priority = 18;

            cameraIndex = _cinemachineVirtualCameras.IndexOf(ActiveCamera);
        }
        static void SetAllCameraInactivePriority()
        {
            foreach (var cameraBase in _cinemachineVirtualCameras)
            {
                if(cameraBase != ActiveCamera && cameraBase.Priority != 0)
                    cameraBase.Priority = 0;
            }
        }
        static void DisableVirtualCamera(CinemachineVirtualCameraBase virtualCameraBase)
        {
            virtualCameraBase.Priority = 0;
            if(IsActive(virtualCameraBase))
                ActiveCamera = null;
        }
        static void EnableVirtualCamera(CinemachineVirtualCameraBase virtualCameraBase)
        {
            virtualCameraBase.enabled = true;
            virtualCameraBase.Priority = 0;
            if(IsActive(virtualCameraBase))
                ActiveCamera = null;
        }
        public void SetFollowTarget(Transform target)
        {
            foreach (var cam in _cinemachineVirtualCameras)
            {
                cam.Follow = target;
            }
        }
        public void SetLookAtTarget(Transform target)
        {
            foreach (var cam in _cinemachineVirtualCameras)
            {
                cam.LookAt = target;
            }
        }
    }
}
