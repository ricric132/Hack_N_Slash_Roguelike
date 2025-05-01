using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BirbScript : MonoBehaviour
{
    public List<Transform> lavaHoles;
    List<Transform> activeLavaHoles;
    public Transform closestLavaHole;

    public enum State
    {
        None,
        Retreating,
        Grounded,
        PeckAttack,
        SpinAttack,
        TailAttack,
        ChargeAttack,
        Charging,
        Flight,
        Swoop,
        TornadoAttack,
        Diving
        
    }

    public State currentState;
    CharacterController controller;
    public float moveSpeed;
    public GameObject player;
    public Animator anim;
    [SerializeField] float turnSpeed;

// Tail whip vars //////////////////////////////////////////////////////////////////////////////////////////
    [SerializeField] float tailwhipRange;
    [SerializeField] float tailwhipDamage;
    [SerializeField] float tailwhipAOE;
    [SerializeField] float tailwhipTrauma;

// Peck vars//////////////////////////////////////////////////////////////////////////////////////////////////////
    [SerializeField] float peckRange;
    [SerializeField] float peckMaxRotation;
    [SerializeField] float peckMaxMovement;
    [SerializeField] float peckTurnTime;
    [SerializeField] float peckMoveTime;
    Vector3 peckAimed;    
    [SerializeField] float peckTurnSpeed;
    [SerializeField] float peckDamage;    
    [SerializeField] float peckTrauma;

    public float peckAOE;
    public Transform peckLocation;
    public bool isPecking;

/////////////////////////////////////////////////////////////////////////////////////////////////
    [SerializeField] float spinRange;
    [SerializeField] float spinSpeed;
    [SerializeField] float spinMaxRange;
    [SerializeField] float spinAOE;
    [SerializeField] float spinDamage;


///////////////////////////////////////////////////////////////////////////////////////////
    [SerializeField] float chargeMinRange;
    [SerializeField] float chargeMaxRange;
    [SerializeField] float maxJumpHeight;
    [SerializeField] float landAOE;
    [SerializeField] float landDamage;
    [SerializeField] float landTrauma;
    [SerializeField] float jumpTime;
    bool midJump;

//////////////////////////////////////////////////////////////////////////////////////////////////


    [SerializeField] float fireBreatheRange;
    [SerializeField] GameObject fireBreathePrefab;
    [SerializeField] Transform fireBreatheStart;
    [SerializeField] float maxSteer;
    [SerializeField] float steeringSharpness;
    [SerializeField] float fireBreatheDamage;
    [SerializeField] Vector3 fireBreatheAOE;
    [SerializeField] Transform fireBreatheHitboxStart;


//////////////////////////////////////////////////////////////////////////////////////////////////


    float groundAttackCD = 3f;
    float flightAttackCD = 3f;
    [SerializeField] Cinemachine.CinemachineTargetGroup camTargetting;
    [SerializeField] float maxHP;
    [SerializeField] Transform healthBar;
    float currentHP;
    int numChargedAttacks;
    [SerializeField] float diveBombAOE;
    [SerializeField] float diveBombDamage;
    [SerializeField] Transform diveBombStart;
    [SerializeField] Cinemachine.CinemachineVirtualCamera birdCam;
    int randomizedAttackNum;
    bool isWalking;
    [SerializeField] Transform dockLocation;
    [SerializeField] GameObject tornadoPrefab;
    [SerializeField] Transform tornadoStart;
    [SerializeField] Cinemachine.CinemachineVirtualCamera highCam;
    
    //[SerializeField] Cinemachine.CinemachineVirtualCamera 

    [SerializeField] float steerStrength;
    [SerializeField] float flySpeed;
    float seed;
    float steering = 0;
    [SerializeField] Transform arenaCentre;
    [SerializeField] float arenaCentreTether;

    //////////////////////////////////////////////////////////////////////////////////////////
    [SerializeField] AudioSource WalkingSFX;
    [SerializeField] AudioSource PeckSFX;
    [SerializeField] AudioSource LandSFX;
    [SerializeField] AudioSource SpinSFX;
    [SerializeField] AudioSource TailWhipSFX;
    [SerializeField] AudioSource SquawkSFX;
    [SerializeField] AudioSource FlySFX;
    [SerializeField] AudioSource FireBreathSFX;

    //////////////////////////////////////////////////////////////////////////////////////////
    
    [SerializeField] VisualEffect Tornado;
    public GameObject pasueUI;

    void Start()
    {
        maxHP = 400f;
        //highCam.Priority = 20;
        activeLavaHoles = lavaHoles;
        controller = GetComponent<CharacterController>();
        currentHP = maxHP;
    }


    void Update()
    {
        UpdateHealthBar();
        if(currentState == State.Retreating)
        {
            //RetreatStateOnUpdate();

            //GamejamDuedateTemp
            Destroy(gameObject);
            Time.timeScale = 0; // Pause the game
            pasueUI.SetActive(true);
        }

        if(currentState == State.Grounded){
            GroundedStateOnUpdate();
        }
        else{
            isWalking = false;
        }

        if(currentState == State.Flight){
            FlightStateOnUpdate();
        }

        if(currentState == State.ChargeAttack){
            JumpStateOnUpdate();
        }
        else{
            midJump = false;
        }

        if(currentState == State.PeckAttack){

        }
        else{
            isPecking = false;
        }

        if(currentState == State.Charging){
            anim.CrossFade("Walking", 0.1f);
        }

        if(anim.GetCurrentAnimatorStateInfo(0).IsName("Walk")){
            WalkingSFX.UnPause();
        }
        else{
            WalkingSFX.Pause();
        }

        if(anim.GetCurrentAnimatorStateInfo(0).IsName("Flight")){
            FlySFX.UnPause();
        }
        else{
            FlySFX.Pause();
        }



    }

// STATE ONUPDATES ///////////////////////////////////////////////////////////////////////////////////////////////////////
    void RetreatStateOnUpdate()
    {
        anim.CrossFade("Walking", 0.1f);
        closestLavaHole = GetClosestTransform(activeLavaHoles, transform);
        Vector3 dir = (closestLavaHole.position - transform.position).normalized;     
        TurnTo(closestLavaHole);
        controller.SimpleMove(dir * moveSpeed);
        if(Vector3.Distance(closestLavaHole.position, transform.position) < 5){
            ResetVars();
            StartCoroutine(Recharge());
        }
    }

    void GroundedStateOnUpdate(){

        groundAttackCD -= Time.deltaTime;
        
        if(groundAttackCD < 0.1 && !isWalking){
            anim.CrossFade("Walk", 0.3f);
            isWalking = true;
        }

        if(groundAttackCD > 0){
            return;
        }

        float distToPlayer = Vector3.Distance(player.transform.position, transform.position);
        Vector3 dirToPlayer = (player.transform.position - transform.position);
        dirToPlayer.y = 0;
        dirToPlayer.Normalize();


        if(randomizedAttackNum == 0){
            if(distToPlayer < chargeMinRange){
                randomizedAttackNum = Random.Range(56, 101);
            }
            else{
                randomizedAttackNum = Random.Range(1, 101);
            }
        }


        
        
        if(randomizedAttackNum <= 55){
            if(distToPlayer <= chargeMaxRange){
                JumpCharge();
            }
            else{
                GetComponent<CharacterController>().SimpleMove(dirToPlayer * moveSpeed);
                transform.forward = dirToPlayer;
            }
        }
        else if(randomizedAttackNum <= 70){
            if(distToPlayer <= peckRange && !isPecking){
                StartCoroutine(PeckAttackChain());
                //transform.forward = dirToPlayer;
            }
            else{
                GetComponent<CharacterController>().SimpleMove(dirToPlayer * moveSpeed);
                transform.forward = dirToPlayer;
            }
        }
        else if(randomizedAttackNum <= 85){
            if(distToPlayer <= tailwhipRange){
                TailAttack();
            }
            else{
                GetComponent<CharacterController>().SimpleMove(dirToPlayer * moveSpeed);
                transform.forward = dirToPlayer;
            }
        }
        else if(randomizedAttackNum <= 100){
            if(distToPlayer <= spinRange){
                StartCoroutine(TurnToOverTime(player.transform, 0.3f));
                SpinAttack();
            }
            else{
                GetComponent<CharacterController>().SimpleMove(dirToPlayer * moveSpeed);
                transform.forward = dirToPlayer;
            }
        }
        
    }

    void JumpStateOnUpdate(){
        if(midJump){
            return;
        }

        TurnToWithSpeedTick(player.transform, turnSpeed);

        
    }

    void FlightStateOnUpdate(){
        if(numChargedAttacks <= 0){
            StartCoroutine(DiveBomb());
            return;
        }

        flightAttackCD -= Time.deltaTime;
        Vector3 dirToPlayer = player.transform.position - transform.position;
        dirToPlayer.y = 0;
        float distToPlayer = dirToPlayer.magnitude;
        dirToPlayer.Normalize();

        arenaCentre.position = new Vector3(arenaCentre.position.x, transform.position.y, arenaCentre.position.z);

        if(Vector3.Distance(transform.position, arenaCentre.position) > arenaCentreTether)
        {
            Vector3 dirToArenaCenter = (arenaCentre.position - transform.position).normalized;
            Vector3 steeringDir = Vector3.ClampMagnitude(dirToArenaCenter - transform.forward, maxSteer) * Time.deltaTime * steeringSharpness;
            transform.forward = (transform.forward + steeringDir).normalized;
            steering = 0;
        }
        else{
            steering +=(Mathf.PerlinNoise(seed * 1000 + 0.5f, Time.time) - 0.5f) * steerStrength;
            steering = Mathf.Clamp(steering, -100, 100);
            //Debug.Log(Mathf.PerlinNoise(seed * 1000 + 0.5f, Time.time));
            transform.forward = (transform.forward + transform.right * steering / 5000).normalized;
        }
        controller.Move(transform.forward * flySpeed * Time.deltaTime);
        
        if(flightAttackCD > 0){
            return;
        }

        if(randomizedAttackNum == 0){
            randomizedAttackNum = Random.Range(1, 101);
        }

        

        if(randomizedAttackNum <= 50){
            if(Vector3.Distance(transform.position, arenaCentre.position) < arenaCentreTether){
                StartCoroutine(TurnToOverTime(player.transform, 0.5f));
                StartCoroutine(TornadoAttack());
            }
        }
        else if(randomizedAttackNum <= 100){
            if(distToPlayer > fireBreatheRange + 5){
                StartCoroutine(FireBreathe());
            }
        }

       
    }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator Recharge(){
        Vector3 risePos = closestLavaHole.GetChild(0).position;
        currentState = State.Charging;
        anim.CrossFade("CrawlIntoHole", 0.1f);
        //play anim
        yield return new WaitForSeconds(3f);
        //camTargetting.m_Targets[camTargetting.FindMember(transform)].weight = 100;
        birdCam.Priority = 100;
        anim.CrossFade("VolcanoRise", 0.3f);
        yield return StartCoroutine(LinearMoveToOverTime(risePos, 0.5f));
        //play anim
        SquawkSFX.PlayDelayed(1f);
        yield return new WaitForSeconds(2f);
        //camTargetting.m_Targets[camTargetting.FindMember(transform)].weight = 1;
        birdCam.Priority = 0;
        activeLavaHoles.Remove(closestLavaHole);
        currentHP = maxHP;
        currentState = State.Flight;
        anim.CrossFade("Flight", 0.1f);
        numChargedAttacks = 20;
        highCam.Priority = 20;
        steering = 0;
    }

    public void EnterVolcano(){
        transform.position = closestLavaHole.GetChild(1).position;
    }

    IEnumerator PeckAttackChain()
    {
        currentState = State.PeckAttack;
        isPecking = true;
        
        anim.CrossFade("Peck", 0.3f);

        while (isPecking){
            yield return new WaitForEndOfFrame();
        }
        
        
        currentState = State.Grounded; 
        randomizedAttackNum = 0;
        groundAttackCD = 2f;
    }


    void JumpCharge(){
        currentState = State.ChargeAttack;
        anim.CrossFade("LeapAttack", 0.3f);
    }

    public IEnumerator Jump(){
        midJump = true;
        Vector3 startLocation = transform.position;
        Vector3 target = player.transform.position;
        Vector3 dif = target - startLocation;
        float heighestPoint = -dif.magnitude/2 * (dif.magnitude/2 - (dif.magnitude));
        float heightFactor = maxJumpHeight/heighestPoint;        
        float time = 0;

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

        LandSFX.Play();
        CreateHitbox(transform.position, landAOE, landDamage, landTrauma);
        player.GetComponent<PlayerStatus>().AddTrauma(0.3f);
        currentState = State.Grounded;
        randomizedAttackNum = 0;
        groundAttackCD = 0.5f;
    }

    void TailAttack(){
        currentState = State.TailAttack;
        anim.CrossFade("tailsweep", 0.1f);
        TailWhipSFX.PlayDelayed(0.8f);
    }

    public void TailHitbox(){
        CreateHitbox(transform.position, tailwhipAOE, tailwhipDamage, tailwhipTrauma);
    }

    public void TailEnd(){
        if(currentState == State.TailAttack){
            currentState = State.Grounded;
        }
        randomizedAttackNum = 0;
        groundAttackCD = 1f;
    }

    void SpinAttack(){
        currentState = State.SpinAttack;
        anim.CrossFade("SPIN", 0.5f);
    }

    public void StartSpin() {
        Tornado.Play();
        StartCoroutine("SpinAttackTrigger");
    }

    public IEnumerator SpinAttackTrigger(){
        float hitCD = 0.5f;
        SpinSFX.Play();
        while (true){
            hitCD -= Time.deltaTime;

            if(hitCD <= 0){
                Collider[] hits = Physics.OverlapSphere(transform.position, spinAOE);
                foreach(Collider hit in hits){
                    if(hit.tag == "Player"){
                        hit.GetComponent<PlayerStatus>().TakeDamage(spinDamage);
                        hitCD = 0.5f;
                    }
                }
            }

            controller.SimpleMove(transform.forward * spinSpeed);
            yield return new WaitForEndOfFrame();
        }
    }
    public IEnumerator SpinAttackEnd(){
        Tornado.Stop();
        StopCoroutine("SpinAttackTrigger");

        currentState = State.Grounded;
        randomizedAttackNum = 0;
        groundAttackCD = 1f;
        yield return null;
    }


    public IEnumerator AimPeck()
    {
        //CreateHitbox(peckLocations[num].position, peckAOE, peckDamage);

        
        Vector3 toPlayer = (player.transform.position - transform.position);
        toPlayer.y = 0;
        Vector3 dir = toPlayer.normalized;
        float dist = (toPlayer - new Vector3(0, toPlayer.y, 0)).magnitude;
        
        float DegToTurn = Vector3.SignedAngle(transform.forward, dir, Vector3.up);
        
        if(Mathf.Abs(DegToTurn) <= 5){
            yield break;
        }

        int turnDir = 1;

        if(DegToTurn < 0){
            turnDir = -1;
        }

        //Debug.Log(DegToTurn);

        float totalTurned = 0;

        while(totalTurned < Mathf.Abs(DegToTurn)){
            transform.forward = Quaternion.AngleAxis(peckTurnSpeed * Time.deltaTime * turnDir, Vector3.up) * transform.forward;
            totalTurned += peckTurnSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }        
    }

    public IEnumerator PeckLunge(){
        StopCoroutine(AimPeck());
        Vector3 toPlayer = (transform.position - player.transform.position);
        toPlayer.y = 0;
        Vector3 dir = toPlayer.normalized;
        float dist = (toPlayer - new Vector3(0, toPlayer.y, 0)).magnitude;

        if(dist <= peckRange){
            yield break;
        }

        float timer = 0;

        while(timer < peckMoveTime){
            controller.Move(transform.forward * peckMaxMovement * Time.deltaTime);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public void DoPeckDamage(){
        player.GetComponent<PlayerStatus>().AddTrauma(0.2f);
        PeckSFX.Play();
        CreateHitbox(peckLocation.position, peckAOE, peckDamage, peckTrauma);
    }

    IEnumerator TornadoAttack(){
        currentState = State.TornadoAttack;
        anim.CrossFade("Tornado", 0.1f);
        yield return new WaitForSeconds(4f);
        numChargedAttacks -= 2;
        anim.CrossFade("Flight", 0.1f);
        currentState = State.Flight;
        randomizedAttackNum = 0;
        flightAttackCD = 3f;
    }

    public void FireTornado(){
        Transform tornado = Instantiate(tornadoPrefab, tornadoStart.position, Quaternion.identity).transform;
        tornado.forward = transform.forward;
        tornado.GetComponent<TornadoScript>().arenaCentre = arenaCentre;

        tornado = Instantiate(tornadoPrefab, tornadoStart.position, Quaternion.identity).transform;
        tornado.forward = (transform.forward + transform.right).normalized;
        tornado.GetComponent<TornadoScript>().arenaCentre = arenaCentre;

        tornado = Instantiate(tornadoPrefab, tornadoStart.position, Quaternion.identity).transform;
        tornado.forward = (transform.forward -transform.right).normalized;
        tornado.GetComponent<TornadoScript>().arenaCentre = arenaCentre;
    }

    IEnumerator FireBreathe(){
        currentState = State.Swoop;
        Vector3 dirToPlayer;
        float distToPlayer;
        while(true){
            dirToPlayer = (player.transform.position - transform.position);     
            dirToPlayer.y = 0;
            distToPlayer = dirToPlayer.magnitude;
            dirToPlayer.Normalize();

            Vector3 desiredVelocity = dirToPlayer;
            Vector3 steering = Vector3.ClampMagnitude(desiredVelocity - transform.forward, maxSteer) * Time.deltaTime * steeringSharpness;
            transform.forward = (transform.forward + steering).normalized;

            controller.Move(transform.forward * flySpeed * Time.deltaTime);

            if(distToPlayer <= fireBreatheRange){
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        FireBreathSFX.Play();
        GameObject fireBreathe = Instantiate(fireBreathePrefab, fireBreatheStart);
        StartCoroutine(FireBreathDealDamage(fireBreathe));
        yield return LinearMoveWithSpeed(transform.position + dirToPlayer * 20f, flySpeed, true);

        steering = 0;
        numChargedAttacks -= 1;
        anim.CrossFade("Flight", 0.1f);
        currentState = State.Flight;
        randomizedAttackNum = 0;
        flightAttackCD = 2f;
    }

    IEnumerator FireBreathDealDamage(GameObject fireBreath){
        bool started = false;
        float hitCD = 0f;
        
        while (true){
            if(fireBreath.GetComponent<VisualEffect>().aliveParticleCount <= 0){
                if(started){
                    Destroy(fireBreath);
                    break;
                }
            }
            else{
                started = true;
            }

            hitCD -= Time.deltaTime;

            if(hitCD <= 0){
                Collider[] hits = Physics.OverlapBox(fireBreatheHitboxStart.position, fireBreatheAOE);
                foreach(Collider hit in hits){
                    if(hit.tag == "Player"){
                        hit.GetComponent<PlayerStatus>().TakeDamage(fireBreatheDamage);
                        hitCD = 0.5f;
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }

    }


    IEnumerator DiveBomb(){
        currentState = State.Diving;
        

        yield return LinearMoveWithSpeed(diveBombStart.position, 20f, true);
        Vector3 toPlayer = player.transform.position - transform.position;
        toPlayer.y = 0;
        transform.forward = toPlayer.normalized;
        anim.CrossFade("Divebomb", 0.3f);

        yield return LinearMoveToOverTime(player.transform.position, 2f);
        toPlayer = player.transform.position - transform.position;
        toPlayer.y = 0;
        transform.forward = toPlayer.normalized;

        player.GetComponent<PlayerStatus>().AddTrauma(0.5f);

        Collider[] hits = Physics.OverlapSphere(transform.position, diveBombAOE);
        foreach(Collider hit in hits){
            if(hit.tag == "Player"){
                hit.GetComponent<PlayerStatus>().TakeDamage(diveBombAOE);
            }
        }
        highCam.Priority = 0;
        currentState = State.Grounded;

    }



// UTILS /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    IEnumerator LinearMoveToOverTime(Vector3 target, float time){  
        float timer = 0;
        Vector3 startPos = transform.position;

        while(timer < time){
            //Debug.Log(timer + "/" + time);
            transform.position = Vector3.Lerp(startPos, target, timer/time);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    IEnumerator LinearMoveWithSpeed(Vector3 target, float speed, bool faceDir){  
        float timer = 0;
        float time = Vector3.Distance(target, transform.position)/speed;
        Vector3 velocity = (target - transform.position).normalized * speed;

        while(timer < time){
            GetComponent<CharacterController>().Move(velocity * Time.deltaTime);
            if(faceDir){
                transform.forward = velocity.normalized;
            }
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    Transform GetClosestTransform(List<Transform> choices, Transform start)
    {
        Transform tempClosest = null;
        float closestDist = Mathf.Infinity;
        foreach(Transform tranform in choices)
        {
            float dist = Vector3.Distance(tranform.position, start.position);
            if (dist < closestDist)
            {
                tempClosest = tranform;
                closestDist = dist;
            }
        }
        return tempClosest;
    }

    void TurnTo(Transform target){
        Vector3 LookAim = target.position - transform.position;
        LookAim.y = 0;
        transform.forward = LookAim;
    }

    IEnumerator TurnToOverTime(Transform target, float time){
        Vector3 LookAim = target.position - transform.position;
        LookAim.y = 0;
        float DegToTurn = Vector3.SignedAngle(transform.forward, LookAim, Vector3.up);
        float DegToTurnPerSec = DegToTurn/time;
        float timer = 0;

        while(timer < time){
            transform.forward = Quaternion.AngleAxis(DegToTurnPerSec * Time.deltaTime, Vector3.up) * transform.forward;
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

    }

    void TurnToWithSpeedTick(Transform target, float degPerSec){
        Vector3 LookAim = target.position - transform.position;
        LookAim.y = 0;
        float DegToTurn = Vector3.SignedAngle(transform.forward, LookAim, Vector3.up);
        float degThisFrame = degPerSec * Time.deltaTime;

        transform.forward = Quaternion.AngleAxis(Mathf.Clamp(DegToTurn, -degThisFrame, degThisFrame), Vector3.up) * transform.forward;
    }


    public void TakeDamage(float damage){
        if(currentState == State.Charging || currentState == State.Flight){
            return;
        }
        currentHP -= damage;
        if(currentHP <= 0){
            StopAllCoroutines();
            LookForHole();
        }
    }

    void CreateHitbox(Vector3 origin, float AOE, float damage, float trauma){
        Collider[] hits = Physics.OverlapSphere(origin, AOE);
        bool alreadyHit = false;
        foreach(Collider hit in hits){
            if(hit.tag == "Player" && !alreadyHit){
                alreadyHit = true;
                hit.GetComponent<PlayerStatus>().TakeDamage(damage);
                hit.GetComponent<PlayerStatus>().AddTrauma(trauma);
            }
        }
    }

    void LookForHole(){
        if(activeLavaHoles.Count > 0){
            anim.CrossFade("Walking", 0.1f);
            currentState = State.Retreating;
        }
        else{
            Die();
        }
    }
    void Die(){
        Destroy(gameObject);
    }

    void UpdateHealthBar(){
        healthBar.transform.localScale = new Vector3(Mathf.Clamp(currentHP/maxHP, 0, 1), 1, 1);
    }

    void ResetVars(){
        StopAllCoroutines();
        isPecking = false;
        isWalking = false;
        midJump = false;
    }

    private void OnDrawGizmos() {
        
       //Gizmos.DrawWireSphere(peckLocation.position, peckAOE);
       //Gizmos.DrawWireSphere(transform.position, chargeMinRange);
       //Gizmos.DrawWireSphere(transform.position, tailwhipAOE);
       //Gizmos.DrawWireSphere(transform.position, tailwhipRange);
       //Gizmos.DrawWireSphere(arenaCentre.position, arenaCentreTether);
       //Gizmos.DrawWireCube(fireBreatheHitboxStart.position, fireBreatheAOE * 2);
       Gizmos.DrawWireSphere(transform.position, landAOE);
        
    }
}
