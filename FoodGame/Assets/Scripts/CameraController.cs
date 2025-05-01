using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraShakeAmount;
    public GameObject currentCam;
    public Vector3 defaultRotation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 offsetRotation = new Vector3(Mathf.PerlinNoise(Time.time, 0), Mathf.PerlinNoise(Time.time, 200), Mathf.PerlinNoise(Time.time, 100)) * Mathf.Pow(GetComponent<PlayerStatus>().trauma, 2)* cameraShakeAmount;
        currentCam.transform.rotation =  Quaternion.Euler(offsetRotation + defaultRotation); 
    }
}
