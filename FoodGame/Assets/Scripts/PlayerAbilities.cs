using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAbilities : MonoBehaviour
{
    public Camera cam;
    Vector3 mouseWorldPosition;
    Ray camRay;
    public GameObject throwingKnifePrefab;
    public float TKCD;
    float timeSinceTK;
    public Image TKCDindicator;
    public GameObject dog;
    public float positionSwitchAbilityCD;
    public Image positionSwitchAbilityCDIndicator;
    float timeSincePositionSwitchAbility;
    public float positionSwitchAbilityArea;
    float positionSwitchAbilityDamage = 50;
    public GameObject indicator;
    public Transform knifeTarget;
    public float knifeJumpOffset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TickTimers();
        UpdateCDIndicator();

        if(timeSinceTK >= TKCD){
            knifeTarget = null;
        }
        
        camRay = cam.ScreenPointToRay(Input.mousePosition); 
        RaycastHit[] hits = Physics.RaycastAll(camRay);

        foreach(RaycastHit hit in hits){
            if(hit.collider.tag == "PlayerFloor")
            {
                mouseWorldPosition = hit.point; 
            }
        }

        if(Input.GetMouseButtonDown(0)){
            Slash();
        }

        if(Input.GetMouseButtonDown(1))
        {
            if(timeSinceTK >= TKCD){
                timeSinceTK = 0;
                GameObject knife = Instantiate(throwingKnifePrefab, transform.position, Quaternion.identity);
                Vector3 dir = (mouseWorldPosition - transform.position).normalized;
                dir.y = 0f;
                knife.transform.forward = dir;
                knife.GetComponent<ThrowingKnife>().playerAbilities = this;
                knife.GetComponent<ThrowingKnife>().damage = 10;
                knife.GetComponent<ThrowingKnife>().dog = dog;
                
            }
            else if(knifeTarget != null){
                //transform.position = knifeTarget.position - (knifeTarget.position - transform.position).normalized * knifeJumpOffset;
            }
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            if(timeSincePositionSwitchAbility >= positionSwitchAbilityCD){
                timeSincePositionSwitchAbility = 0;
                dog.GetComponent<CharacterController>().enabled = false;
                GetComponent<CharacterController>().enabled = false;
                Vector3 dogPos = dog.transform.position;
                Vector3 selfPos = transform.position;

                transform.position = mouseWorldPosition;
                dog.transform.position = selfPos;

                Instantiate(indicator, transform.position, Quaternion.Euler(90, 0, 0)); //Replace with animation
                Collider[] colliders = Physics.OverlapSphere(transform.position, positionSwitchAbilityArea);
                foreach(Collider collider in colliders){
                    if(collider.tag == "Enemy")
                    {
                        collider.GetComponent<GeneralEnemy>().TakeDamage(positionSwitchAbilityDamage);
                    }
                }
                dog.GetComponent<nomnomDog>().positionSwitch();

                dog.GetComponent<CharacterController>().enabled = true;
                GetComponent<CharacterController>().enabled = true;
            }
            
        }

        if(Input.GetKeyDown(KeyCode.E)){

        }


    }

    void TickTimers()
    {
        timeSinceTK += Time.deltaTime;
        timeSincePositionSwitchAbility += Time.deltaTime;
    }

    void UpdateCDIndicator()
    {
        TKCDindicator.fillAmount = 1 - Mathf.Min(timeSinceTK/TKCD, 1);
        positionSwitchAbilityCDIndicator.fillAmount = 1 - Mathf.Min(timeSincePositionSwitchAbility/positionSwitchAbilityCD, 1);
    }

    void Slash(){
        GetComponent<FoodEffectHandler>().onHit();
    }

    void OnDrawGizmos(){
        Gizmos.DrawRay(camRay);
        Gizmos.DrawWireSphere(mouseWorldPosition, 2f);
    }
}
