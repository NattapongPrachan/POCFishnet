using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using FishNet.Object;
using UniRx;
using FishNet.Connection;

namespace Grandora.Bifrost
{
    public abstract class BifrostBridge : SerializedMonoBehaviour, IBifrostOperation, IBifrostListener, IBifrostNetwork, IBifrostUpdate,IDisposable
    {
        bool _isInit;
        Bifrost _bifrost;
        public Bifrost Bifrost{
            get{
                return _bifrost;
            }
            set{
                _bifrost = value;
            }
        }
        void Start()
        {
            if(!_isInit)
                RegisterComponentWhenAssigned();
        }
        private void OnDestroy()
        {
            UnRegisterComponentWhenRemoved();
        }
        #region IBifrostInit
        public virtual void OnCreate()
        {
            this.gameObject?.SetActive(true);
            Bifrost.OnBifrostAdded?.Subscribe(_ => { OnBifrostAdded(_); }).AddTo(this);
            Bifrost.OnBifrostRemoved?.Subscribe(_ => { OnBifrostRemoved(_); }).AddTo(this);
            _isInit = true;
        }
        #endregion
        #region IBifrostLlistener
        public virtual void OnBifrostAdded(BifrostBridge bifrostBridge) {}
        public virtual void OnBifrostRemoved(BifrostBridge bifrostBridge){}
        public void RegisterComponentWhenAssigned()
        {
            Debug.Log("RegisterComponentWhenAssigned");
            var findBifrost = GetComponentInParent<Bifrost>();
            if(findBifrost != null) findBifrost.RegisterInRunTime(this);
        }
        public virtual void UnRegisterComponentWhenRemoved() {
            Bifrost?.UnregisterInRuntime(this);
        }
        #endregion
        #region IBifrostUpdate
        public virtual void OnUpdate() { }

        public virtual void OnFixedUpdate() { }

        public virtual void OnLateUpdate() { }
        #endregion
        #region IBifrostNetwork

        public virtual void OnStartServer() { }

        public virtual void OnStopClient() { }

        public virtual void OnStartNetwork() { }

        public virtual void OnStopNetwork() { }

        public virtual void OnStopServer() { }

        public virtual void OnDespawnServer(NetworkConnection connection) { }

        public virtual void OnOwnershipClient(NetworkConnection connection) { }

        public virtual void OnOwnershipServer(NetworkConnection connection) { }

        public virtual void OnSpawnServer(NetworkConnection connection) {}

        public virtual void Reset() {}
        public virtual void OnValidate() {}

        public virtual void Dispose()
        {
            
        }
        #endregion

    }
}

