using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpdateManagement
{
    bool AutoRegisterUpdateProcess { get; set; }
    void RegisterUpdateProcess();
    void UnregisterUpdateProcess();
}
