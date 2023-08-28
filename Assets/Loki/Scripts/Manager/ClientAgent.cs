using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Assertions;

namespace Grandora.Behaviour
{
    public static class ClientAgent
    {
        static ClientAgent()
        {
            clientAgentUpdate = new GameObject("ClientUpdate",typeof(ClientAgentUpdate)).GetComponent<ClientAgentUpdate>();
            Debug.Log("clientAgentUpdate "+clientAgentUpdate);
        }
        static ClientAgentUpdate clientAgentUpdate;
        //static HashSet<ObjectBehaviour> objectUpdateables = new HashSet<ObjectBehaviour>();
        static Dictionary<System.Type,ObjectBehaviour> objectBehaviours = new Dictionary<System.Type, ObjectBehaviour>();
        static Dictionary<System.Type,ObjectBehaviour>.ValueCollection ObjectBehaviourValues => objectBehaviours.Values;
        public static void Register<T>(System.Type type,T obj) where T : ObjectBehaviour
        {
            Debug.Assert(!objectBehaviours.ContainsKey(type));
            Debug.Log("Add "+type + " value "+obj);
            objectBehaviours.Add(type,obj);
        }
        public static void Unregister(System.Type type)
        {
            //Debug.Assert(objectBehaviours.ContainsKey(typeof(T).Name));
            //Debug.Assert(objectBehaviours.ContainsKey(typeof(T).Name));
            Debug.Assert(objectBehaviours.ContainsKey(type));
            objectBehaviours.Remove(type);
        }
        public class ClientAgentUpdate : MonoBehaviour
        {
            void Update()
            {
                foreach (var objUpdate in ObjectBehaviourValues)
                {
                    objUpdate.OnUpdate();
                }
            }
            void LateUpdate() {
                foreach (var objUpdate in ObjectBehaviourValues)
                {
                    objUpdate.OnLateUpdate();
                }
            }
        }
    }
}
