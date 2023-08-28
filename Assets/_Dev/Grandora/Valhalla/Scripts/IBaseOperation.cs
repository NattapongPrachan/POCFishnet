using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBaseOperation 
{
    void OnCreate();
    void OnUpdate();
    void OnFixedUpdate();
    void OnLateUpdate();
    void OnObjectDestroy();
    void OnActive();
    void OnInActive();
    void Dispose();

}
