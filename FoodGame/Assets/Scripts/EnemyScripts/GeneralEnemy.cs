using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralEnemy : MonoBehaviour
{
    public float maxHP;
    float HP;

    public List<Renderer> renderedParts = new List<Renderer>();
    public List<Color> baseColors = new List<Color>();
    public GameObject healthBarFill;
    public GameObject Canvas;
    public GameObject mCamera;

    [SerializeField] private Material emissiveMaterial;
    [SerializeField] private Renderer[] objectsToChange;
    [SerializeField] AudioSource GetHitSound;
    

    // Start is called before the first frame update
    public virtual void Start()
    {
        foreach(Renderer objectToChange in objectsToChange){
            emissiveMaterial = objectToChange.GetComponent<Renderer>().material;
            emissiveMaterial.DisableKeyword("_EMISSION");
        }
        mCamera = Camera.main.gameObject;
        HP = maxHP;
        foreach(Renderer part in renderedParts){
            baseColors.Add(part.material.color);
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        //emmisiveMaterial = objectToChange
        UpdateHealthBar();
    }

    public virtual void UpdateHealthBar(){
        healthBarFill.transform.localScale = new Vector3(HP/maxHP, 1, 1);
        Canvas.transform.LookAt(transform.position + mCamera.transform.rotation * Vector3.forward, mCamera.transform.rotation * Vector3.up);
    }

    public virtual void TakeDamage(float damage){
        //GetHitSound.Play();
        HP -= damage;
        if(HP <= 0){
            
            Die();
        }
        StartCoroutine("Flash");
    }

    public virtual void Die(){
        Destroy(gameObject);
    }

    IEnumerator Flash(){
        /*
        foreach(Renderer part in renderedParts){
            part.material.EnableKeyword("_EMISSION");
            //material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            part.material.SetColor("_EmmisionColor", Color.white);
            
        }
        */
        Debug.Log("flash");
        foreach(Renderer objectToChange in objectsToChange){
            emissiveMaterial = objectToChange.GetComponent<Renderer>().material;
            emissiveMaterial.EnableKeyword("_EMISSION");
        }

        yield return new WaitForSeconds(.25f);

        foreach(Renderer objectToChange in objectsToChange){
            emissiveMaterial = objectToChange.GetComponent<Renderer>().material;
            emissiveMaterial.DisableKeyword("_EMISSION");
        }
        /*
        for(int i = 0; i < renderedParts.Count; i++){
            renderedParts[i].material.EnableKeyword("_EMISSION");
            renderedParts[i].material.SetColor("_EmmisionColor", Color.black);
        }
        */
        //renderer.material.color = normalColor;
    }

}
