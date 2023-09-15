using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Grandora.Bifrost;
using UnityEngine;
namespace Grandora.Bifrost
{
    public class Foo1Bridge : BifrostBridge
    {

        //public override void OnBifrostAdded(BifrostBridge bifrostBridge)
        //{
        //    base.OnBifrostAdded (bifrostBridge);
        //    Debug.Log("OnBifrostAdd " + bifrostBridge);
        //}
        public override void OnCreate()
        {
            base.OnCreate();
            Debug.Log("Bifrost ? " + Bifrost);
        }
        public override void OnBifrostAdded(BifrostBridge bifrostBridge)
        {
            base.OnBifrostAdded(bifrostBridge);
            Debug.Log("OnBifrostAdd " + bifrostBridge);
        }

        public override void OnBifrostRemoved(BifrostBridge bifrostBridge)
        {
            base.OnBifrostRemoved (bifrostBridge);
            Debug.Log("OnBifrostRemove " + bifrostBridge);
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            Debug.Log("Foo1bridge onupdate");
        }
    }
}
