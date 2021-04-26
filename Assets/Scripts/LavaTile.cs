using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaTile : MonoBehaviour
{
    private bool active = true;
    private void Start()
    {
        gameObject.tag = "LavaTile";
        InvokeRepeating(nameof(ToggleLava), 0, Random.Range(0,5));
    }

    public bool IsActive()
    {
        return active;
    }

    private void ToggleLava()
    {
        if (active)
        { 
           DisableLava();
        }
        else
        {
            ActivateLava();
        }
    }

    private void ActivateLava()
    {
        active = true;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void DisableLava()
    {
        active = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }
    
}
