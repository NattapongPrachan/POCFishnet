using Grandora;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public static class LokiUpdateManager 
{
    static LokiUpdateAgent lokiUpdateAgent;
    //static HashSet<LokiBehaviour> lokiUpdateables = new HashSet<LokiBehaviour>();
    static ReactiveCollection<LokiBehaviour> lokiUpdateAbles = new ReactiveCollection<LokiBehaviour>();
    public static int componentCount => lokiUpdateAbles.Count;
    static LokiUpdateManager()
    {
        lokiUpdateAgent = new GameObject("LokiUpdateAgent", typeof(LokiUpdateAgent)).GetComponent<LokiUpdateAgent>();
        Object.DontDestroyOnLoad(lokiUpdateAgent);
        lokiUpdateAbles.ObserveAdd().Subscribe(instance =>
        {
            
        }).AddTo(lokiUpdateAgent);
        lokiUpdateAbles.ObserveRemove().Subscribe(instance =>
        {
            
        }).AddTo(lokiUpdateAgent);
    }
    public static void Register<T>(T t) where T : LokiBehaviour
    {
        if(!lokiUpdateAbles.Contains(t))
            lokiUpdateAbles.Add(t);
    }
    public static void UnRegister<T>(T t) where T : LokiBehaviour
    {
        if (lokiUpdateAbles.Contains(t))
            lokiUpdateAbles.Remove(t);
    }
    public class LokiUpdateAgent : MonoBehaviour
    {
        [SerializeField] int processCount;
        [SerializeField] int count => lokiUpdateAbles.Count;
        void Update()
        {
            processCount = lokiUpdateAbles.Count;
            foreach (var objUpdate in lokiUpdateAbles)
            {
                objUpdate.OnUpdate();
            }
        }
        void FixedUpdate()
        {
            foreach (var objUpdate in lokiUpdateAbles)
            {
                objUpdate.OnFixedUpdate();
            }
        }
        void LateUpdate()
        {
            foreach (var objUpdate in lokiUpdateAbles)
            {
                objUpdate.OnLateUpdate();
            }
        }
    }
}
