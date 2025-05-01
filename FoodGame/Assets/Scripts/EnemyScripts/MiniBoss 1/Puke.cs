using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puke : MonoBehaviour
{
    public float damagePerSecond = 5f; // DPS value
    public float damageDuration = 2f;   // Time duration for damage to occur

    private bool isDamaging = false;

    public bool active = false;
    public MinibossStateManager minibossStateManager;
    [SerializeField] Vector3 hitboxSize;
    float timeElapsed;
    [SerializeField] PlayerStatus player;
    [SerializeField] Transform hitboxStart;
    private void Update() {
        if(minibossStateManager.PukeActive == true){
            Collider[] hits = Physics.OverlapBox(hitboxStart.position, hitboxSize);
            foreach(Collider collider in hits){
                if(collider.tag == "Player"){
                    timeElapsed = 0f;
                }
            }
        }

        if(timeElapsed < damageDuration){
            float damageThisFrame = damagePerSecond * Time.deltaTime;
            player.TakeDamage(damageThisFrame);
        }

        timeElapsed += Time.deltaTime;

    }
    /*
    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && minibossStateManager.PukeActive == true)
        {
            isDamaging = true;
            StartCoroutine(ApplyDamageOverTime(other.gameObject));
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && minibossStateManager.PukeActive == true)
        {
            isDamaging = false;
        }
    }
    
    private IEnumerator ApplyDamageOverTime(GameObject target)
    {
        float timeElapsed = 0f;

        while (isDamaging && timeElapsed < damageDuration)
        {
            // Calculate damage for this frame based on DPS
            float damageThisFrame = damagePerSecond * Time.deltaTime;

            // Apply the damage to the target
            target.GetComponent<PlayerStatus>().TakeDamage(damageThisFrame);

            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
    */

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(hitboxStart.position, hitboxSize);
    }

}
