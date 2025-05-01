using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject cabbagePrefab;
    public GameObject peasPrefab;
    public Transform spawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SpawnCabbage(){
        Instantiate(cabbagePrefab, spawnPoint.position, Quaternion.identity);
    }

    public void SpawnPeas(){
        Instantiate(peasPrefab, spawnPoint.position, Quaternion.identity);
    }
}
