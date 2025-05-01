using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBossFightTrigger : MonoBehaviour
{
    [SerializeField] float checkRange; 
    [SerializeField] GameObject walls;
    [SerializeField] GameObject boss;
    [SerializeField] GameObject player;
    bool started;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(player.transform.position, transform.position) <= checkRange && !started){
            started = true;
            boss.SetActive(true);
            walls.SetActive(true);
        }
    }

    public void End(){
        walls.SetActive(false);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, checkRange);
    }
}
