using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CabbageEnemy : GeneralEnemy
{
    public GameObject player;
	public GameObject VFX;
    float timeSinceRoll;
    public float rollTime; 
    public float rollCD;
    public float rollSpeed;
    public float moveSpeed;
    public Animator anim;
    Vector3 rollDir = Vector3.zero;
    Vector3 recoilDir = Vector3.zero;
    float recoilTime = 0.2f;
    float timeSinceRecoil;
    public float attackRange;
    bool canMove = true;
    enum State{
        None,
        Windup,
        Rolling,
        Recoilling
    };
    

    State state = State.None;
    public Transform wallCheck;

    float stunDur;
    float knockbackDur;
    Vector3 knockbackDir;
    public ResourcesSO cabbageSO;
    Vector3 playerKnockbackDir;
    [SerializeField] AudioSource chargeUpSFX;
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
            float distToPlayer = (player.transform.position - transform.position).magnitude;
            Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
            dirToPlayer.y = 0;

            timeSinceRoll += Time.deltaTime;
            timeSinceRecoil += Time.deltaTime;

            if(timeSinceRoll > rollTime && state == State.Rolling)
            {
                state = State.None;
                return;
            }

            if(state == State.Rolling){
				VFX.SetActive(true);
                RaycastHit hit;
                if(Physics.Raycast(wallCheck.position, rollDir, out hit)){
                    if(/*hit.transform.gameObject.layer == 8*/ Vector3.Distance(wallCheck.position, hit.point) < 2){
                        //Debug.Log("Hit");
                        if(hit.transform.tag == "Player")
                        {
                            hit.transform.gameObject.GetComponent<PlayerStatus>().AddTrauma(0.7f);
                            hit.transform.gameObject.GetComponent<PlayerStatus>().TakeDamage(20f);
                            hit.transform.gameObject.GetComponent<PlayerStatus>().Stun(0.5f);
                            playerKnockbackDir = hit.transform.position;
                            playerKnockbackDir.y = 0;
                            playerKnockbackDir.Normalize();
                            hit.transform.gameObject.GetComponent<PlayerController>().Knockback(playerKnockbackDir * 5 + hit.transform.position, 0.25f);

                        }
                        recoilDir = Vector3.Reflect(rollDir, hit.normal);
                        recoilDir.y = 0;
                        recoilDir.Normalize();
                        timeSinceRecoil = 0;
                        state = State.Recoilling;
                        anim.SetTrigger("Crash");
                        anim.SetBool("Rolling", false);
                    }
                }

                GetComponent<CharacterController>().SimpleMove(rollDir * rollSpeed);
                transform.forward = rollDir;

                return;
            } 

            if(timeSinceRoll > rollCD && distToPlayer < attackRange && state == State.None){
                state = State.Windup;
				VFX.SetActive(true);
                StartCoroutine("Roll");
                return;
            }
            if(state == State.None)
            {
				VFX.SetActive(false);
                anim.SetBool("Walking", true);
                anim.SetBool("Rolling", false);
                GetComponent<CharacterController>().SimpleMove(dirToPlayer * moveSpeed);
                transform.forward = dirToPlayer;
                return;

            }


            if(state == State.Recoilling){
				VFX.SetActive(false);
                GetComponent<CharacterController>().SimpleMove(recoilDir * 15f);
                if(timeSinceRecoil >= recoilTime){
                    state = State.None;
                }
            }
        }
    }

    IEnumerator Roll(){
        chargeUpSFX.Play();
        anim.SetBool("Walking", false);
        anim.SetTrigger("Charge");
        yield return new WaitForSeconds(1); 
        chargeUpSFX.Stop();    
        anim.SetBool("Rolling", true);     
        timeSinceRoll = 0;
        state = State.Rolling;
        rollDir = player.transform.position - transform.position;
        rollDir.y = 0; 
        rollDir.Normalize();
        
    }    
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        stunDur = 0.5f;
        knockbackDur = 0.1f;
        knockbackDir =  transform.position - player.transform.position;
        knockbackDir.y = 0;
        knockbackDir.Normalize();
        state = State.None;
    }

    public override void Die(){
        player.GetComponent<PlayerStatus>().GainResource(cabbageSO, 1);
        base.Die();
        
    }
    
    void OnDrawGizmos(){
        Gizmos.DrawRay(wallCheck.position, rollDir);
    }

    void OnCollisionEnter(Collision collision){
            if(state == State.Rolling){
                if(collision.transform.tag == "Player")
                {
                    collision.transform.gameObject.GetComponent<PlayerStatus>().AddTrauma(0.7f);
                    collision.transform.gameObject.GetComponent<PlayerStatus>().TakeDamage(20f);
                    collision.transform.gameObject.GetComponent<PlayerStatus>().Stun(0.5f);
                    playerKnockbackDir = collision.transform.position;
                    playerKnockbackDir.y = 0;
                    playerKnockbackDir.Normalize();
                    collision.transform.gameObject.GetComponent<PlayerController>().Knockback(playerKnockbackDir * 5 + collision.transform.position, 0.25f);

                }

                RaycastHit hit;
                if(Physics.Raycast(wallCheck.position, rollDir, out hit)){
                    recoilDir = Vector3.Reflect(rollDir, hit.normal);
                }
                else{
                    recoilDir = -rollDir;
                }
                recoilDir.y = 0;
                recoilDir.Normalize();
                timeSinceRecoil = 0;
                state = State.Recoilling;
                anim.SetTrigger("Crash");
                anim.SetBool("Rolling", false);
            } 
    }
}
