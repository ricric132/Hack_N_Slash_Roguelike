using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MiniBossCombo
{
    Idle,
    Attack1,
    Attack2,
    Summoning,
    Attack3,
    // add more combo states as needed
}
public class MiniBossController : MonoBehaviour
{
    public GameObject pukePrefab;
    public GameObject MiniSpawnPrefab;
    public float pukeCooldown = 4f;
    public float summonCooldown = 2f;
    public float DetectionRange = 20f;
    public float pukeDamagePerSecond = 0.5f;
    public float pukeTime = 2f;
    public float SpinDuration = 4f;
    public float SpinCooldown = 4f;
    public int MiniSpawnRate = 2;
    public float summonRadius = 3f;
    public Animator animator;

    ////////////////////////////////////////////

    public float HitDamage = 10f;
    public float HitRange = 1f;
    public int HitAnimationIndex = 1;

    public float PukingDamage = 10f;
    public float PukingRange = 10f;
    public int PukingAnimationIndex = 2;

    public float SummoningDamage = 0f;
    public float SummoningRange = 1f;
    public int SummoningAnimationIndex = 3;

    public float SpinningDamage = 10f;
    public float SpinningRange = 5f;

    public int SpinningAnimationIndex = 4;

    ////////////////////////////////////////////
    public bool isMoving;
    public bool isAttacking;
    public float ComfortZone = 5f;
    private float timer;
    private float Lastingtimer;
    private float currentAnimationTime = 2f;

    public Transform hitPosition;
    public MiniBossCombo currentState;

    public bool Attacked;

    void Start()
    {
        //animator = GetComponent<Animator>();
        isMoving = false;
        isAttacking = false;
        Attacked = false;
    }

    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        
        if (distanceToPlayer < DetectionRange)
        {
            // Face the player
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            if(isAttacking)
            {
                isMoving = false;
            }
            else
            { 
                isMoving = true;
            }
            if(distanceToPlayer > ComfortZone)
            {
                if(isMoving)
                {
                    transform.position += directionToPlayer * 5 * Time.deltaTime;
                    animator.SetBool("IsWalking", true);
                    animator.SetBool("IsAttacking", false);
                }
                else
                {
                    animator.SetBool("IsAttacking", true);
                    animator.SetBool("IsWalking", false);
                }
            }
            else if(!Attacked)
            {
                isAttacking = true; 
                timer += Time.deltaTime;
                if(timer >= currentAnimationTime)
                {
                    timer = 0;
                    Attacked = true;
                    MinibossATKcombo();
                }
            }
            if(Attacked && distanceToPlayer > ComfortZone)
            {
                isAttacking = true; 
                Lastingtimer += Time.deltaTime;
                if(Lastingtimer >= 2f)
                {
                    Attacked = false;
                }
            }
            Debug.Log(timer);

        }
        else
        {
            currentState = MiniBossCombo.Idle;
            isMoving = false;
            isAttacking = false;
            // Player is out of range, stop attacking
        }
    }

    public void MinibossATKcombo()
    {
        switch (currentState)
        {
            case MiniBossCombo.Idle:
                currentState = MiniBossCombo.Attack1;
                animator.SetTrigger("Melee");
                ExecuteAttack(HitDamage, HitRange, HitAnimationIndex);
                break;
            case MiniBossCombo.Attack1:
                currentState = MiniBossCombo.Attack2;
                animator.SetTrigger("Puke");
                ExecuteAttack(PukingDamage, PukingRange, PukingAnimationIndex);
                break;
            case MiniBossCombo.Attack2:
                currentState = MiniBossCombo.Summoning;
                animator.SetTrigger("Summon");
                ExecuteAttack(SummoningDamage, SummoningRange, SummoningAnimationIndex);
                break;
            case MiniBossCombo.Summoning:
                currentState = MiniBossCombo.Attack3;
                animator.SetTrigger("Spin");
                ExecuteAttack(SpinningDamage, SpinningRange, SpinningAnimationIndex);
                break;
            case MiniBossCombo.Attack3:
                currentState = MiniBossCombo.Idle;
                animator.SetBool("IsAttacking", false);
                animator.SetBool("IsWalking", false);
                Attacked = false;
                break;
        }
    }
    void ExecuteAttack(float damage, float range, int attackAnimationIndex)
    {
        animator.SetBool("IsAttacking", true);
        animator.SetBool("IsWalking", false);
        if(attackAnimationIndex == HitAnimationIndex)
        {
            Collider[] colliders = Physics.OverlapSphere(hitPosition.position, range);
            foreach(Collider collider in colliders){
                if(collider.tag == "Player")
                {
                    collider.GetComponent<PlayerStatus>().TakeDamage(damage);
                }
            }
        }
        else if(attackAnimationIndex == SpinningAnimationIndex)
        {
            Collider[] colliders = Physics.OverlapSphere(hitPosition.position, range);
            foreach(Collider collider in colliders){
                if(collider.tag == "Player")
                {
                    collider.GetComponent<PlayerStatus>().TakeDamage(damage);
                }
            }
        }
        else if(attackAnimationIndex == PukingAnimationIndex)
        {
            //GameObject Puke = Instantiate(pukePrefab, transform.position + transform.forward, transform.rotation);
            //Puke.transform.parent = transform;
        }
        else if(attackAnimationIndex == SummoningAnimationIndex)
        {
            //isMoving = false;
            //isAttacking = true;
            //for (int i = 0; i < MiniSpawnRate; i++)
            //{
            //    Vector2 randomPoint = Random.insideUnitCircle.normalized * summonRadius;
            //    Vector3 summonPosition = transform.position + new Vector3(randomPoint.x, 0, randomPoint.y);
            //    summonPosition.y = transform.position.y;
            //    GameObject smallEnemy = Instantiate(MiniSpawnPrefab, summonPosition, Quaternion.identity);
            //}
        }
        //Debug.Log("Delayed");
        //Delayed(1f);
    }
    private IEnumerator Delayed(float DelayTime)
    {        
        yield return new WaitForSeconds(DelayTime);
        isAttacking = false;
        isMoving = true;
        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsWalking", true);
        Attacked = false;
        Debug.Log("Set");
    }
}
