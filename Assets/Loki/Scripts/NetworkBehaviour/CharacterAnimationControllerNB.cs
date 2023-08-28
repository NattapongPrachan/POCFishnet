using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Grandora.Behaviour;
using Grandora.GameInput;
using UniRx;
using UnityEngine;
using Grandora.Network;

namespace Grandora.Behaviour
{
    public class CharacterAnimationControllerNB : NetworkObjectBehaviour
    {
        PlayerInputSystem inputSystem;
        public Animator animator{get;private set;}
        IDisposable updateAnimatorEvent;
        int vertical;
        int horizontal;
        bool isInterActing;
        private void Awake() {
            //characterManager = GetComponent<CharacterManager>();
            //animator = GetComponent<Animator>();
            
        }
        public override void OnCreate()
        {
            base.OnCreate();
        }
        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!IsOwner)
            {
                updateAnimatorEvent?.Dispose();
                inputSystem = null;
            }
        }
        //public override void  OnNetworkSpawn()
        //{
        //    if(!IsLocalPlayer)
        //    {
        //        updateAnimatorEvent?.Dispose();
        //        inputSystem = null;
        //    }


        //}
        // public void SetupController(Animator _animator,PlayerInputSystem _inputSystem,CharacterActionStatus _action_status)
        // {
        //     SetupAnimator(_animator);
        //     SetupInput(_inputSystem);
        //     SetupCharacterActionStatus(_action_status);
        // }
        public void SetupAnimator(Animator _animator)
        {
            animator = _animator;
            vertical = Animator.StringToHash("moveY");
            horizontal = Animator.StringToHash("moveX");
            //isInterActing = animator.GetBool(AnimationParamKeys.isInteracting);
        }
        public void SetupInput(PlayerInputSystem _inputSystem)
        {
            inputSystem = _inputSystem;
            AddListenerAnimator();
        }
        // public void SetupCharacterActionStatus(CharacterActionStatus _action_status)
        // {
        //     action_status.Value = _action_status;
        //     AddListener();
        //     action_status.Value.isRelax = true;
        // }
        // void AddListener()
        // {
        //     action_status.Value.ObserveEveryValueChanged(v => v.isSit).Subscribe(_ =>{
        //         animator.SetBool(AnimationParamKeys.isSit,_);
        //     }).AddTo(this);
        //     action_status.Value.ObserveEveryValueChanged(v => v.isJump).Subscribe(_ =>{
        //         animator.SetBool(AnimationParamKeys.isJump,_);
        //     }).AddTo(this);
        //     action_status.Value.ObserveEveryValueChanged(v => v.isAir).Subscribe(_ =>{
        //         animator.SetBool(AnimationParamKeys.isAir,_);
        //     }).AddTo(this);
        //     action_status.Value.ObserveEveryValueChanged(v => v.isMove).Subscribe(_ =>{
        //         animator.SetBool(AnimationParamKeys.isMove,_);
        //     }).AddTo(this);
        //     action_status.Value.ObserveEveryValueChanged(v => v.isArmed).Subscribe(_ =>{
        //         animator.SetBool(AnimationParamKeys.isArm,_);
        //     }).AddTo(this);
        //     action_status.Value.ObserveEveryValueChanged(v => v.is2Hand).Subscribe(_ =>{
        //         animator.SetBool(AnimationParamKeys.is2Hand,_);
        //     }).AddTo(this);
        //     action_status.Value.ObserveEveryValueChanged(v => v.isRelax).Subscribe(_ =>{
        //         animator.SetBool(AnimationParamKeys.isRelax,_);
        //     }).AddTo(this);
        //     action_status.Value.ObserveEveryValueChanged(v => v.jumpCount).Where(j => j>1).Subscribe(_ =>{
        //         animator.SetTrigger(AnimationParamKeys.doubleJump);
        //     }).AddTo(this);
        //     action_status.Value.ObserveEveryValueChanged(v => v.isStunned).Subscribe(_ =>{
                
        //         animator.SetBool(AnimationParamKeys.isStunned,_);
        //         if(_)
        //             animator.SetTrigger(AnimationTriggerKeys.stunned);
        //     }).AddTo(this);
        //     action_status.Value.ObserveEveryValueChanged(v => v.isKnockDown).Subscribe(_ =>{
               
        //         animator.SetBool(AnimationParamKeys.isKnockDown,_);
        //         if(_)
        //              animator.SetTrigger(AnimationTriggerKeys.knockDown);
        //     }).AddTo(this);
        //     action_status.Value.ObserveEveryValueChanged(v => v.handleWeaponType).Subscribe(_ =>
        //     {
        //         animator.SetInteger(AnimationParamKeys.handleRightWeaponType,_);
        //     }).AddTo(this);
        //     action_status.Value.ObserveEveryValueChanged(v => v.useRangeWeapon).Subscribe( useRangeWeapon => animator.SetBool(AnimationParamKeys.useRangeWeapon,useRangeWeapon)).AddTo(this);
        //     action_status.Value.ObserveEveryValueChanged(v => v.isAiming).Subscribe(isAiming => animator.SetBool(AnimationParamKeys.isAiming, isAiming)).AddTo(this);
        //     action_status.Value.ObserveEveryValueChanged(v => v.isBlock).Subscribe(_ =>
        //     {
        //         if(_)
        //         {
        //             PlayTargetAnimation("Shield_defence_start", true, false);
        //         }
        //         animator.SetBool(AnimationParamKeys.isBlock, _);
        //     }).AddTo(this);
           
        // }
        public void PlayAction(bool active)
        {
            // if(!active || animator.GetBool(AnimationParamKeys.isInteracting))return;
            //     if(characterManager.action_status.Value.isRelax)
            //     {
            //         PlayTargetAnimation(AnimationNameKeys.Activate,true);
            //     }else
            //     {
            //         //characterAttacker.HandleAttack(characterManager.characterEquipment.rightWeapon.Value);
            //     }
        }
        public void UpdateAnimatorValues(float verticalMovement,float horizontalMovement)
        {
            #region Vertical
            float v = 0;
            if(verticalMovement > 0 && verticalMovement < 0.55f)
            {
                v = 0.5f;
            }else if(verticalMovement > 0.55f)
            {
                v = 1;
            }else if(verticalMovement <0 && verticalMovement > -0.55f)
            {
                v = -0.5f;
            }else if(verticalMovement < -0.55f)
            {
                v = -1;
            }
            else
            {
                v= 0;
            }
            #endregion

            #region  Horizontal
            float h = 0;
            if(horizontalMovement > 0 && horizontalMovement < 0.55f)
            {
                h = 0.5f;
            }else if(horizontalMovement > 0.55f)
            {
                h = 1;
            }else if(horizontalMovement <0 && horizontalMovement > -0.55f)
            {
                h = -0.5f;
            }else if(horizontalMovement < -0.55f)
            {
                h = -1;
            }
            else
            {
                h= 0;
            }
            #endregion
            animator.SetFloat(vertical,v,0,0);
            animator.SetFloat(horizontal,h,0,0);
        }
        public async void PlayTargetAnimation(string targetAnim,bool isInteracting,bool applyRootmotion = true, float fadeTime = 0.2f)
        {
            ApplyRootMotion(applyRootmotion);
            //animator.SetBool(AnimationParamKeys.isInteracting, isInteracting);
            animator.CrossFade(targetAnim, fadeTime);
            await Task.Delay(TimeSpan.FromSeconds(0.05f));
        }
        public void ApplyRootMotion(bool applyRootMotion)
        {
            animator.applyRootMotion = applyRootMotion;
        }
        public void ApplyRootMotionInt(int applyRootMotion)
        {
            animator.applyRootMotion = applyRootMotion == 1 ? true : false;
        }
        public override void OnUpdate()
        {
            //throw new System.NotImplementedException();
        }
        public override void OnInActive()
        {
            base.OnInActive();
            updateAnimatorEvent?.Dispose();
            updateAnimatorEvent = null;
        }
        // public override void OnActive()
        // {
        //     base.OnActive();
        //     AddListenerAnimator();
        // }

        public void TriggerMouseUp()
        {
           // animator.SetTrigger(AnimationParamKeys.triggerMouseUp);
        }

        void AddListenerAnimator()
        {
            if(inputSystem == null)return;
            updateAnimatorEvent?.Dispose();
            updateAnimatorEvent = Observable.EveryUpdate().Where(i => inputSystem != null).Subscribe(_ =>{
                    UpdateAnimatorValues(inputSystem.movement.Value.y,inputSystem.movement.Value.x);
                }).AddTo(this);
        }
        public override void OnObjectBehaviourAdded(ObjectBehaviour obj)
       {
            if(obj is PlayerInputSystem)
            {
                SetupInput(obj as PlayerInputSystem);
                SetupAnimator(GetComponentInChildren<Animator>());
            }
       }

    }
}
