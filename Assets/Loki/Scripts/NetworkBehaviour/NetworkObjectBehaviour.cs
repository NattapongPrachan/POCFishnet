
using System;
using UnityEngine;
using UniRx;
using FishNet.Object;

namespace Grandora.Network
{
    [System.Serializable]
    public abstract class NetworkObjectBehaviour : NetworkBehaviour,ILokiListener,IBaseOperation
    {
        public bool AutoRegisterUpdate = true;
        private bool _isInit;
        private LokiBehaviour _lokiBehaviour;
        [SerializeField]
        public LokiBehaviour LokiBehaviour
        {
            get => _lokiBehaviour;
            set => _lokiBehaviour = value;
        }
        public bool IsInit
        {
            get => _isInit;
            set => _isInit = value;
        }
        void Awake()
        {
            
        }
        private void Start()
        {
            if (!IsInit)
                RegisterComponentWhenAssigned();
        }
        public virtual void OnCreate()
        {
            if (IsInit)
            {
                return;
            }
            LokiBehaviour.ObserveEveryValueChanged(_ => _ != null).Subscribe(_ =>
            {
                AddListener();

            }).AddTo(this);
            IsInit = true;
        }
        public virtual void AddListener()
        {
            LokiBehaviour.OnObjectBehaviourAdded?.Subscribe(_ => OnObjectBehaviourAdded(_));
            LokiBehaviour.OnObjectBehaviourRemoved?.Subscribe(_ => OnObjectBehaviourRemoved(_));
            LokiBehaviour.OnNetworkObjectBehaviourAdded?.Subscribe(_ => OnNetworkObjectBehaviourAdded(_));
            LokiBehaviour.OnNetworkObjectBehaviourRemoved?.Subscribe(_ => OnNetworkObjectBehaviourRemoved(_));
        }
        public virtual void OnUpdate()
        {
            //Depug.LogColor("NetworkObjectBehaviour " + this.GetType() + "OnUpdate", Color.magenta);
        }
        public virtual void OnFixedUpdate() {}
        public virtual void OnLateUpdate(){}
        public virtual void OnObjectDestroy(){
            UnregisterComponentWhenRemoved();
        }
        public virtual void OnActive(){
            if(AutoRegisterUpdate)
                RegisterObject();
        }
        public virtual void OnInActive(){
            UnregisterObject();
        }
        public virtual void Dispose(){
            if(this == null)return;
            if(this.gameObject != null)
            {
                Destroy(this.gameObject);
            }
            
        }
        private void OnEnable() {
            OnActive();
        }
        private void OnDisable() {
            OnInActive();
        }
        protected void RegisterObject()
        {
           // NetworkObjectUpdateManager.Register(this);
        }
        protected void UnregisterObject()
        {
           // NetworkObjectUpdateManager.UnRegister(this);
        }
        public void RegisterComponentWhenAssigned()
        {
            var findLoki = GetComponentInParent<LokiBehaviour>();
            if (findLoki != null)
            {
                findLoki.RegisterInRunTime(this);
            }
        }
        public void UnregisterComponentWhenRemoved()
        {
            var findLoki = GetComponentInParent<LokiBehaviour>();
            if (findLoki != null)
            {
                findLoki.UnRegisterInRuntime(this);
            }
        }


        //ibase operation

        public virtual void OnObjectBehaviourAdded(ObjectBehaviour objectBehaviour){}
        public virtual void OnNetworkObjectBehaviourAdded(NetworkObjectBehaviour networkObjectBehaviour){}
        public virtual void OnObjectBehaviourRemoved(ObjectBehaviour objectBehaviour){}
        public virtual void OnNetworkObjectBehaviourRemoved(NetworkObjectBehaviour networkObjectBehaviour){}
    }
}
