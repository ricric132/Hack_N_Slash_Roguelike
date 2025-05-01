using UnityEngine;

public class MinibossSpawn : MinibossBaseState
{
    public override void EnterState(MinibossStateManager miniboss)
    {
        miniboss.animator.SetTrigger("Summon");
        miniboss.attacked = false;
        miniboss.delayed = false;
    }
    public override void UpdateState(MinibossStateManager miniboss)
    {
        if(miniboss.distanceToPlayer >= miniboss.AttackRange && !miniboss.delayed)
        {
            miniboss.SwitchStates(miniboss.ChasingState);
        }
        else if (miniboss.attacked == false)
        {
            miniboss.SpawnMinis();
            miniboss.attackindex += 1;
            miniboss.attacked = true;
        }
        if(miniboss.attacked && !miniboss.delayed)
        {
            StartDelayedAction();
            miniboss.delayed = true;
        }

        if (IsDelayComplete(2f))
        {
            miniboss.SwitchStates(miniboss.ChasingState);
            miniboss.delayed = false;
        }
    }
    public override void OnCollisionEnter(MinibossStateManager miniboss, Collision collision)
    {

    }
}
