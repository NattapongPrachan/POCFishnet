using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet.Connection;
using FishNet.Object;
using Grandora.Bifrost;
using Mono.CSharp;
using UniRx;
using UnityEngine;
namespace Grandora.Bifrost
{
    public abstract class Bifrost : NetworkBehaviour
    {
    //public Subject<>
        public ReactiveDictionary<string,BifrostBridge> BifrostBridges = new ReactiveDictionary<string, BifrostBridge>();
        public Subject<BifrostBridge> OnBifrostAdded = new Subject<BifrostBridge>();
        public Subject<BifrostBridge> OnBifrostRemoved = new Subject<BifrostBridge>();
        [SerializeField]List<BifrostBridge> _bifrostList = new List<BifrostBridge>();
        bool _isInit = false;
        public void RegisterInRunTime(BifrostBridge bifrostBridge)
        {
            Debug.Log("RegisterInRunTime : "+bifrostBridge);
            if(!_isInit) return;
            AssignBridge(bifrostBridge);
        }
        public void UnregisterInRuntime(BifrostBridge bifrostBridge)
        {
            if (!_isInit) return;
            UnAssignBridge(bifrostBridge);
        }
        private void Start()
        {
        
        }
        #region  Bifrost
        void Initialized()
        {
            GetBifrostFromComponent();
            CreateBridges();
            _isInit = true;
        }
        //single
        public void UnAssignBridge(BifrostBridge bridge)
        {
            RemoveBifrost(bridge);
        }
        public void AssignBridge(BifrostBridge bridge)
        {
            Debug.Log("AssignBridge " + bridge);
            AddBifrost(bridge);
            StartCreateBridge(bridge);
        }
        public void StartCreateBridge(BifrostBridge bridge)
        {
            bridge.Bifrost = this;
            bridge.enabled = true;
            bridge.gameObject.SetActive(true);
            bridge.OnCreate();
            if(_isInit)
                OnBifrostAdded?.OnNext(bridge);
        }
    //multiple
        void GetBifrostFromComponent()
        {
            //_bifrostList = GetComponentsInChildren<BifrostBridge>().ToList();
            for (int i = 0; i < _bifrostList.Count; i++)
            {
                AddBifrost(_bifrostList[i]);
            }
            _bifrostList.Clear();
            _bifrostList = null;
        }
        void CreateBridges()
        {
            foreach (var bridge in BifrostBridges.Values)
            {
                StartCreateBridge(bridge);
            }
        }

        void AddBifrost(BifrostBridge bifrostBridge)
        {
            if(!BifrostBridges.ContainsKey(bifrostBridge.GetInstanceID().ToString()))
            {
                BifrostBridges.Add(bifrostBridge.GetInstanceID().ToString(),bifrostBridge);
                //invoke event
            }
        }
        void Update()
        {
            foreach (var bifrost in BifrostBridges.Values)
            {
                UpdateBifrost(bifrost as IBifrostUpdate);
            }
        }
        private void FixedUpdate()
        {
            foreach (var bifrost in BifrostBridges.Values)
            {
                FixedUpdateBifrost(bifrost as IBifrostUpdate);
            }
        }
        void LateUpdate()
        {
            foreach (var bifrost in BifrostBridges.Values)
            {
                LateUpdateBifrost(bifrost as IBifrostUpdate);
            }
        }
        void UpdateBifrost(IBifrostUpdate bifrostUpdate)
        {
            bifrostUpdate?.OnUpdate();
        }
        void FixedUpdateBifrost(IBifrostUpdate bifrostUpdate)
        {
            bifrostUpdate?.OnFixedUpdate();
        }
        void LateUpdateBifrost(IBifrostUpdate bifrostUpdate)
        {
            bifrostUpdate?.OnFixedUpdate();
        }
        void RemoveBifrost(BifrostBridge bifrostBridge)
        {
            if(bifrostBridge != null)
            {
                var bifrostID = bifrostBridge.GetInstanceID().ToString();
                if(BifrostBridges.ContainsKey(bifrostID))
                {
                    BifrostBridges.Remove(bifrostID);
                    ((IDisposable)bifrostBridge)?.Dispose();
                    OnBifrostRemoved?.OnNext(bifrostBridge);
                }
            }
        }
        public T Get<T>() where T : BifrostBridge
        {
            var valueGet = BifrostBridges.Values.FirstOrDefault(t => t is T);
            if(valueGet == null)
            {
                Debug.LogWarningFormat("Bifrost " + typeof(T) + " cannot found", typeof(T).Name);
            }
            return (T)valueGet;
        }
        void InvokeBifrosts(string methodName)
        {
            foreach (var bridge in BifrostBridges.Values)
            {
                bridge.Invoke(methodName,0);
            } 
        }
        void InvokeBifrostWithArgument(string methodName, NetworkConnection networkConnection)
        {
            foreach (var bridge in BifrostBridges.Values)
            {
                bridge.Invoke(methodName, 0);
            }
        }
        #endregion
        string OnCreateMethod = "OnCreate";
        string OnUpdateMethod = "OnUpdate";
        string OnStartServerMethod = "OnStartServer";
        string OnStartClientMethod = "OnStartClient";
        string OnStopClientMethod = "OnStopClient";
        string OnStartNetworkMethod = "OnStartNetwork";
        string OnStopNetworkMethod = "OnStopNetwork";
        string OnStopServerMethod = "OnStopServer";
        string ResetMethod = "Reset";
        string OnValidateMethod = "OnValidate";
        string OnDespawnServerMethod = "OnDespawnServer";
        string OnOwnershipClientMethod = "OnOwnershipClient";
        string OnOwnershipServerMethod = "OnOwnershipServer";
        string OnSpawnServerMethod = "OnSpawnServer";

        //NetworkBehaviour Implement
        public override void OnStartServer()
        {
            Initialized();
            InvokeBifrosts(OnStartServerMethod);
        }
        public override void OnStartClient()
        {
            base.OnStartClient();
            InvokeBifrosts(OnStartClientMethod);
        }
        public override void OnStopClient()
        {
            base.OnStopClient();
            InvokeBifrosts(OnStopClientMethod);
        }
        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            InvokeBifrosts(OnStartNetworkMethod);
        }
        public override void OnStopNetwork()
        {
            base.OnStopNetwork();
            InvokeBifrosts(OnStopNetworkMethod);
        }
        public override void OnStopServer()
        {
            base.OnStopServer();
            InvokeBifrosts(OnStopServerMethod);
        }
        public override void OnDespawnServer(NetworkConnection connection)
        {
           base.OnDespawnServer(connection);
            InvokeBifrostWithArgument(OnDespawnServerMethod, connection);
        }
        public override void OnOwnershipClient(NetworkConnection prevOwner)
        {
            base.OnOwnershipClient(prevOwner);
            InvokeBifrostWithArgument(OnOwnershipClientMethod, prevOwner);
        }
        public override void OnOwnershipServer(NetworkConnection prevOwner)
        {
            base.OnOwnershipServer(prevOwner);
            InvokeBifrostWithArgument(OnOwnershipServerMethod, prevOwner);
        }
        public override void OnSpawnServer(NetworkConnection connection)
        {
            base.OnSpawnServer(connection);
            InvokeBifrostWithArgument(OnSpawnServerMethod, connection);
        }
        protected override void Reset()
        {
            base.Reset();
            InvokeBifrosts(ResetMethod);
        }
        protected override void OnValidate()
        {
            base.OnValidate();
            InvokeBifrosts(OnValidateMethod);
        }
        //public override void O

        
    }
}
