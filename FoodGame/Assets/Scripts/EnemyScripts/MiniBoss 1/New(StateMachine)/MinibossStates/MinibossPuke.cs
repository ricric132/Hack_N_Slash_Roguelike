using UnityEngine;

public class MinibossPuke : MinibossBaseState
{
    public override void EnterState(MinibossStateManager miniboss)
    {
        miniboss.animator.SetTrigger("Puke");
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
            miniboss.Puke.Play();
            miniboss.attackindex += 1;
            miniboss.PukeActive = true;
            miniboss.attacked = true;
        }
        if(miniboss.attacked && !miniboss.delayed)
        {
            StartDelayedAction();
            miniboss.delayed = true;
        }

        if (IsDelayComplete(2f))
        {
            miniboss.Puke.Stop();
            miniboss.PukeActive = false;
            miniboss.SwitchStates(miniboss.ChasingState);
            miniboss.delayed = false;
        }
    }
    public override void OnCollisionEnter(MinibossStateManager miniboss, Collision collision)
    {
        
    }
}
