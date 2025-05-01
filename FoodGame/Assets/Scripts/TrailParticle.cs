using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TrailParticle : MonoBehaviour
{
    VisualEffect trail;
    Vector3 savedPos;
    // Start is called before the first frame update
    void Start()
    {
        trail = GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {

        trail.SetVector3("dir", savedPos - transform.position);
        savedPos = transform.position;
    }
}
