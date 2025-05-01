using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera[] characterHighFollow =  new Cinemachine.CinemachineVirtualCamera[4];
    Vector3[] defaultRotations =  new Vector3[4];
    public Cinemachine.CinemachineVirtualCamera currentCam;
    int currentCamIndex;
    public GameObject player;
    Vector3 offsetRotation;
    public Vector3 defaultRotation;

    public float cameraShakeAmount;


    // Start is called before the first frame update
    void Start()
    {
        
        for(int i = 0; i < 4; i++)
        {
            defaultRotations[i] = characterHighFollow[i].transform.rotation.eulerAngles;
        }
    }

    // Update is called once per frame
    void Update()
    {
        defaultRotation = defaultRotations[currentCamIndex];

        characterHighFollow[0].Priority = 0;
        characterHighFollow[1].Priority = 0;
        characterHighFollow[2].Priority = 0;
        characterHighFollow[3].Priority = 0;

        currentCam.Priority += 4;

        int a = (currentCamIndex - 1) % 4;
        int b = (currentCamIndex + 1) % 4;

        if(a == -1){
            a = 3;
        }

        characterHighFollow[a].Priority += 2;
        characterHighFollow[b].Priority += 2;
        
        

        
        
        Cinemachine.CinemachineVirtualCamera closestCam = null;
        float closestTurn = Mathf.Infinity;

        foreach(Cinemachine.CinemachineVirtualCamera cam in characterHighFollow){
            RaycastHit hit;

            /*
            if(Physics.SphereCast(cam.transform.position, 0.1f, player.transform.position - cam.transform.position, out hit)){
                Debug.Log(hit.collider.tag);
                if(hit.collider.tag == "Terrain")
                {
                    cam.Priority = 0;
                }
            }
            */

            if(Physics.Linecast(player.transform.position, cam.transform.position, out hit)){
                Debug.Log(hit.collider.tag);
                if(hit.collider.tag == "Terrain")
                {
                    cam.Priority = 0;
                }
            }

            if(Physics.Linecast(cam.transform.position, player.transform.position, out hit)){
                Debug.Log(hit.collider.tag);
                if(hit.collider.tag == "Terrain")
                {
                    cam.Priority = 0;
                }
            }

            if(cam.Priority != 0){
                Vector3 dir = cam.transform.forward;
                dir.y = 0;
                dir.Normalize();
                if(closestTurn > (player.transform.forward - dir).magnitude){
                    closestTurn = (player.transform.forward - dir).magnitude;
                    closestCam = cam;
                }

            }
        }

        if(closestCam != null){
            closestCam.Priority += 1;
        }



        int counter = 0;
        foreach(Cinemachine.CinemachineVirtualCamera cam in characterHighFollow){
            if(Cinemachine.CinemachineCore.Instance.IsLive(cam)){
                currentCam = cam;
                currentCamIndex = counter;
            }
            counter++;
        }


        offsetRotation = new Vector3(Mathf.PerlinNoise(Time.time, 0), Mathf.PerlinNoise(Time.time, 200), Mathf.PerlinNoise(Time.time, 100)) * Mathf.Pow(player.GetComponent<PlayerStatus>().trauma, 2)* cameraShakeAmount;
        currentCam.transform.rotation =  Quaternion.Euler(offsetRotation + defaultRotation); 

    }

    void OnDrawGizmos(){

        foreach(Cinemachine.CinemachineVirtualCamera cam in characterHighFollow)
        {
            Gizmos.DrawLine(player.transform.position, cam.transform.position);
        }

    }
}
