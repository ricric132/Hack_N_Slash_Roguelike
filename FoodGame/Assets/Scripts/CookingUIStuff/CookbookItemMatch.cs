using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CookbookItemMatch : MonoBehaviour
{
    public List<CookbookItemSO> unlockedDishes;
    public List<CookbookItemSO> displayedDishes = new List<CookbookItemSO>();
    public List<ResourcesSO> selectedIngredients;
    public bool needUpdate;


    void Update(){
        if(needUpdate){
            updateBook();
            needUpdate = false;
        }
    }
    public void updateBook(){
        
        List<Transform> children = GetComponentsInChildren<Transform>(true).Skip(1).ToList();
        List<Transform> dishes = new List<Transform>();
        displayedDishes.Clear();

        foreach(Transform child in children){
            if(child.parent == transform){
                dishes.Add(child);
            }
        }
        Debug.Log(dishes.Count());
        foreach (Transform child in dishes){
            bool includeIngredient = true;
            foreach(ResourcesSO ingredient in selectedIngredients) {
                if(!child.gameObject.GetComponent<DishBookItem>().dish.ingredients.Contains(ingredient)){
                    includeIngredient = false;
                    break;
                }
            }

            if(includeIngredient == false){
                child.gameObject.SetActive(false);
            }
            else{
                child.gameObject.SetActive(true);
                displayedDishes.Add(child.gameObject.GetComponent<DishBookItem>().dish);
            }
                
            
        }
    }
    

}
