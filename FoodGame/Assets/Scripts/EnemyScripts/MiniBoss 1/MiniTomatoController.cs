using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MiniTomatoController : MonoBehaviour
{
    public GameObject player;
    public float speed = 5f;
    public float chargeTime = 2f;
    public bool PlayerFound = false;
    private Vector3 targetPosition;
    public float leapDistance = 5f;
    public float leapHeight = 2f;
    public float leapSpeed = 5f;
    public float damage = 5f;
    public bool leaped = false;
    private float distanceToPlayer;
    public GameObject Explosion;
    public float liveTime = 2f;
    private float Count = 0f;
    public GameObject explode;
    bool inAir = true;

    // Start is called before the first frame update
    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(Throw());
    }

    // Update is called once per frame
    public void Update()
    {
        if(inAir){
            return;
        }
        Count += Time.deltaTime;
        if(Count >= liveTime)
        {
            GameObject PFX = Instantiate(Explosion, transform.position, Quaternion.identity);
            Destroy(gameObject); 
        }
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if(distanceToPlayer <= 5f && !PlayerFound)
        {
            targetPosition = player.transform.position;
            PlayerFound = true;
        }
        else if(!PlayerFound)
        {
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            transform.position += directionToPlayer * speed * Time.deltaTime;
        }
        else if(PlayerFound)
        {
            float step = leapSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
            if (Vector3.Distance(transform.position, targetPosition) < 3f)
            {
                transform.position = targetPosition;
                Collider[] colliders = Physics.OverlapSphere(transform.position, 5f);
                foreach(Collider collider in colliders){
                    if(collider.tag == "Player")
                    {
                        collider.GetComponent<PlayerStatus>().TakeDamage(damage);
                    }
                }
                GameObject PFX = Instantiate(Explosion, transform.position, Quaternion.identity);
                GameObject SFX = Instantiate(explode, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
    IEnumerator Throw(){
        Vector3 startLocation = transform.position;
        Vector3 target = player.transform.position;
        target.x += Random.Range(-5, 5);
        target.z += Random.Range(-5, 5);
        Vector3 dif = target - startLocation;
        float heighestPoint = -dif.magnitude/2 * (dif.magnitude/2 - (dif.magnitude));
        float heightFactor = 20/heighestPoint;        
        float time = 0;
        float jumpTime = 2f;

        while (time <= jumpTime){
            float x = time/jumpTime * dif.magnitude;
            target.y = startLocation.y;
            //startLocation.y = 0;
            float y = (-x * (x - (dif.magnitude))) * heightFactor;
            Vector3 xz = Vector3.ClampMagnitude(dif, x);

            transform.position = new Vector3(xz.x, y, xz.z) + startLocation;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        inAir = false;
    }
}
