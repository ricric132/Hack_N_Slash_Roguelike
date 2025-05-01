using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BellyMenuFoodItem : MonoBehaviour, IDragHandler, IDropHandler
{
    public ResourcesSO foodSO;
    public CookbookItemMatch cookbookItemMatch; 
    bool selected = false;
    public Transform freedragUI;
    public Transform containerUI;
    public Vector3 dragSize;
    public GameObject placeHolderPrefab;
    RectTransform containerRect;
    public int listIndex;
    public int amount;
    public TextMeshProUGUI amountText;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Image>().sprite = foodSO.image;   
        RectTransform containerRect = containerUI.gameObject.GetComponent<RectTransform>();
        
    }

    // Update is called once per frame
    void Update()
    {
        amountText.text = "X" + amount.ToString();
    }

    public void OnDrag(PointerEventData eventData){
        if(transform.parent == containerUI){
            listIndex = transform.GetSiblingIndex();
            GameObject placeholder = Instantiate(placeHolderPrefab, containerUI);
            placeholder.transform.SetSiblingIndex(listIndex);
        }
        transform.position = eventData.position;
        transform.SetParent(freedragUI);
        transform.localScale = dragSize;
    }

    public void OnDrop(PointerEventData eventData){
        transform.localScale = Vector3.one;
        int index = Mathf.FloorToInt(((containerUI.gameObject.GetComponent<RectTransform>().rect.height + containerUI.gameObject.GetComponent<RectTransform>().rect.position.y) - GetComponent<RectTransform>().position.y + 590 + 100)/220);
        Debug.Log(GetComponent<RectTransform>().position.y);
        if(GetComponent<RectTransform>().position.x < 300){
            transform.SetParent(containerUI);
            transform.SetSiblingIndex(index);
        }

        if(transform.parent.tag != "BellyUI"){
            if(selected == false){
                cookbookItemMatch.selectedIngredients.Add(foodSO);
                cookbookItemMatch.updateBook();
            }
            selected = true;
        }
        else{
            if(selected == true){
                cookbookItemMatch.selectedIngredients.Remove(foodSO);
                cookbookItemMatch.updateBook();
            }
            selected = false;
        }
    }
}
