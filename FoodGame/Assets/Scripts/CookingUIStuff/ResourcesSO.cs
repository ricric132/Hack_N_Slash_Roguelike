using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Resources", menuName = "ScriptableObjects/Resource")]
public class ResourcesSO : ScriptableObject
{
    public string name;
    public Sprite image;
    public string description;
}
