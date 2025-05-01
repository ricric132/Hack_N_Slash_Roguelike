using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BellyManager : MonoBehaviour
{
    GameObject[] children;
    int childObjectsAmount;
    public PlayerStatus playerInventory;
    public GameObject menuItemPrefab;
    public CookbookItemMatch cookbookItemMatch; 
    public Transform freedragUI;
    public Transform containerUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(childObjectsAmount != transform.childCount){
            childObjectsAmount = transform.childCount;
            UpdateArray();
        }
    }

    void UpdateArray(){
        children = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i).gameObject;
            children[i].GetComponent<UIDrag>().listIndex = i;
        }
    }

    public void CreateMenu(){
        Debug.Log("create");
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        
        foreach(KeyValuePair<ResourcesSO, int> resource in playerInventory.resourcesList){
            BellyMenuFoodItem item = Instantiate(menuItemPrefab, transform).GetComponent<BellyMenuFoodItem>();
            item.freedragUI = this.freedragUI;
            item.cookbookItemMatch = this.cookbookItemMatch;
            item.containerUI = this.containerUI;
            item.foodSO = resource.Key;
            item.amount = resource.Value;
        }
    }
}
