using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestThrower : MonoBehaviour
{
    public GameObject tomato;
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            GameObject _tomato = Instantiate(tomato, transform.position, Quaternion.identity);
            _tomato.GetComponent<ThrownTomato>().target = target.transform.position;

        }
    }
}
