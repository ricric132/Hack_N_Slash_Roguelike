using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCharacterMovement : MonoBehaviour
{
    public float speed;
    public float gravity; 
    public float dashTime;
    public float dashSpeed;
    bool isDashing;
    Vector3 velocity;
    Vector3 savedDashDir;
    float timeSinceDash = 10;
    public float dashCD;
    public Vector3 averageVelocity;

    Queue<Vector3> velocityQueue;
    public int numOfVelocities;

    public GameObject cam;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public float trauma;

    public CameraManager cameraManager;
    public bool canMove;
    // Start is called before the first frame update
    void Start()
    {
        velocityQueue = new Queue<Vector3>();
        Physics.IgnoreLayerCollision(6 , 6, true);
        Physics.IgnoreLayerCollision(7 , 6, true);
        Physics.IgnoreLayerCollision(7 , 9, true);
        Physics.IgnoreLayerCollision(6 , 9, true);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q)){
            Debug.Log(cam.transform.forward);
            Debug.Log(cam.transform.right);
        }

        
        if(timeSinceDash >= dashTime && isDashing){
            isDashing = false;
            Physics.IgnoreLayerCollision(7 , 10, false);
        }

        if(canMove){
             velocity = new Vector3();
            float x = Input.GetAxisRaw("Horizontal");
            float y = -gravity;
            float z = Input.GetAxisRaw("Vertical");
            Vector3 moveDir = Vector3.zero;
            Vector3  dir = new Vector3(x, 0f, z).normalized;

            if(dir.magnitude >= 0.1){

                
                float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.transform.rotation.eulerAngles.y;
                //float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cameraManager.defaultRotation.y;

                //float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

                moveDir = (Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward).normalized; 
            }

            if(Input.GetKeyDown(KeyCode.Space) && timeSinceDash >= dashCD){
                savedDashDir =  moveDir * dashSpeed;
                isDashing = true;
                Physics.IgnoreLayerCollision(7 , 10, true);
                timeSinceDash = 0;      
                    
                velocityQueue.Clear();
                averageVelocity = Vector3.zero;
                numOfVelocities = 0;
            }



            if(!isDashing){
                velocity = moveDir * speed;

                if(velocityQueue.Count >= 30){
                    Vector3 pastVelocity =  velocityQueue.Dequeue();
                    averageVelocity -= pastVelocity;
                    if(pastVelocity != new Vector3(0, 0, 0)){
                        numOfVelocities -= 1;
                    }
                    
                }
                

                velocityQueue.Enqueue(velocity);
                averageVelocity += velocity;
                if(velocity != new Vector3(0, 0, 0)){
                    numOfVelocities += 1;
                }

                velocity = velocity + new Vector3(0, y, 0);
            }
            else{
                velocity = savedDashDir;
            }


            GetComponent<CharacterController>().SimpleMove(velocity);
        }

   
        tickTimers();
        

    }

    
    public void AddTrauma(float trauma){
        this.trauma = Mathf.Min(this.trauma + trauma, 0.5f);
    }


    void tickTimers(){
        timeSinceDash += Time.deltaTime;
        trauma = Mathf.Max(trauma - Time.deltaTime, 0);
    }
}
