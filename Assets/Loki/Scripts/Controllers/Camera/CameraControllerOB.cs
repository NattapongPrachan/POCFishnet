using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Grandora.GameInput;
using Grandora.Manager;
using UniRx;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grandora.Behaviour
{
    public enum CameraMode
    {
        FirstPerson,ThirdPerson,Range,ShowFish,AimFish
    }
    [DefaultExecutionOrder(100)]
    public class CameraControllerOB : ObjectBehaviour
    {
        #region Camera & virtaulcamera
        public static Subject<Camera> OnCameraUpdate = new Subject<Camera>();
        Transform _cameraTransform;
        public Transform CameraTransform => _cameraTransform;
       
        public Camera MainCamera{get; private set;}
        //Cinemachine
        [SerializeField]Dictionary<CameraMode, CinemachineVirtualCameraBase> _cameraMapper;
        ReactiveCollection<CinemachineVirtualCameraBase> _cinemachineVirtualCameras = new ReactiveCollection<CinemachineVirtualCameraBase>();
        public ReactiveProperty<CameraMode> CameraMode = new ReactiveProperty<CameraMode>(Behaviour.CameraMode.ThirdPerson);
        public CinemachineVirtualCamera FirstPersonCamera;
        public CinemachineVirtualCamera ThirdPersonCamera;
        #endregion
        #region Camera speed
        float tempXSpeed = 0;
        float tempYSpeed = 0;
        #endregion
        public override void OnCreate()
        {
            base.OnCreate();
            MainCamera = CameraManager.Instance.Camera;
            _cameraTransform = MainCamera.transform;
            GetAllVirtualCameraBase();
            AddEventListener();
        }
        void GetAllVirtualCameraBase()
        {
            _cinemachineVirtualCameras = GetComponentsInChildren<CinemachineVirtualCameraBase>().ToReactiveCollection();
        }
        void AddEventListener()
        {
            // GameSetting.Instance._renderScale.AsObservable().Subscribe(_ =>{
            //     thirdPersonCamera.m_Lens.FarClipPlane = GameSetting.Instance._renderScale.Value*GameSetting.Instance._renderMultiply.Value;
            // }).AddTo(this);
            // GameSetting.Instance._renderMultiply.AsObservable().Subscribe(_ =>{
            //     thirdPersonCamera.m_Lens.FarClipPlane = GameSetting.Instance._renderScale.Value*GameSetting.Instance._renderMultiply.Value;
            // }).AddTo(this);
            // GameSetting.Instance._cameraSensitivity.AsObservable().Subscribe(async cameraSensitivityValue =>
            // {
            //     thirdPersonCamera.m_XAxis.m_MaxSpeed = cameraSensitivityValue * GameSetting.Instance._cameraXAxisMultiply.Value;
            //     thirdPersonCamera.m_YAxis.m_MaxSpeed = cameraSensitivityValue * GameSetting.Instance._cameraYAxisMultiply.Value;
            // }).AddTo(this);
        }
        // private void Start() {
        //     MainCamera = CameraManager.Instance.Camera;
        //     _cameraTransform = MainCamera.transform;
        //     //EnvironmentHolder.Instance.SetupCamera(mainCamera);
        // }
        
        public override void OnUpdate()
        {
            
        }
        public override void Dispose()
        {
            base.Dispose();
        }

        
        public override void OnObjectBehaviourAdded(ObjectBehaviour objectBehaviour)
        {
            if(objectBehaviour is PlayerInputSystem)
            {

            }
        }   
        #region Command
        
        public void SetupCamera(CameraMode cameraMode, Transform lookAtTransform = null, Transform followTransform= null)
        {
            _cameraMapper[cameraMode].Follow = followTransform;
            _cameraMapper[cameraMode].LookAt = lookAtTransform;
        }
        public void SetupObjectTransform(Transform transform)
        {
            ThirdPersonCamera.LookAt = transform;
            ThirdPersonCamera.Follow = transform;
        }
        public void SetUpCharacter(Transform characterTransform)
        {
            ThirdPersonCamera.Follow = characterTransform;
            foreach (var cameraBase in _cameraMapper.Values)
            {
                cameraBase.Follow = characterTransform;
            }
            OnCameraUpdate.OnNext(MainCamera);
        }
        public void SwitchCamera(CameraMode mode)
        {
            foreach (var cameraBase in _cameraMapper)
            {
                cameraBase.Value.Priority = 0;
            }
            _cameraMapper[mode].Priority = 10;
        }
        // public void SetUpCharacter(Transform characterTransform , PlayerInputSystem inputSystem)
        // {
            
        //     _cameraTransform = MainCamera.transform;
        //     SetUpCharacter(characterTransform);
        // }
        public void CloseCamera(bool isActive)
        {
            this.gameObject.SetActive(isActive);
        }
        #endregion
        #region Button
        [Button]
        public void ThirdPerson() => SwitchCamera(Behaviour.CameraMode.ThirdPerson);
        [Button]
        public void Range() => SwitchCamera(Behaviour.CameraMode.Range);

        [Button]
        public void AimFish() => SwitchCamera(Behaviour.CameraMode.AimFish);
        #endregion
    }
}
