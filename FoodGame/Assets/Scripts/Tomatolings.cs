using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tomatolings : MonoBehaviour
{
    public Transform target = null;
    [SerializeField] float speed;
    [SerializeField] float slowingDistance;
    [SerializeField] float maxSteer;
    [SerializeField] float steeringSharpness;
    [SerializeField] Transform playerFollowPoint;
    [SerializeField] List<Transform> otherTomatolings;
    [SerializeField] float separationStrength;
    float maxJumpHeight = 10;
    float jumpTime = 1f;
    [SerializeField] Animator anim;
    [SerializeField] GameObject visuals;
    Vector3 velocity;
    
    public bool dead = true;
    public float respawnTimer = 0;
    bool jumping = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(dead){
            respawnTimer += Time.deltaTime;
            return;
        }

        if(jumping){
            return;
        }

        if(target != null){
            Vector3 LookAim = target.position - transform.position;
            LookAim.y = 0;
            transform.forward = LookAim.normalized;
            velocity = transform.forward * speed;
            GetComponent<CharacterController>().SimpleMove(velocity);

            if((target.position - transform.position).magnitude <= 5f){
                StartCoroutine(Jump());
            }
        }
        else{
            Vector3 target_offset = playerFollowPoint.position - transform.position;
            float distance = target_offset.magnitude;
            float ramped_speed = speed * (distance / slowingDistance);
            float clipped_speed = Mathf.Min(ramped_speed, speed);
            Vector3 desired_velocity = (clipped_speed / distance) * target_offset;
            Vector3 steering = desired_velocity - velocity;
            steering += ApplySeparation();
            steering.y = 0;
            velocity += steering;
            transform.forward = velocity.normalized;
            GetComponent<CharacterController>().SimpleMove(velocity);
        }   

        if(velocity.magnitude > 1f){
            anim.CrossFade("Run", 0.1f);
        }
        
        
    }

    Vector3 ApplySeparation(){
        Vector3 steering = Vector3.zero;
        foreach(Transform tomatoling in otherTomatolings){
            if(!tomatoling.gameObject.GetComponent<Tomatolings>().dead){
                Vector3 toOther = transform.position - tomatoling.position;
                steering += toOther.normalized * 1/toOther.magnitude;
            }
        }

        return steering * separationStrength;
    }

    IEnumerator Jump(){
        jumping = true;
        Vector3 startLocation = transform.position;
        Vector3 curTarget = target.position;
        Vector3 dif = curTarget - startLocation;
        float heighestPoint = -dif.magnitude/2 * (dif.magnitude/2 - (dif.magnitude));
        float heightFactor = maxJumpHeight/heighestPoint;        
        float time = 0;

        while (time <= jumpTime){
            float x = time/jumpTime * dif.magnitude;
            curTarget.y = startLocation.y;
            //startLocation.y = 0;
            float y = (-x * (x - (dif.magnitude))) * heightFactor;
            Vector3 xz = Vector3.ClampMagnitude(dif, x);

            transform.position = new Vector3(xz.x, y, xz.z) + startLocation;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        jumping = false;
        Explode();
    }

    void Explode(){
        visuals.SetActive(false);
        //playVFX
        
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, 8f);
        List<GameObject> hits = new List<GameObject>();
        foreach(Collider collider in colliders){
            if(!hits.Contains(collider.gameObject)){
                if(collider.tag == "Enemy"){
                    hits.Add(collider.gameObject);
                    collider.GetComponent<GeneralEnemy>().TakeDamage(3);
                }
                if(collider.tag == "Bird"){
                    hits.Add(collider.gameObject);
                    collider.GetComponent<BirbScript>().TakeDamage(3);
                }
            }
        }
        dead = true;
    }

    public void Respawn(){
        if(!dead){
            return;
        }
        transform.position = playerFollowPoint.position;
        visuals.SetActive(true);
        dead = false;
        respawnTimer = 0f;
    }

    private void OnCollisionEnter(Collision other) {
        if(!jumping){
            return;
        }

        Explode();
    }
}
