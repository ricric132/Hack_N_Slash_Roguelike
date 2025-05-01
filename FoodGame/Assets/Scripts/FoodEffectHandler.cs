using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FoodEffectHandler : MonoBehaviour
{
    public List<CookbookItemSO> foodsToAdd = new List<CookbookItemSO>();
    public GameObject tomatoPrefab;

    public Dictionary<CookbookItemSO, int> allFoods = new Dictionary<CookbookItemSO, int>();

    // Start is called before the first frame update
    void Start()
    {
        foreach(CookbookItemSO food in foodsToAdd){
            allFoods[food] = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onHit(){
        foreach(KeyValuePair<CookbookItemSO, int> food in allFoods){
            if(food.Value != 0)
            {
                if(food.Key.onHit){
                    StartCoroutine(food.Key.name + "_OnHit", food.Value);
                }
            }
        }
    }

    IEnumerator GardenSalad_OnHit(int amount){
        for(int i = 0; i < amount; i++){
            Transform target = null;
            List<Transform> enemies = new List<Transform>();
            Collider[] inrange = Physics.OverlapSphere(transform.position, 30);

            foreach(Collider collider in inrange){
                if(collider.gameObject.tag == "Enemy"){
                    enemies.Add(collider.gameObject.transform);
                }
            }

            if(enemies.Count > 0){
                target = enemies[UnityEngine.Random.Range(0, enemies.Count-1)];
            }

            if(target == null){
                yield break;
            }

            if(UnityEngine.Random.Range(0, 100) > 50){
                GameObject tomato = Instantiate(tomatoPrefab, transform.position, Quaternion.identity);
                tomato.GetComponent<ThrownTomato>().target = target.position;
            }

        }
    }

    


}
