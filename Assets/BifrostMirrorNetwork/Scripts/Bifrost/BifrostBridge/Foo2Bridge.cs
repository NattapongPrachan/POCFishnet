using System.Collections;
using System.Collections.Generic;
using Grandora.Bifrost;
using UnityEngine;

public class Foo2Bridge : BifrostBridge
{
    public override void OnCreate()
    {
        base.OnCreate();
        var foo1 = Bifrost.Get<Foo1Bridge>();
        Debug.Log("Foo2 Create Get " + foo1);
        //Debug.Log("F002Bridge Create isServer "+this.IsServer);
        //Debug.Log("isnetworked "+this.IsNetworked);
    }
    private void Update()
    {
        Debug.Log("Foo2Bridge Update");
    }
}
