using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UniRx;
using System;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem.Interactions;

namespace Grandora.GameInput
{
    public class PlayerInputSystem : InputObject<PlayerInputAction>
    {
        [SerializeField] GameObject _mobileJoyStick;
        public GameObject MobileJoyStick => _mobileJoyStick;
        public ReactiveProperty<Vector2> movement = new ReactiveProperty<Vector2>(Vector2.zero);
        public ReactiveProperty<Vector2> mouseLook = new ReactiveProperty<Vector2>(Vector2.zero);
        public override void OnCreate()
        {
            base.OnCreate();
           
        }
        public override void AddInput()
        {
            inputActionAsset = input.asset; // จำเป็นต้องมี
            input.Player.Movement.performed += OnMove;
            input.Player.Movement.canceled += OnMove;
            if (Application.platform == RuntimePlatform.Android)
            {
                input.PlayerGamePad.Look.performed += OnMouseLook;
                input.PlayerGamePad.Look.canceled += OnMouseLook;
            }
            else
            {
                input.Player.MouseLook.performed += OnMouseLook;
                input.Player.MouseLook.canceled += OnMouseLook;
            }
            
            input.PlayerGamePad.Movement.performed += OnMove;
            input.PlayerGamePad.Movement.canceled += OnMove;

            

        }

        private void OnMouseLook(InputAction.CallbackContext context)
        {
            mouseLook.Value = context.ReadValue<Vector2>();
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            movement.Value = context.ReadValue<Vector2>();
        }
         public override void RemoveInput()
        {
            //throw new NotImplementedException();
        }
    }
}
