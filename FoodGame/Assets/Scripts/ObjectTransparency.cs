using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTransparency : MonoBehaviour
{
    public Transform player; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit[] hits = Physics.BoxCastAll(transform.position,  new Vector3(0f, 0f, 0), (player.position - transform.position).normalized);
        foreach(RaycastHit hit in hits){
            if(hit.transform.gameObject.TryGetComponent<TransparencySwitch>(out TransparencySwitch transparency)){
                transparency.goTrans();
            }
        }
    }
    
    void OnDrawGizmos(){
        //Gizmos.DrawWireCube(transform.position,  new Vector3(Vector3.Distance(player.position, transform.position), 5, 5), player.position - transform.position);
        //Gizmos.dra
    }
}
