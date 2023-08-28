using Grandora.GameInput;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using YGG;

namespace Grandora.Behaviour
{
    public class CharacterThirdPersonController : ObjectBehaviour
    {
        public bool LockCameraPosition = false;
        private bool lockPitchRotation = false;
        private bool lockYawRotation = false;
        public float TopClamp = 70.0f;
        public float BottomClamp = -30.0f;
        public float CameraAngleOverride = 0.0f;
        [SerializeField]
        ReactiveProperty<PlayerInputSystem> _input;
        [SerializeField]Transform _cameraRoot;
        [Header("Aim & Shoot")]
        [SerializeField] float _angle = 45;
        [SerializeField] float _initVelocity;
        [SerializeField] LineRenderer _lineRenderer;
        [SerializeField]
        [Range(10, 100)]
        int linePoint = 25;
        [SerializeField]
        [Range(0.01f, 0.25f)]
        float timeBetweenPoint = 0.1f;

        [Header("POCGun")]
        [SerializeField] bool _fireAuto;
        [SerializeField] float _fireRatePerMinute;
        [SerializeField] int numSpawn = 1;
        [SerializeField] bool _isBurst;


        [SerializeField] float _step;
        [SerializeField] Transform spawnPoint;
        [SerializeField] Transform arrow;

        IDisposable _aimObservable;
        IDisposable _fireAutoObservable;
        IDisposable _shootObservable;
        IDisposable _cancelFireObservable;
        IDisposable _fireRateObservable;
        [Header("Cinemachine Sensitivity")]
        [SerializeField] float _cinemachineTargetYaw;
        [SerializeField] float _cinemachineTargetPitch;
        [SerializeField] float _threshold = 0.01f;

        public Transform BulletPoint => spawnPoint;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }
        public override void OnCreate()
        {
            base.OnCreate();
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            float angle = _angle * Mathf.Deg2Rad;
        }
        
        private void LateUpdate()
        {
            CameraRotation();
        }
        void CameraRotation()
        {
            if (_input == null || _cameraRoot == null) return;
            if (_input.Value.mouseLook.Value.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
                deltaTimeMultiplier = Time.deltaTime;
                if (!lockYawRotation) _cinemachineTargetYaw += _input.Value.mouseLook.Value.x * GameSetting.Instance._cameraSensitivity.Value * 100 * deltaTimeMultiplier;
                _cinemachineTargetYaw += _input.Value.mouseLook.Value.x * GameSetting.Instance._cameraSensitivity.Value * 100 * deltaTimeMultiplier;
                if (!lockPitchRotation) _cinemachineTargetPitch += _input.Value.mouseLook.Value.y * GameSetting.Instance._cameraSensitivity.Value * 100 * deltaTimeMultiplier;
            }
            _cinemachineTargetYaw = GameUtils.ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = GameUtils.ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
            _cameraRoot.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        }
    }
}
