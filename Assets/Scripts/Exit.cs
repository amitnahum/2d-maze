// A grenade
// - instantiates an explosion Prefab when hitting a surface
// - then destroys itself

using System;
using UnityEngine;
using System.Collections;

public class Exit : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("the player won");
        Debug.Log(other.gameObject.tag);
    }
}