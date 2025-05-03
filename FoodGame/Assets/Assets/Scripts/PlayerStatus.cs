using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerStatus : MonoBehaviour
{
    public float playerHealth = 100f;
    public float playerMaxHealth = 100f;
    public GameObject healthFill;
    public Dictionary<ResourcesSO, int> resourcesList = new Dictionary<ResourcesSO, int>();
    public Dictionary<CookbookItemSO, int> cookedItems = new Dictionary<CookbookItemSO, int>();
    public TextMeshProUGUI hpText;
    public float trauma;
    public float stunDuration;
    [SerializeField] AudioSource TakenDamageSFX;
    private bool SFXplaying = false;

    void Start()
    {
        playerHealth = playerMaxHealth;
        
    }

    void Update()
    {
        updateUI();
        if(playerHealth <= 0)
        {
            Destroy(gameObject);
            RestartScene();
        }
        tickTimers();
    }

    public void RestartScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void TakeDamage(float damage)
    {
        if(!SFXplaying)
        {
            TakenDamageSFX.Play();
            SFXplaying = true;
            StartCoroutine(SFXCooldown());
        }
        playerHealth -= damage;
    }

    private IEnumerator SFXCooldown()
    {
        yield return new WaitForSeconds(2.0f); 
        SFXplaying = false;
    }

    public void GainResource(ResourcesSO resource, int amount){
        if(resourcesList.ContainsKey(resource)){
            resourcesList[resource] += amount;
        }
        else{
            resourcesList[resource] = amount;
        }

    }

    public void AddCookedItem(CookbookItemSO food){
        Debug.Log(food.name);
        if(cookedItems.ContainsKey(food)){
            cookedItems[food] += 1;
        }
        else{
            cookedItems[food] = 1;
        }
    }

    void updateUI(){
        hpText.text = "HP: " + playerHealth.ToString(); 
        healthFill.transform.localScale = new Vector3(playerHealth/playerMaxHealth, 1, 1);
    }

    public void AddTrauma(float trauma){
        this.trauma = Mathf.Min(this.trauma + trauma, 1f);
    }

    public void Stun(float stunDur){
        if(stunDur > stunDuration){
            stunDuration = stunDur;
        }
    }

    

    void tickTimers(){
        trauma = Mathf.Max(trauma - Time.deltaTime/3, 0);
        stunDuration = Mathf.Max(stunDuration - Time.deltaTime, 0);
    }
}
