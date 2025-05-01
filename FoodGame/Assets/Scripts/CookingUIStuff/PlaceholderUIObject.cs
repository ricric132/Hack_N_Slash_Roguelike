using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceholderUIObject : MonoBehaviour
{
    public float sizeDecreaseRate;
    float ySize;
    float defaultYSize;
    // Start is called before the first frame update
    void Awake()
    {
        ySize = GetComponent<RectTransform>().sizeDelta.y;
        defaultYSize = GetComponent<RectTransform>().sizeDelta.y;
    }

    // Update is called once per frame
    void Update()
    {
        ySize -= sizeDecreaseRate * defaultYSize * Time.unscaledDeltaTime;
        GetComponent<RectTransform>().sizeDelta = new Vector2(1 ,ySize);
        if(ySize <= 0){
            Destroy(gameObject);
        }
    }
}
