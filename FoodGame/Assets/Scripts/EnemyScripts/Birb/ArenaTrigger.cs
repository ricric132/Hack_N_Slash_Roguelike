using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaTrigger : MonoBehaviour
{
    [SerializeField] GameObject boss;
    [SerializeField] GameObject walls;
    [SerializeField] GameObject UI;

    [SerializeField] GameObject player;

    [SerializeField] Transform tpPoint;
    [SerializeField] Transform fightTrigger;
    [SerializeField] float triggerRange = 30f;

    bool triggered = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(player.transform.position, fightTrigger.position) < triggerRange && !triggered){
            TriggerFight();
        }
    }

    void TriggerFight(){
        Debug.Log("fight started"); 
        triggered = true;
        boss.SetActive(true);
        walls.SetActive(true);
        UI.SetActive(true);

        //player.transform.position = tpPoint.position;
    }
}
