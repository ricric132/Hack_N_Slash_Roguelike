using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CookedItem", menuName = "ScriptableObjects/CookedItem")]
public class CookbookItemSO : ScriptableObject
{
    public string name;
    public Sprite image;
    public string description;
    public List<ResourcesSO> ingredients;
    public List<int> amounts;

    public bool onHit;

    public bool onKill;

    public bool onTakeDamage;
}
