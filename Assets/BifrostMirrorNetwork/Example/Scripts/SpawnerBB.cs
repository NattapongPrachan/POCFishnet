using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using FishNet.Managing;
using System.Linq;
using FishNet;

namespace Grandora.Bifrost
{
    public class SpawnerBB : BifrostBridge
    {
        [SerializeField] float timeSpawn = 1f;
        [SerializeField] GameObject objectSpawn;
        [SerializeField] float _radius;
        public override void OnCreate()
        {
            base.OnCreate();
            Debug.Log("SpawnerBB OnCreate");
            Observable.Interval(TimeSpan.FromSeconds(timeSpawn)).Subscribe(_=>Spawn()).AddTo(this);
        }
        void Spawn()
        {
            Debug.Log("Spawn");
            var go = Instantiate(objectSpawn);
            var randomPosition = UnityEngine.Random.insideUnitCircle * _radius;
            go.transform.position = new Vector3(randomPosition.x, 10, randomPosition.y);
            InstanceFinder.ServerManager.Spawn(go, null);

        }
    }
}
