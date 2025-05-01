using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TornadoScript : MonoBehaviour
{
    float steering = 0;
    
    float counter = 0;
    [SerializeField] float steerStrength;
    [SerializeField] float speed;
    float seed;
    Vector3 velocity = Vector3.forward;
    public Transform arenaCentre;
    [SerializeField] float arenaCentreTether;
    [SerializeField] float maxSteer;
    [SerializeField] float steeringSharpness;
    [SerializeField] Vector3 AOE;
    [SerializeField] float damage;
    
    float hitCD = 0.3f;

    bool started = false;
    

    // Start is called before the first frame update
    void Start()
    {
        seed = Random.Range(0, 1000);
        GetComponent<VisualEffect>().Play();
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
    }

    
    void Update()
    {
        
        if(GetComponent<VisualEffect>().aliveParticleCount <= 0){
            if(started){
                Destroy(gameObject);
            }
        }
        else{
            started = true;
        }

        hitCD -= Time.deltaTime;

        if(hitCD <= 0){
             Collider[] hits = Physics.OverlapBox(transform.position, AOE);
            foreach(Collider hit in hits){
                if(hit.tag == "Player"){
                    hit.GetComponent<PlayerStatus>().TakeDamage(damage);
                    hitCD = 0.5f;
                }
            }
        }


        if(Vector3.Distance(transform.position, arenaCentre.position) > arenaCentreTether)
        {
            Vector3 dirToArenaCenter = arenaCentre.position - transform.position;
            dirToArenaCenter.y = 0;
            dirToArenaCenter.Normalize();
            Vector3 steeringDir = Vector3.ClampMagnitude(dirToArenaCenter - transform.forward, maxSteer) * Time.deltaTime * steeringSharpness;
            transform.forward = (transform.forward + steeringDir).normalized;
            steering = 0;
        }
        else{
            steering +=(Mathf.PerlinNoise(seed * 1000 + 0.5f, Time.time) - 0.5f) * steerStrength * Time.deltaTime;
            steering = Mathf.Clamp(steering, -100, 100);
            //Debug.Log(Mathf.PerlinNoise(seed * 1000 + 0.5f, Time.time));
            
            transform.forward = (transform.forward + transform.right * (steering/100) * Time.deltaTime).normalized;   
        }
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, AOE*2);
    }
}
