using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BookPullup : MonoBehaviour
{
    public Transform pullUp;
    public Transform originalPos;
    public Transform peakPos;
    bool isSelected;
    public GameObject hitBox;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isSelected){
            hitBox.SetActive(false);
        }
        else{
            hitBox.SetActive(true);
        }
    }
    public void PullUp(){
        transform.DOMove(pullUp.position, 1);
        transform.DORotate(new Vector3(0, 0, -5), 1f);
        isSelected = true;
    }

    public void PutAway(){
        transform.DOMove(originalPos.position, 1f);

        transform.DORotate(new Vector3(0, 0, -5), 1f);        
        isSelected = false;
    }

    public void PutDown(){
        if(isSelected){
            return;
        }
        transform.DOMove(originalPos.position, 0.5f);
        transform.DORotate(new Vector3(0, 0, 0), 0.5f);
    }

    public void peak(){
        if(isSelected){
            return;
        }
        transform.DOMove(peakPos.position, 0.5f);
        transform.DORotate(new Vector3(0, 0, -10), 0.5f);
    }
}
