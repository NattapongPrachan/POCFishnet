using System.Collections;
using System.Collections.Generic;
using Grandora.Behaviour;
using Grandora.GameInput;
using Grandora.Manager;
using Grandora.Network;
using UnityEngine;

public class CharacterLocalNB : NetworkObjectBehaviour
{
    [SerializeField]PlayerInputSystem playerInput;
    //public override void OnNetworkSpawn() // netcode
    //{
    //    base.OnNetworkSpawn();
    //    if(!IsOwner)
    //    {
    //        playerInput.enabled = false;
    //    }else
    //    {
    //        CameraManager.Instance.SetupCamera(Camera.main);
    //        var camera = Instantiate(Resources.Load<GameObject>("CameraObjectBehaviour")).GetComponent<CameraControllerOB>();
    //        camera.SetupCamera(CameraMode.ThirdPerson,LokiBehaviour.GetOB<CharacterLocalOB>().CameraRoot,LokiBehaviour.GetOB<CharacterLocalOB>().CameraRoot);
    //        LokiBehaviour.AssignObjectBehaviour(camera);
    //    }
    //}
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner)
        {
            playerInput.enabled = false;
        }
        else
        {
            CameraManager.Instance.SetupCamera(Camera.main);
            var camera = Instantiate(Resources.Load<GameObject>("CameraObjectBehaviour")).GetComponent<CameraControllerOB>();
            camera.SetupCamera(CameraMode.ThirdPerson, LokiBehaviour.GetOB<CharacterLocalOB>().CameraRoot, LokiBehaviour.GetOB<CharacterLocalOB>().CameraRoot);
            LokiBehaviour.AssignObjectBehaviour(camera);
            if (Application.platform == RuntimePlatform.Android)
            {
                Instantiate(playerInput.MobileJoyStick);
            }
        }
    }
    public override void OnNetworkObjectBehaviourAdded(NetworkObjectBehaviour networkObjectBehaviour)
    {
    }

    public override void OnNetworkObjectBehaviourRemoved(NetworkObjectBehaviour networkObjectBehaviour)
    {
    }

    public override void OnObjectBehaviourAdded(ObjectBehaviour objectBehaviour)
    {
        switch(objectBehaviour)
        {
            case PlayerInputSystem playerInputSystem:
            playerInput = playerInputSystem;
            break;
        }
    }

    public override void OnObjectBehaviourRemoved(ObjectBehaviour objectBehaviour)
    {
    }
}
