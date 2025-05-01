using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownTomato : MonoBehaviour
{
    public Vector3 target;
    public Vector3 startLocation;
    float x = 0;
    float speed = 30;
    float heightFactor = 0.5f;
    float maxHeight = 10;
    Vector3 dif;
    float explosionRadius = 3;
    float explosionDamage = 10;
    // Start is called before the first frame update
    void Start()
    {
        startLocation = transform.position;
        dif = target - startLocation;
        float heighestPoint = (-dif.magnitude/2 * (dif.magnitude/2 - (dif.magnitude)));
        heightFactor = maxHeight/heighestPoint;
    }

    // Update is called once per frame
    void Update()
    {
        if(x > dif.magnitude){
            explode();
        }
        target.y = startLocation.y;
        //startLocation.y = 0;
        float y = (-x * (x - (dif.magnitude))) * heightFactor;
        Vector3 xz = Vector3.ClampMagnitude(dif, x);

        transform.position = new Vector3(xz.x, y, xz.z) + startLocation;
        x += Time.deltaTime * (dif.magnitude);

        
    }

    void explode(){
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(Collider collider in colliders){
            if(collider.tag == "Enemy")
            {
                collider.GetComponent<GeneralEnemy>().TakeDamage(explosionDamage);
            }
        }
    }
}
