using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIDrag : MonoBehaviour, IDragHandler, IDropHandler
{
    public Transform freedragUI;
    public Transform containerUI;
    public Vector3 dragSize;
    public GameObject placeHolderPrefab;
    public int listIndex;
    
    public void OnDrag(PointerEventData eventData){
        if(transform.parent == containerUI){
            GameObject placeholder = Instantiate(placeHolderPrefab, containerUI);
            placeholder.transform.SetSiblingIndex(listIndex);
        }
        transform.position = eventData.position;
        transform.SetParent(freedragUI);
        transform.localScale = dragSize;
    }

    public void OnDrop(PointerEventData eventData){
        transform.localScale = Vector3.one;
        Debug.Log(GetComponent<RectTransform>().position.x);
        if(GetComponent<RectTransform>().position.x < 300){
            transform.SetParent(containerUI);
        }
    }
}
