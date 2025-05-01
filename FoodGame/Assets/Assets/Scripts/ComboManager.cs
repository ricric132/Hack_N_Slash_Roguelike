using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public enum ComboState
{
    Idle,
    Attack1,
    Attack2,
    Attack3,
    // add more combo states as needed
}

public class ComboManager : MonoBehaviour
{
    public VisualEffect Pound;
    public VisualEffect Hook;
    public VisualEffect Slash;
    public ComboState currentState;
    public Animator animator;
    public float lastAttackTime;
    public float maxComboDelay = 0.5f; // maximum time between attacks to keep the combo going
    // add properties for each attack in the combo
    public float attack1Damage = 10f;
    public float attack1Range = 1f;
    public float attack1AnimationTime = 1f;
    public int attack1AnimationIndex = 1;

    public float attack2Damage = 10f;
    public float attack2Range = 1f;
    public float attack2AnimationTime = 1f;
    public int attack2AnimationIndex = 2;

    public float attack3Damage = 10f;
    public float attack3Range = 1f;
    public float attack3AnimationTime = 0.8f;
    public int attack3AnimationIndex = 3;

    private float DelayTime = 0.5f;

    public PlayerController player;

    public Transform hitPosition;
    public Camera cam;
    Ray camRay;
    Vector3 mouseWorldPosition;

    public float enemyMagnetDeg; 
    public float enemyMagnetDist;

    public AudioSource slash;
    public AudioSource thirdslash;

    // add more attack properties as needed
    private void Start()
    {
        currentState = ComboState.Idle;
    }

    public void ExecuteCombo()
    {
        Vector3 dir = MagnetizeTarget();
        if(dir.magnitude < 100){
            transform.forward = dir;
        }
        
        switch (currentState)
        {
            case ComboState.Idle:
                currentState = ComboState.Attack1;
                animator.SetTrigger("Attack1");
                player.GetComponent<PlayerController>().IsAttacking = true;
                ExecuteAttack(attack1Damage, attack1Range, attack1AnimationIndex);
                break;
            case ComboState.Attack1:
                currentState = ComboState.Attack2;
                animator.SetTrigger("Attack2");
                player.GetComponent<PlayerController>().IsAttacking = true;
                ExecuteAttack(attack2Damage, attack2Range, attack2AnimationIndex);
                break;
            case ComboState.Attack2:
                currentState = ComboState.Attack3;
                animator.SetTrigger("Attack3");
                player.GetComponent<PlayerController>().IsAttacking = true;
                player.leaping = true;
                ExecuteAttack(attack3Damage, attack3Range, attack3AnimationIndex);
                break;
            case ComboState.Attack3:
                currentState = ComboState.Idle;
                player.GetComponent<PlayerController>().IsAttacking = false;
                //animator.SetInteger("Inter", 0);
                break;
        }
        lastAttackTime = Time.time;
    }

    void ExecuteAttack(float damage, float range, int attackAnimationIndex)
    {
        player.GetComponent<PlayerController>().canAttack = false;
        //if(attackAnimationIndex < 3)
        //{
        player.push();
        //}
        //animator.SetInteger("Inter", attackAnimationIndex);

        // trigger attack end event
        // apply attack damage
        StartCoroutine(DelayedVFX(attackAnimationIndex, DelayTime));
        
        
    }
    private IEnumerator DelayedVFX(int attackAnimationIndex, float DelayTime)
    {        
        if(attackAnimationIndex == 3)
        {
            yield return new WaitForSeconds(DelayTime);
            Pound.Play();
            thirdslash.Play();
        }
        else
        {
            yield return new WaitForSeconds(0.2f);
            if(attackAnimationIndex == 1)
            {
                Hook.Play();
                slash.Play();
            }
            else if(attackAnimationIndex == 2)
            {
                Slash.Play();
                slash.Play();
            }
        }
    }
    public void FinishAttack(){
        player.GetComponent<PlayerController>().canAttack = true;
        
        player.leaping = false;
        
        //animator.SetInteger("Inter", 0);

        Collider[] colliders = Physics.OverlapSphere(hitPosition.position, attack1Range);
        foreach(Collider collider in colliders){
            if(collider.tag == "Enemy")
            {
                collider.GetComponent<GeneralEnemy>().TakeDamage(attack1Damage);
                player.GetComponent<PlayerStatus>().playerHealth += attack1Damage * 0.05f;
            }
            if(collider.tag == "Miniboss")
            {
                collider.GetComponent<MiniBossStatus>().TakeDamage(attack1Damage);
                player.GetComponent<PlayerStatus>().playerHealth += attack1Damage * 0.05f;
            }
            if(collider.tag == "Bird")
            {
                collider.GetComponent<BirbScript>().TakeDamage(attack1Damage);
                player.GetComponent<PlayerStatus>().playerHealth += attack1Damage * 0.1f;
            }
        }

    }

    public void AllowMove(){
        if(player.GetComponent<PlayerController>().canAttack == true){
            player.GetComponent<PlayerController>().IsAttacking = false;
        }
    }

    private void Update()
    {
        if (Time.time - lastAttackTime > maxComboDelay)
        {
            animator.SetBool("InCombo", false);
            currentState = ComboState.Idle;
            player.GetComponent<PlayerController>().IsAttacking = false;
            player.GetComponent<PlayerController>().canAttack = true;
            animator.SetInteger("Inter", 0);
        }
        else{
            animator.SetBool("InCombo", true);
        }
    }

    Vector3 MagnetizeTarget(){
        Vector3 currentDir = transform.forward;
        currentDir.y = 0;
        Vector3 closestDir = Vector3.positiveInfinity;

        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, enemyMagnetDist);
        foreach(Collider enemy in enemiesInRange){
            if(enemy.tag == "Enemy"){
                Vector3 dirToEnemy = enemy.transform.position - transform.position;
                dirToEnemy.y = 0;
                
                if(Vector3.Angle(currentDir, dirToEnemy) < enemyMagnetDeg){
                    if(dirToEnemy.magnitude < closestDir.magnitude){
                        closestDir = dirToEnemy;
                    }
                }
            }
        }
        return closestDir;
        

        
    }

    void OnDrawGizmos(){
        Gizmos.DrawWireSphere(transform.position, enemyMagnetDist);
    }
}