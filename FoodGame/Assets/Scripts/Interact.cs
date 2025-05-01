using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera cookingCam;

    public bool isCooking;
    public GameObject cookingUI;

    public GameObject combatUI;
    public Transform crockPotSpawnLocation;
    public GameObject crockPotPrefab; 
    GameObject activeCrockPot;
    bool crockPotExists;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F)){
            isCooking = ! isCooking;
        }

        if(isCooking){
            cookingCam.Priority = 11;
            cookingUI.SetActive(true);
            combatUI.SetActive(false);
            GetComponent<PlayerController>().canMove = false;
            if(crockPotExists == false){
                crockPotExists = true;
                activeCrockPot = Instantiate(crockPotPrefab, crockPotSpawnLocation.position, Quaternion.identity);
            }
        }
        else{
            cookingCam.Priority = 0;
            combatUI.SetActive(true);
            cookingUI.SetActive(false);
            GetComponent<PlayerController>().canMove = true;
            if(activeCrockPot == true){
                crockPotExists = false;
                Destroy(activeCrockPot);
            }
        }

    }
}
