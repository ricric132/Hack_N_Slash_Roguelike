using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingKnife : MonoBehaviour
{
    public Vector3 hitBox;
    public float damage;
    float speed = 50;
    public GameObject dog;
    public PlayerAbilities playerAbilities;
    bool stuck;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!stuck)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        

            Collider[] inHitBox = Physics.OverlapBox(transform.position, hitBox);
            foreach(Collider collider in inHitBox)
            {
                if(collider.tag == "Enemy"){
                    playerAbilities.gameObject.GetComponent<CharacterController>().enabled = false;
                    playerAbilities.gameObject.transform.position = collider.transform.position - (collider.transform.position - playerAbilities.gameObject.transform.position).normalized * 1f;
                    playerAbilities.gameObject.GetComponent<CharacterController>().enabled = true;
                    collider.gameObject.GetComponent<GeneralEnemy>().TakeDamage(damage);
                    dog.GetComponent<nomnomDog>().knifeHit(collider.transform);
                    playerAbilities.knifeTarget = collider.transform;
                    stuck = true;

                    //transform.parent = collider.transform;
                    Destroy(gameObject);
                    break;
                }
            }
        }
    }

    void OnDrawGizmos(){
        Gizmos.DrawWireCube(transform.position, hitBox);
    }
}
