using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CellScript : MonoBehaviour {

    public GameObject wallL;
    public GameObject wallR;
    public GameObject wallU;
    public GameObject wallD;

    public void MakeLava()
    {
     gameObject.AddComponent<LavaTile>();
    }

}