using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 10f;
    public float dashSpeed = 30f;
    public float dashDuration = 0.25f;
    public float dashCooldown = 2f;
    private bool canDash = true;
    private Vector3 dashDirection;
    public Animator animator;
    private ComboManager combo;
    public bool IsAttacking = false;
    public bool canAttack = true;

    public float jumpForce = 5f;
    public float forwardForce = 10f;
    public bool leaping = false;
    public bool skyfall = false;

    private Vector3 targetPosition;

    public Vector3 averageVelocity;

    Queue<Vector3> velocityQueue;
    public int numOfVelocities;
    public bool canMove = true;
    [SerializeField] AudioSource walkingSFX;
    [SerializeField] Transform stuckCheck;
   
    void Start()
    {
        velocityQueue = new Queue<Vector3>();
        combo = GetComponent<ComboManager>();
        GetComponent<CharacterController>().enableOverlapRecovery = true;
    }
    private void FixedUpdate()
    {
        if(skyfall)
        {
            Vector3 gravity = 20f * Physics.gravity * ((2f + Time.deltaTime) * 9.8f);

        }
        else
        {
            Vector3 gravity = 1f * Physics.gravity;

        }
    }
    void Update()
    {
        
        bool playWalkAudio = false;

        if(canMove == false){
            return;
        }

        GetUnstuck();

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if(transform.position.y > 1 && !leaping)
        {
            skyfall = true;
        }
        else
        {
            skyfall = false;
        }

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        if(leaping)
        {
            Leap();
        }
        if (movement.magnitude > 0f && canAttack )
        {
            
            float currentSpeed = movementSpeed;

            //Velocity//
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            if(IsAttacking == false){
                //Dash Direction//
                if (Input.GetKeyDown(KeyCode.Space) && canDash)
                {
                    dashDirection = movement.normalized;
                    StartCoroutine(DoDash());
                }
                else
                {
                    //Normal Movement//
                    GetComponent<CharacterController>().SimpleMove(movement * currentSpeed);
                    trackMovement(movement * currentSpeed);
                    animator.SetBool("IsMoving", true);
                    playWalkAudio = true;
                    
                }

                combo.currentState = ComboState.Idle;
            }
        }
        else
        {
            animator.SetBool("IsMoving", false);
            averageVelocity = Vector3.zero;
            velocityQueue.Clear();
            numOfVelocities = 0;
            //Stop Player From Falling Over Or Moving//
        }

        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            combo.ExecuteCombo();
        }

        if(playWalkAudio == true){
            walkingSFX.UnPause();
        }
        else{
            walkingSFX.Pause();
        }
    }
    IEnumerator DoDash()
    {
        canMove = false;
        Physics.IgnoreLayerCollision(7, 6, true);
        Physics.IgnoreLayerCollision(7, 10, true);
        canDash = false;
        Vector3 startPos = transform.position;

        //Dash Destination//
        Vector3 endPos = startPos + dashDirection * dashSpeed * dashDuration;

        //Move Over Time//
        float t = 0f;
        while (t < dashDuration) {
            transform.position = Vector3.Lerp(startPos, endPos, t / dashDuration);
            yield return null;
            t += Time.deltaTime;
        }

        //Innacurracy Fix (Temp)// 
        transform.position = endPos;

        Physics.IgnoreLayerCollision(7, 6, false);
        Physics.IgnoreLayerCollision(7, 10, false);
        canMove = true;
        //Cooldown//
        yield return new WaitForSeconds(dashCooldown);

        canDash = true;

    }
    public void Leap()
    {
    }

    public void push()
    {
        Vector3 targetPosition = transform.position + transform.forward * 2f;

        StartCoroutine(MovePlayerSmoothly(targetPosition, 0.25f));
    }

    public void Knockback(Vector3 velocity, float time){
        StartCoroutine(MovePlayerSmoothly(velocity, time));
    }

    private IEnumerator MovePlayerSmoothly(Vector3 targetPosition, float time)
    {
        float elapsedTime = 0f;
        float calcSpeed = Vector3.Distance(targetPosition, transform.position)/time;
        Vector3 velocity = (targetPosition - transform.position).normalized * calcSpeed;
        Vector3 startPosition = transform.position;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * 1/time;
            GetComponent<CharacterController>().SimpleMove(velocity);

            yield return null;
        }
    }

    void trackMovement(Vector3 velocity){
        

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
    }


    void GetUnstuck(){
        bool stuck = false;
        Transform objStuckIn = null;

        Collider[] colliders = Physics.OverlapSphere(stuckCheck.position, 0f);
        foreach(Collider collider in colliders){
            if(collider.transform != transform){
                stuck = true;
                objStuckIn = collider.transform;
            }
        }
        
        if(!stuck){
            return;
        }
        //Debug.Log(objStuckIn.name);
        int RaysToShoot = 30;
        float angle = 0;
        float closestDist = Mathf.Infinity;
        Vector3 closestExit = Vector3.zero;
        for (int i=0; i<RaysToShoot; i++) {
            float x = Mathf.Sin (angle);
            float y = Mathf.Cos (angle);
            angle += 2 * Mathf.PI / RaysToShoot;

            Vector3 dir = new Vector3 (transform.position.x + x, 0, transform.position.y + y);
            Ray ray = new Ray(transform.position, dir);
            
            ray.origin = ray.GetPoint(100);
            ray.direction = -ray.direction;

            
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                float dist = 100 - hit.distance;
                if(dist < closestDist && hit.transform == objStuckIn){
                    closestDist = dist;
                    closestExit = hit.point;
                }
                
            }
        }

        if(closestExit != Vector3.zero){
            transform.position = closestExit;
        }

    }

    
}