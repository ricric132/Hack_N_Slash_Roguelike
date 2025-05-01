using UnityEngine;

public class MinibossMelee : MinibossBaseState
{
    public override void EnterState(MinibossStateManager miniboss)
    {
        miniboss.animator.SetTrigger("Hit");
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
            miniboss.attackindex += 1;
            miniboss.attacked = true;
            if(miniboss.distanceToPlayer <= 4f)
            {
                miniboss.player.GetComponent<PlayerStatus>().TakeDamage(8f);
            }
        }

        if(miniboss.attacked && !miniboss.delayed)
        {
            StartDelayedAction();
            miniboss.delayed = true;
        }

        if (IsDelayComplete(1f))
        {
            miniboss.SwitchStates(miniboss.ChasingState);
            miniboss.delayed = false;
        }
    }
    public override void OnCollisionEnter(MinibossStateManager miniboss, Collision collision)
    {
        
    }
}
