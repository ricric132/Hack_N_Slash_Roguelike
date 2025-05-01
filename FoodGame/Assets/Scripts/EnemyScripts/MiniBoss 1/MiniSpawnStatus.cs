using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniSpawnStatus : MonoBehaviour
{
    public float MiniSpawnHealth = 20f;
    public float MiniSpawnMaxHealth = 20f;

    void Start()
    {
        MiniSpawnHealth = MiniSpawnMaxHealth;
    }

    void Update()
    {
        if(MiniSpawnHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        MiniSpawnHealth -= damage;
    }
}