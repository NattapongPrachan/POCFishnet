using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using UniRx;
using Grandora;
using Grandora.Network;

public abstract class ObjectBehaviour : SerializedMonoBehaviour, ILokiListener,IBaseOperation
{
    public bool AutoRegisterUpdate = true;
    private bool _isInit;
    private LokiBehaviour _lokiBehaviour;
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
    protected virtual void Start() {
        if(!IsInit)
            RegisterComponentWhenAssigned();
    }
    #region  IBaseOperation
     public virtual void OnCreate() 
        {
            if (IsInit)
            {
                return;
            }
            LokiBehaviour.ObserveEveryValueChanged(_ => _).Where(_ => _!= null).Subscribe(_ =>
            {
                AddListener();
            }).AddTo(this);
            IsInit = true;
        } 
    public virtual void AddListener()
    {
        LokiBehaviour.OnObjectBehaviourAdded?.Subscribe(_ => OnObjectBehaviourAdded(_));
        LokiBehaviour.OnObjectBehaviourRemoved?.Subscribe(_ => OnObjectBehaviourRemoved(_));
        // LokiBehaviour.OnNetworkObjectBehaviourAdded?.Subscribe(_ => OnNetworkObjectBehaviourAdded(_));
        // LokiBehaviour.OnNetworkObjectBehaviourRemoved?.Subscribe(_ => OnNetworkObjectBehaviourRemoved(_));
    }
    public virtual void OnDestroy()
    {
        OnObjectDestroy();
    }
    public virtual void RemoveListener(){}
    public virtual void OnUpdate()
    {
    }
    public virtual void OnFixedUpdate()
    {
    }
    public virtual void OnLateUpdate()
    {
        
    }
    public virtual void OnObjectDestroy()
    {
        RemoveListener();
        UnregisterComponentWhenRemoved();
    }
    public virtual void OnActive()
    {
    }
    public virtual void OnInActive()
    {
    }
    public virtual void Dispose()
    {
        
    }
    #endregion
    
    #region ILokiListener
        //ibase operation

        public virtual void OnObjectBehaviourAdded(ObjectBehaviour objectBehaviour){}
        public virtual void OnNetworkObjectBehaviourAdded(NetworkObjectBehaviour networkObjectBehaviour){}
        public virtual void OnObjectBehaviourRemoved(ObjectBehaviour objectBehaviour){}
        public virtual void OnNetworkObjectBehaviourRemoved(NetworkObjectBehaviour networkObjectBehaviour){}

        public virtual void RegisterComponentWhenAssigned()
        {
            var findLoki = GetComponentInParent<LokiBehaviour>();
            if (findLoki != null)findLoki.RegisterInRunTime(this);
        }

        public void UnregisterComponentWhenRemoved()
        {
            LokiBehaviour?.UnRegisterInRuntime(this);
        }

    #endregion

}
