using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFloor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 11; i++){ 
            Physics.IgnoreLayerCollision(11, i, true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
