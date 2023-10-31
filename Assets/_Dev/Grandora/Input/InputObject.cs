using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Grandora.GameInput
{
    public abstract class InputObject<T> : ObjectBehaviour where T : IInputActionCollection2,IDisposable,new()
    {
        [SerializeField]bool autoAddInput = false;
        public T input;
        protected InputActionAsset inputActionAsset;
        public override void OnCreate()
        {
            base.OnCreate();
            input = new T();
            AddInput();
        }
        private void OnEnable()
        {
            inputActionAsset?.Enable();
        }
        private void OnDisable()
        {
           inputActionAsset?.Disable();
        }
        public abstract void AddInput();
        public abstract void RemoveInput();
    }
}
