using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyScript : GeneralEnemy
{

    public float fireRate = 2;
    public GameObject player;
    public float projectileSpeed;
    public GameObject projectilePrefab;
    public float attackRange;
    public float kiteRange;
    public float moveSpeed;
    public int burstAmount;
    public float burstSpeed;
    int numfired = 0;
    float timeTillNextShot = 0;
    public float kiteCD;
    float timeTillKite;
    public float kiteDur;
    float kiteDurLeft;
    public float kiteSpeed;
    Vector3 kiteDir;
    bool canMove = true;
    public Animator anim;
    public Transform shootPoint;
    
    float stunDur;
    float knockbackDur;
    Vector3 knockbackDir;
    public ResourcesSO peasSO;

    


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        player = GameObject.Find("Player");

        
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        stunDur -= Time.deltaTime;
        knockbackDur -= Time.deltaTime;

        if (stunDur > 0){
            canMove = false;
            if(knockbackDur > 0){
                GetComponent<CharacterController>().SimpleMove(knockbackDir * 20f);
            }
        }
        else{
            canMove = true;
        }

        if(canMove){
            if(kiteDurLeft >= 0){
                kiteDurLeft -= Time.deltaTime;
                GetComponent<CharacterController>().SimpleMove(kiteDir * kiteSpeed);
                
                return;
            }
            
            float distToPlayer = (player.transform.position - transform.position).magnitude;

            Vector3 dirToPlayer = (player.transform.position - transform.position);
            dirToPlayer.y = 0;
            dirToPlayer.Normalize();
            if(distToPlayer < attackRange){
                if(timeTillNextShot <= 0){
                    StartCoroutine("BurstFire");
                    timeTillNextShot = 1/fireRate;
                    
                    /*
                        numfired++;
                    if(numfired < burstAmount){
                        timeTillNextShot = 1/burstSpeed;
                    }
                    else{
                        timeTillNextShot = 1/fireRate;
                        numfired -= burstAmount;
                    }
                    */
                }
            }
            else{
                GetComponent<CharacterController>().SimpleMove(dirToPlayer * moveSpeed);
                transform.LookAt(player.transform, Vector3.up);
                anim.SetBool("Walking", true);
            }
            
            if(distToPlayer < kiteRange){
                GetComponent<CharacterController>().SimpleMove(-dirToPlayer * moveSpeed);
                transform.LookAt(player.transform, Vector3.up);
                anim.SetBool("Walking", true);
                if(timeTillKite <= 0){
                    StartCoroutine("FanFire");
                    timeTillKite = kiteCD;
                } 
            }

            timeTillNextShot -= Time.deltaTime;
            timeTillKite -= Time.deltaTime;
            
        }

        GetComponent<CharacterController>().SimpleMove(new Vector3(0, -10, 0));

        
    }

    IEnumerator BurstFire(){
        anim.SetTrigger("Shoot");
        anim.SetBool("Walking", false);
        canMove = false;
        Vector3 predictedAim = AimPredictPlayerMovement();
        transform.forward = predictedAim.normalized;
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < burstAmount; i++){
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            var randomErrorAngle = Random.Range(-3, 3);
            var adjustedAim = Quaternion.AngleAxis(randomErrorAngle, Vector3.up) * predictedAim;
            projectile.GetComponent<ProjectileScript>().speed = projectileSpeed;
            projectile.GetComponent<ProjectileScript>().dir = adjustedAim;
            yield return new WaitForSeconds(1/burstSpeed);
        }
        canMove = true;
    }

    Vector3 AimPredictPlayerMovement(){
        float t1 = Mathf.Infinity;
        float t2 = Mathf.Infinity;
        Vector3 predictedLocation = new Vector3();
        Vector3 playerVelocity = player.GetComponent<PlayerController>().averageVelocity/Mathf.Max(player.GetComponent<PlayerController>().numOfVelocities, 1);
        Vector3 playerPosition = player.transform.position;

        float a = Vector3.Dot(playerVelocity, playerVelocity) - projectileSpeed * projectileSpeed;
        float b = (2 * Vector3.Dot(playerPosition - transform.position, playerVelocity));
        float c = Vector3.Dot(player.transform.position - transform.position, player.transform.position - transform.position);

        if(a == 0){
            t1 = - c/b;
        }
        else{
            t1 = (-b + Mathf.Sqrt(b * b - 4 * a * c))/(2*a);
            t2 = (-b - Mathf.Sqrt(b * b - 4 * a * c))/(2*a);
        }

        if(t1 > 0){
            if(t1 < t2 || t2 < 0){
                predictedLocation = playerPosition + playerVelocity * t1;
            }
            else if (t2 > 0){
                predictedLocation = playerPosition + playerVelocity * t2;
            }
        }
        else if(t2 > 0){
            predictedLocation = playerPosition + playerVelocity * t2;
        }
        
        //Debug.Log(predictedLocation);

        if(predictedLocation != null)
        {
            predictedLocation.y = shootPoint.position.y; 
            Vector3 firingDir = (predictedLocation - shootPoint.position).normalized;   
            return firingDir;     
        }
        else{
            return Vector3.zero;
        }


    }

    IEnumerator FanFire(){
        anim.SetTrigger("SideShot");
        anim.SetBool("Walking", false);
        kiteDir = (transform.position - player.transform.position);
        kiteDir.y = 0;
        kiteDir.Normalize();
        canMove = false;        
        Vector3 firingDir = (player.transform.position - transform.position).normalized;  
        int width = 30;
        var adjustedAim = Quaternion.AngleAxis(width, Vector3.up) * firingDir;
        adjustedAim = Quaternion.AngleAxis(-width, Vector3.up) * firingDir;
        transform.forward = adjustedAim.normalized;

        yield return new WaitForSeconds(1.5f);
        canMove = true;

        kiteDurLeft = kiteDur;


        GameObject projectile1 = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        GameObject projectile2 = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        GameObject projectile3 = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

        projectile1.GetComponent<ProjectileScript>().speed = projectileSpeed;
        projectile1.GetComponent<ProjectileScript>().dir = firingDir;


        projectile2.GetComponent<ProjectileScript>().speed = projectileSpeed;
        projectile2.GetComponent<ProjectileScript>().dir = adjustedAim;
        

        projectile3.GetComponent<ProjectileScript>().speed = projectileSpeed;
        projectile3.GetComponent<ProjectileScript>().dir = adjustedAim;

    }

    //void JumpRetreat(){

    //}

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        stunDur = 0.5f;
        knockbackDur = 0.1f;
        knockbackDir =  transform.position - player.transform.position;
        knockbackDir.y = 0;
        knockbackDir.Normalize();
        StopCoroutine("BurstFire");
        StopCoroutine("FanFire");
    }

    public override void Die(){
        player.GetComponent<PlayerStatus>().GainResource(peasSO, 1);
        base.Die();

    }
}
