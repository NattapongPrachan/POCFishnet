// using System.Collections;
// using System.Collections.Generic;
// using Grandora.Behaviour;
// using Grandora.Mirror;
// using Mirror;
// using UnityEngine;
// using UnityEngine.InputSystem;

// namespace Grandora.GameInput
// {
//     public abstract class NetworkInputObject<T> : NetworkObjectBehaviour where T : IInputActionCollection2, new()
//     {
//         [SerializeField]bool autoAddInput = false;
//         protected T input;
//         private void Awake() {
//             OnCreate();
//         }
//         public override void OnCreate()
//         {
//             input = new T();
//             EnableInput(autoAddInput);
//         }
//         public override void OnActive()
//         {
//             base.OnActive();
//             EnableInput(true);
//         }
//         public override void OnInactive()
//         {
//             base.OnInactive();
//             EnableInput(false);
//         }
//         public abstract void AddInput();
//         public virtual void RemoveInput(){}
//         public virtual void EnableInput(bool active)
//         {
//             print("Enableinput "+active);
//             if(active)
//             {
//                 input?.Enable();
//                 AddInput();
//             }
//             else    
//             {
//                 input?.Disable();
//                 RemoveInput();
//             }
//         }   
//     }
// }
