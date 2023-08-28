using Grandora.Behaviour;
using Grandora.Network;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Grandora
{
    public class LokiBehaviour : SerializedMonoBehaviour,IBaseOperation,IUpdateManagement
    {
        [ShowInInspector]
        protected bool isInit = false;
        public Action OnTestAction;
        //Action
        public Subject<ObjectBehaviour> OnObjectBehaviourAdded;
        public Subject<ObjectBehaviour> OnObjectBehaviourRemoved;

        public Subject<NetworkObjectBehaviour> OnNetworkObjectBehaviourAdded;
        public Subject<NetworkObjectBehaviour> OnNetworkObjectBehaviourRemoved;
        public ReactiveDictionary<string, NetworkObjectBehaviour> _networkBehaviours = new ReactiveDictionary<string, NetworkObjectBehaviour>();
        public ReactiveDictionary<string, ObjectBehaviour> _objectBehaviours = new ReactiveDictionary<string, ObjectBehaviour>();

        protected bool _autoRegisterUpdateProcess = false;
        public bool AutoRegisterUpdateProcess 
        {
            get => _autoRegisterUpdateProcess; 
            set => _autoRegisterUpdateProcess = value;
        }

        public virtual void Awake()
        {
            Initialized();
            GetObjectFormComponent();
            GetNetworkObjectFormComponent();
            RunOnCreateBehaviour();
            NotifyBehaviourAdded();
            OnCreate();
            isInit = true;
        }
        public virtual void Start()
        {
            //RegisterUpdateProcess();
            
        }
        #region Assign Loki in runtine
        public void RegisterInRunTime(ILokiListener iLokiListerner)
        {
            Debug.Log("REgisterInRuntime "+iLokiListerner);
            if (!isInit)
            {
                Debug.LogWarning(iLokiListerner + "At " + this.gameObject.name + " try to register when Loki isn't init");
                return;
            }
            if (iLokiListerner is ObjectBehaviour)
            {
                AddObjectBehaviour(iLokiListerner as ObjectBehaviour);
                StartCreateObjectBehaviour(iLokiListerner as ObjectBehaviour);
            }
            else if (iLokiListerner is NetworkObjectBehaviour)
            {
                AddNetworkObjectBehaviour(iLokiListerner as NetworkObjectBehaviour);
                StartCreateNetworkObjectBehaviour(iLokiListerner as NetworkObjectBehaviour);
            }
        }
        public void UnRegisterInRuntime(ILokiListener lokiListener)
        {
            if (!isInit) return;
            if (lokiListener is ObjectBehaviour)
            {
                RemoveObjectBehaviour(lokiListener as ObjectBehaviour);
            }
            else if (lokiListener is NetworkObjectBehaviour)
            {
                RemoveNetworkObjectBehaviour(lokiListener as NetworkObjectBehaviour);
            }
        }
        #endregion

        private void Initialized()
        {
            if (isInit) return;
            OnObjectBehaviourAdded = new Subject<ObjectBehaviour>();
            OnNetworkObjectBehaviourAdded = new Subject<NetworkObjectBehaviour>();
        }

        private void OnEnable()
        {
            OnActive();
        }

        private void NotifyBehaviourAdded()
        {
            _objectBehaviours.ForEach(_ =>
            {
                OnObjectBehaviourAdded.OnNext(_.Value);
            });
            _networkBehaviours.ForEach(_ =>
            {
                OnNetworkObjectBehaviourAdded.OnNext(_.Value);
            });
            OnTestAction?.Invoke();
        }

        void GetObjectFormComponent()
        {
            GetComponentsInChildren<ObjectBehaviour>().ForEach(_ =>
            {
                AddObjectBehaviour(_);
            });
            
        }
        void GetNetworkObjectFormComponent()
        {
            GetComponentsInChildren<NetworkObjectBehaviour>().ForEach(_ => {
                AddNetworkObjectBehaviour(_);
            });

        }
        void RunOnCreateBehaviour()
        {
            _objectBehaviours.ForEach(_ =>
            {
                StartCreateObjectBehaviour(_.Value);
            });
            _networkBehaviours.ForEach(_ =>
            {
                StartCreateNetworkObjectBehaviour(_.Value);
            });
            
        }
        #region Get
        public T GetNB<T>() where T : NetworkObjectBehaviour
        {
            var valueGet = _networkBehaviours.Values.FirstOrDefault(t => t is T);
            if(valueGet == null)
            {
                Debug.LogWarningFormat("LokiBehaviour NetworkObject " + typeof(T) + " cannot found", typeof(T).Name);
            }
            return (T)valueGet;
        }
        public T GetOB<T>() where T : ObjectBehaviour
        {
            var valueGet = _objectBehaviours.Values.FirstOrDefault(t => t is T);
            if (valueGet == null)
            {
                Debug.LogWarningFormat("LokiBehaviour ObjectBehaviour " + typeof(T) + " cannot found", typeof(T).Name);
            }
            return (T)valueGet;
        }
        #endregion
        #region Operation 
        public void AssignObjectBehaviour(ObjectBehaviour objectBehaviour)
        {
            AddObjectBehaviour(objectBehaviour); 
            StartCreateObjectBehaviour(objectBehaviour);
        }
        void StartCreateObjectBehaviour(IBaseOperation IBaseOperation)
        {
            var obj = IBaseOperation as ObjectBehaviour;
            obj.LokiBehaviour = this;
            obj.enabled = true;
            IBaseOperation.OnCreate();
        }
        void StartCreateNetworkObjectBehaviour(NetworkObjectBehaviour networkObjectBehaviour)
        {
            networkObjectBehaviour.LokiBehaviour = this;
            networkObjectBehaviour.enabled = true;
            networkObjectBehaviour.OnCreate();
        }
        protected void AddObjectBehaviour(ObjectBehaviour objectBehaviour)
        {
            Debug.Log("Loki Manager AddObjectBehaviour "+objectBehaviour.GetType().Name);
            if (!_objectBehaviours.ContainsKey(objectBehaviour.GetInstanceID().ToString()))
            {
                _objectBehaviours.Add(objectBehaviour.GetInstanceID().ToString(), objectBehaviour);
                OnObjectBehaviourAdded?.OnNext(objectBehaviour);
                //StartCreateObjectBehaviour(objectBehaviour);
            }
        }
        protected void AddNetworkObjectBehaviour(NetworkObjectBehaviour networkObjectBehaviour)
        {
            //print("AddNetworkObjectBehaviour " + networkObjectBehaviour);
            if (!_networkBehaviours.ContainsKey(networkObjectBehaviour.GetInstanceID().ToString()))
            {
                _networkBehaviours.Add(networkObjectBehaviour.GetInstanceID().ToString(), networkObjectBehaviour);
                OnNetworkObjectBehaviourAdded?.OnNext(networkObjectBehaviour);
                //StartNetworkObjectBehaviour(networkObjectBehaviour);
            }
        }
        protected void RemoveObjectBehaviour(ObjectBehaviour objectBehaviour)
        {
            if (objectBehaviour != null)
            {
                var stringKey = objectBehaviour.GetInstanceID().ToString();
                if (_objectBehaviours.ContainsKey(stringKey))
                {
                    var objectTarget = _objectBehaviours[stringKey];
                    _objectBehaviours.Remove(stringKey);
                    Destroy(objectTarget);
                    OnObjectBehaviourRemoved?.OnNext(objectBehaviour);
                }
            }
        }
        protected void RemoveNetworkObjectBehaviour(NetworkObjectBehaviour networkObjectBehaviour)
        {
            if(networkObjectBehaviour != null)
            {
                var stringKey = networkObjectBehaviour.GetInstanceID().ToString();
                if (_networkBehaviours.ContainsKey(stringKey))
                {
                    var networkObjectTarget = _networkBehaviours[stringKey];
                    _networkBehaviours.Remove(stringKey);
                    Destroy(networkObjectTarget);
                    OnNetworkObjectBehaviourRemoved?.OnNext(networkObjectBehaviour);
                }
            }
        }
       

        public virtual void OnUpdate()
        {
            _networkBehaviours.Where(_ => _.Value.isActiveAndEnabled).ForEach(_ =>
            {
                    _.Value.OnUpdate();
            });
            _objectBehaviours.Where(_ => _.Value.isActiveAndEnabled).ForEach(_ =>
            {
                    _.Value.OnUpdate();
            });
        }
        public virtual void OnFixedUpdate()
        {
            _networkBehaviours.ForEach(_ =>
            {
                if (_.Value.isActiveAndEnabled)
                    _.Value.OnFixedUpdate();
            });
            _objectBehaviours.ForEach(_ =>
            {
                if (_.Value.isActiveAndEnabled)
                    _.Value.OnFixedUpdate();
            });

        }
        public virtual void OnLateUpdate()
        {
            _networkBehaviours.ForEach(_ =>
            {
                if (_.Value.isActiveAndEnabled)
                    _.Value.OnLateUpdate();
            });
            _objectBehaviours.ForEach(_ =>
            {
                if (_.Value.isActiveAndEnabled)
                    _.Value.OnLateUpdate();
            });
        }
        #endregion

        public virtual void Update()
        {
            OnUpdate();
        }
        public virtual void FixedUpdate()
        {
            OnFixedUpdate();
        }
        public virtual void LateUpdate()
        {
            OnLateUpdate();
        }

        public virtual void OnCreate()
        {
            
        }
        public void OnObjectDestroy()
        {
            
        }

        public void OnActive()
        {
            print("Loki behaviour OnActive");
            /*_networkBehaviours.ForEach(_ =>
            {
                _.Value.OnActive();
            });
            _objectBehaviours.ForEach(_ =>
            {
                _.Value.OnActive();
            });*/
        }
        public void OnInActive()
        {
            _networkBehaviours.ForEach(_ =>
            {
                _.Value.OnInActive();
            });
            _objectBehaviours.ForEach(_ =>
            {
                _.Value.OnInActive();
            });
        }
        public void Dispose()
        {
            _networkBehaviours.ForEach(_ =>
            {
                _.Value.Dispose();
            });
            _objectBehaviours.ForEach(_ =>
            {
                _.Value.Dispose();
            });
        }

        public void RegisterUpdateProcess()
        {
            //LokiUpdateManager.Register(this);
        }

        public void UnregisterUpdateProcess()
        {
            //LokiUpdateManager.UnRegister(this);
        }
    }
}
