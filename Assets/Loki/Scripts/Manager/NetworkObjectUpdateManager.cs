using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using Grandora.Network;
using UnityEngine;
using UniRx;
using System.Linq;
namespace Grandora.Manager
{
public class NetworkObjectUpdateManager : MonoBehaviour
{
    static NetworkObjectUpdateManager()
        {
            networkUpdateAgent = new GameObject("networkUpdateAgent",typeof(NetworkObjectUpdateAgent)).GetComponent<NetworkObjectUpdateAgent>();
            Object.DontDestroyOnLoad(networkUpdateAgent);
            //Debug.Log("networkUpdateAgent "+networkUpdateAgent);
        }
        static NetworkObjectUpdateAgent networkUpdateAgent;
        static HashSet<NetworkObjectBehaviour> objectUpdateables = new HashSet<NetworkObjectBehaviour>();
        public static int componentCount => objectUpdateables.Count;
        public static void Register<T>(T t) where T : NetworkObjectBehaviour
        {
            //Debug.Log("*NetworkObjectBehaviour register--------> "+t.GetType());
            if(!objectUpdateables.Contains(t))
                objectUpdateables.Add(t);
        }
        public static void UnRegister<T>(T t) where T : NetworkObjectBehaviour
        {
            //Debug.Log("*NetworkObjectBehaviour UnRegister--------> "+t.GetType());
            if(objectUpdateables.Contains(t))
                objectUpdateables.Remove(t);
        }
        public class NetworkObjectUpdateAgent : MonoBehaviour
        {
            void Start() 
            {
                
            }
            void Update()
            {
                foreach (var objUpdate in objectUpdateables)
                {
                    objUpdate.OnUpdate();
                }
            }
            void LateUpdate() {
                foreach (var objUpdate in objectUpdateables)
                {
                    objUpdate.OnLateUpdate();
                }
            }

            private void FixedUpdate()
            {
                foreach (var objUpdate in objectUpdateables)
                {
                    objUpdate.OnFixedUpdate();
                }
            }
        }
    }
}
