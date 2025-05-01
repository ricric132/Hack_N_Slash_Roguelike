using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSelfTermination : MonoBehaviour
{
    private float TimeALive = 0f;
    private float MaximumTime = 2f;

    // Update is called once per frame
    void Update()
    {
        TimeALive += Time.deltaTime;
        if(TimeALive >= MaximumTime)
        {
            Destroy(gameObject);
        }
    }
}
