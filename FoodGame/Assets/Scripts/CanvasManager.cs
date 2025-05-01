using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public GameObject cookingUI;
    public BellyManager bellyManager;
    public GameObject combatUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F)){
            cookingUI.SetActive(!cookingUI.activeSelf);
            combatUI.SetActive(!combatUI.activeSelf);
            bellyManager.CreateMenu();

        }

        if(cookingUI.activeSelf){
            //Time.timeScale = 0f;
        }
        else{
            //Time.timeScale = 1f;
        }

    }
}
