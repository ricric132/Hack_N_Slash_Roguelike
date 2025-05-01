using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class nomnomDog : MonoBehaviour
{
    public GameObject player;
    public float tetherRange;
    public float checkEnemyInterval;
    List<GameObject> enemiesNearPlayer = new List<GameObject>();
    public float attackCD;
    public float targetSwitchCD;
    float timeSinceAttack;
    float timeSinceTargetSwitch;
    Transform targetPosition;
    public float speed;
    public float wanderSpeed;
    CharacterController controller;
    public float enemyTetherRange;
    public float closeAttackRange;
    public Transform swipeStart;
    public float swipeSize;
    float swipeDamage = 20;
    public GameObject swipeIndicator;
    public bool canMove;
    public float positionSwitchAbilityArea;
    float positionSwitchAbilityDamage = 50;
    public GameObject indicator;
    public float wanderJitter = 100f;
    public float wanderRadius = 1.2f;
    public float wanderDistance = 2f;
    Vector3 wanderTarget;
    private Seeker seeker;
    public Path path;
    public float nextWaypointDistance = 3;
    private int currentWaypoint = 0;
    public bool reachedEndOfPath;
    public float maxSteer;
    public float steeringSharpness;
    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        StartCoroutine("checkEnemies");
        controller = GetComponent<CharacterController>();

        //float theta = Random.value * 2 * Mathf.PI;
        //wanderTarget = new Vector3(wanderRadius * Mathf.Cos(theta), 0f, wanderRadius * Mathf.Sin(theta));

        seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        TickTimers();

        if(targetPosition != null){
            wanderTarget = Vector3.zero;
            float dist = Vector3.Distance(transform.position, targetPosition.position);
            if(dist > enemyTetherRange){
                controller.SimpleMove((targetPosition.position - transform.position).normalized * speed);
            }
            else{
                controller.SimpleMove((targetPosition.position - transform.position).normalized * speed * -1);
            }

            if(timeSinceAttack >= attackCD){
                if(dist < closeAttackRange){
                    BasicSwipeAttack();
                }
                timeSinceAttack = 0;
            }
        }
        else if(enemiesNearPlayer.Count > 0){
            float closestDist = Mathf.Infinity;
            GameObject closestEnemy = null;
            foreach(GameObject enemy in enemiesNearPlayer){
                if(Vector3.Distance(enemy.transform.position, transform.position) < closestDist){
                    closestDist = Vector3.Distance(enemy.transform.position, transform.position);
                    closestEnemy = enemy;
                }
            }

            if(closestEnemy != null){
                targetPosition = closestEnemy.transform;
                //transform.position = targetPosition.position;
                //Destroy(targetPosition.gameObject);
            }
        }
        else{
            Wander();
        }



        /*
        if(timeSinceTargetSwitch >=  targetSwitchCD && enemiesNearPlayer.Count > 0){
            timeSinceTargetSwitch = 0;

            targetPosition = enemiesNearPlayer[Random.Range(0, enemiesNearPlayer.Count)].transform;
  
            
            float closestDist = Mathf.Infinity;
            GameObject closestEnemy = null;
            foreach(GameObject enemy in enemiesNearPlayer){
                if(Vector3.Distance(enemy.transform.position, transform.position) < closestDist){
                    closestDist = Vector3.Distance(enemy.transform.position, transform.position);
                    closestEnemy = enemy;
                }
            }

            if(closestEnemy != null){
                targetPosition = closestEnemy.transform;
                //transform.position = targetPosition.position;
                //Destroy(targetPosition.gameObject);
            }
            
        }
        */

        transform.LookAt(targetPosition);
        canMove = true;
    }

    void TickTimers()
    {
        timeSinceAttack += Time.deltaTime;
        timeSinceTargetSwitch += Time.deltaTime;
    }

    void BasicSwipeAttack()
    {
        Instantiate(swipeIndicator, swipeStart.position, Quaternion.identity);
        Collider[] colliders =  Physics.OverlapSphere(swipeStart.position, swipeSize);
        foreach(Collider collider in colliders){
            if(collider.tag == "Enemy")
            {
                collider.GetComponent<GeneralEnemy>().TakeDamage(swipeDamage);
            }
        }
    }
   

    IEnumerator checkEnemies(){
        while (true)
        {
            Collider[] inrange = Physics.OverlapSphere(player.transform.position, tetherRange);

            enemiesNearPlayer.Clear();

            foreach(Collider collider in inrange){
                if(collider.gameObject.tag == "Enemy"){
                    enemiesNearPlayer.Add(collider.gameObject);
                }
            }

            yield return new WaitForSeconds(checkEnemyInterval);
        }
    }

    public void knifeHit(Transform target){
        //targetPosition = target;
    }

    public void positionSwitch(){
        Instantiate(indicator, transform.position, Quaternion.Euler(90, 0, 0)); //Replace with animation
        Collider[] colliders = Physics.OverlapSphere(transform.position, positionSwitchAbilityArea);
        foreach(Collider collider in colliders){
            if(collider.tag == "Enemy")
            {
                collider.GetComponent<GeneralEnemy>().TakeDamage(positionSwitchAbilityDamage);
            }
        }
    }

    public void OnPathComplete (Path p) {
        Debug.Log("A path was calculated. Did it fail with an error? " + p.error);

        if (!p.error) {
            path = p;
            // Reset the waypoint counter so that we start to move towards the first point in the path
            currentWaypoint = 0;
        }
    }

    public void Wander()
    {
        Vector3 endCheck = Vector3.zero;
        if(path != null){
            endCheck = path.vectorPath[path.vectorPath.Count-1];
            endCheck.y = transform.position.y;
        }
        
        if(Vector3.Distance(transform.position, endCheck) < 3f|| wanderTarget == Vector3.zero){
            path = null;
            wanderTarget =  player.transform.position + Vector3.ClampMagnitude(new Vector3((Random.value - 0.5f), 0f, (Random.value - 0.5f)) * 1.5f, 1) * tetherRange;
            wanderTarget.y = transform.position.y;
            seeker.StartPath(transform.position, wanderTarget, OnPathComplete);
        }

        if (path == null) {
            return;
        }  

        reachedEndOfPath = false;
        float distanceToWaypoint;
        while (true) {
            distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance) {
                if (currentWaypoint + 1 < path.vectorPath.Count) {
                    currentWaypoint++;
                } else {
                    reachedEndOfPath = true;
                    break;
                }
            } 
            else 
            {
                break;
            }
        }

        var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint/nextWaypointDistance) : 1f;

        if(Vector3.Distance(transform.position, player.transform.position) > tetherRange){
            speedFactor *= 2;
            Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            Vector3 velocity = dir * speed * speedFactor;
            transform.LookAt(transform.position + velocity);
            controller.SimpleMove(velocity);
        }
        else{
            Vector3 velocity = Seek(path.vectorPath[currentWaypoint]) * wanderSpeed * speedFactor;
            velocity.y = 0;  
            transform.LookAt(transform.position + velocity);
            controller.SimpleMove(velocity);
        }


        /*
        float _speed = wanderSpeed;
        if(Vector3.Distance(transform.position, player.transform.position) > tetherRange){
            wanderTarget = player.transform.position - transform.position;
            _speed = 10;
        }
        float jitter = wanderJitter * Time.deltaTime;

        wanderTarget += new Vector3(Random.Range(-1f, 1f) * jitter, 0f, Random.Range(-1f, 1f) * jitter);
        
        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        Vector3 targetPosition = transform.position + transform.right * wanderDistance + wanderTarget;

        Debug.DrawLine(transform.position, targetPosition);
        Vector3 velocity = (targetPosition - transform.position).normalized; 
        velocity *= _speed;
        

        return velocity;
        */
    }
    Vector3 Seek(Vector3 Target)
    {
        Vector3 desiredVelocity = (Target - transform.position).normalized;
        Vector3 steering = Vector3.ClampMagnitude(desiredVelocity - velocity, maxSteer) * Time.deltaTime * steeringSharpness;
        Vector3 _velocity = (velocity + steering).normalized;
        return _velocity;
    }



    void OnDrawGizmos(){
        Gizmos.DrawWireSphere(player.transform.position, tetherRange);
        Gizmos.DrawWireSphere(swipeStart.position, swipeSize);
    }
}
