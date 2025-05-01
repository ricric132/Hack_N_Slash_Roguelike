using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparencySwitch : MonoBehaviour
{
    bool blocking;
    public Material[] opaque;
    public Material[] transparent;
    float transTime;
    public float timeTillOpaque = 2f;
    bool isTrans = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(transTime >= 0) {
            if(isTrans == false)
            {
                GetComponent<Renderer>().materials = transparent;
                isTrans = true;
            }
        }
        else if(isTrans == true)
        {
            GetComponent<Renderer>().materials = opaque;
            isTrans = false;
        }

        transTime -= Time.deltaTime;
    }

    public void goTrans(){
        transTime = timeTillOpaque;
    }
}
