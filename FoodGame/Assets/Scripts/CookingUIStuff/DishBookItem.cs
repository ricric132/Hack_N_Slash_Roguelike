using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DishBookItem : MonoBehaviour
{
    public CookbookItemSO dish;
    public Image foodImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;

    void Start(){
        foodImage.sprite = dish.image;
        nameText.text = dish.name;
        descText.text = dish.description;
    }
}
