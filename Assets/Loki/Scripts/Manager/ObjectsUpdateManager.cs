using System.Collections;
using System.Collections.Generic;
using Grandora.Behaviour;
using UnityEngine;
namespace Grandora.Manager
{
    public static class ObjectsUpdateManager
    {
        static ObjectsUpdateManager()
        {
            objectUpdateAgent = new GameObject("ClientUpdate",typeof(ObjectUpdateAgent)).GetComponent<ObjectUpdateAgent>();
            Object.DontDestroyOnLoad(objectUpdateAgent);
        }
        static ObjectUpdateAgent objectUpdateAgent;
        static HashSet<ObjectBehaviour> objectUpdateables = new HashSet<ObjectBehaviour>();
        public static int componentCount => objectUpdateables.Count;
        public static void Register<T>(T t) where T : ObjectBehaviour
        {
            //Debug.Log("* ObjectBehaviour Register *"+t.GetType());
            if(!objectUpdateables.Contains(t))
                objectUpdateables.Add(t);
        }
        public static void UnRegister<T>(T t) where T : ObjectBehaviour
        {
            //Debug.Log("* ObjectBehaviour UnRegister * "+t.GetType());
            if(objectUpdateables.Contains(t))
                objectUpdateables.Remove(t);
        }
        public class ObjectUpdateAgent : MonoBehaviour
        {
            [SerializeField]int count => objectUpdateables.Count;
            void Update()
            {
                foreach (var objUpdate in objectUpdateables)
                {
                    objUpdate.OnUpdate();
                }
            }
            void FixedUpdate() {
                foreach (var objUpdate in objectUpdateables)
                {
                    objUpdate.OnFixedUpdate();
                }
            }
            void LateUpdate() {
                foreach (var objUpdate in objectUpdateables)
                {
                    objUpdate.OnLateUpdate();
                }
            }
        }
    }
}
