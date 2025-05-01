using Unity.VisualScripting;
using UnityEngine;

public class MinibossSpin : MinibossBaseState
{
    public override void EnterState(MinibossStateManager miniboss)
    {
        miniboss.animator.SetTrigger("Spin");
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
            miniboss.spinVFX.SetActive(true);
            if(miniboss.distanceToPlayer <= 6f)
            {
                miniboss.player.GetComponent<PlayerStatus>().TakeDamage(20f);
            }
            miniboss.attackindex += 1;
            miniboss.attacked = true;
        }
        if(miniboss.attacked && !miniboss.delayed)
        {
            StartDelayedAction();
            miniboss.delayed = true;
        }

        if (IsDelayComplete(3f))
        {
            miniboss.spinVFX.SetActive(false);
            miniboss.SwitchStates(miniboss.ChasingState);
            miniboss.delayed = false;
        }
    }
    public override void OnCollisionEnter(MinibossStateManager miniboss, Collision collision)
    {
        
    }
}