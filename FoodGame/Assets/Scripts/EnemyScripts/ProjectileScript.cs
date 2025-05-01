using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float speed;
    public Vector3 dir;
    public float lifetime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += dir * speed * Time.deltaTime;
        lifetime -= Time.deltaTime;
        if(lifetime <= 0){
            Destroy(gameObject);
        }
        
    }

    void OnCollisionEnter(Collision collision){
        if(collision.collider.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerStatus>().AddTrauma(0.2f);
            collision.gameObject.GetComponent<PlayerStatus>().TakeDamage(20f);
            Destroy(gameObject);
        }
    }
    
}
