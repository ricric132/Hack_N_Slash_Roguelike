using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MinibossStateManager : MonoBehaviour
{
    MinibossBaseState currentState;
    public MinibossIdle IdleState = new MinibossIdle();
    public MinibossChase ChasingState = new MinibossChase();
    public MinibossMelee MeleeState = new MinibossMelee();
    public MinibossPuke PukingState = new MinibossPuke();
    public MinibossSpawn SpwaningState = new MinibossSpawn();
    public MinibossSpin SpinningState = new MinibossSpin();
    public Animator animator;
    public GameObject player;
    public VisualEffect Puke;
    public float MinibossEngageRange = 30f;
    public float AttackRange = 5f;
    public float distanceToPlayer = 0f;
    public int attackindex = 0;
    public bool attacked = false;
    public bool delayed = false;
    public int MiniSpawnRate = 3;
    public GameObject MiniTomatoe;
    public bool PukeActive = false;
    public GameObject spinVFX;
    public GameObject PukeCollider;
    void Start()
    {
        spinVFX.SetActive(false);
        currentState = IdleState;
        currentState.EnterState(this);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    public void Update()
    {
        if (attackindex > 3)
        {
            attackindex = 0;
        }
        currentState.UpdateState(this);
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if(PukeActive == false)
        {
            PukeCollider.SetActive(false);
        }
        else{
            PukeCollider.SetActive(true);
        }
    }
    public void SpawnMinis()
    {
        for (int i = 0; i < MiniSpawnRate; i++)
            {
               Vector2 randomPoint = Random.insideUnitCircle.normalized * 5;
               Vector3 summonPosition = transform.position + new Vector3(randomPoint.x, 0, randomPoint.y);
               summonPosition.y = transform.position.y;
               GameObject smallEnemy = Instantiate(MiniTomatoe, summonPosition, Quaternion.identity);
            }
    }

    void OnCollisionEnter(Collision collision)
    {
        currentState.OnCollisionEnter(this, collision);
    }
    public void SwitchStates(MinibossBaseState State)
    {
        currentState = State;
        State.EnterState(this);
    }
}
