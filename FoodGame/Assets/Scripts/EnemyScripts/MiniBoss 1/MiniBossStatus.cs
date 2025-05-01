using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniBossStatus : MonoBehaviour
{
    public float MiniBossHealth = 200f;
    public float MiniBossMaxHealth = 200f;
    public EnemySpawnManager enemySpawnManager;
    public GameObject player;

    public GameObject healthBar;
    public GameObject healthBarObject;
    [SerializeField] GameObject arena;
    [SerializeField] ResourcesSO tomatoSO;
    
    


    public void Start()
    {
        //enemySpawnManager = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<EnemySpawnManager>();
        MiniBossHealth = MiniBossMaxHealth;
        player = GameObject.Find("Player");
        
    }
    private void UpdateHealthBar()
    {
        healthBar.transform.localScale = new Vector3(Mathf.Clamp(MiniBossHealth/MiniBossMaxHealth, 0, 1), 1, 1);
    }

    public void Update()
    {
        if(healthBarObject.activeSelf == false){
            healthBarObject.SetActive(true);
            UpdateHealthBar();
        }
    }

    public void TakeDamage(float damage)
    {
        MiniBossHealth -= damage;        
        if(MiniBossHealth <= 0)
        {
            //enemySpawnManager.MinibossKilled = true;
            player.GetComponent<PlayerStatus>().GainResource(tomatoSO, 1);
            healthBarObject.SetActive(false);
            arena.SetActive(false);
            Destroy(gameObject);
        }
        UpdateHealthBar();
    }
}
