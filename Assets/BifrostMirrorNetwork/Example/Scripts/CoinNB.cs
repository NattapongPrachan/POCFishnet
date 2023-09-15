using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinNB : NetworkBehaviour
{
    [Server]
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Despawn(this.gameObject);
        }
    }
}
