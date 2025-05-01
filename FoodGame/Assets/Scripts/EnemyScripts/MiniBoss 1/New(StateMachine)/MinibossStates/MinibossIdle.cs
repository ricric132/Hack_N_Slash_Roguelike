using UnityEngine;

public class MinibossIdle : MinibossBaseState
{

    public override void EnterState(MinibossStateManager miniboss)
    {
        miniboss.animator.SetTrigger("Idle");
        miniboss.Puke.Stop();
    }
    public override void UpdateState(MinibossStateManager miniboss)
    {
        if(miniboss.distanceToPlayer <= miniboss.MinibossEngageRange)
        {
            miniboss.animator.SetTrigger("Chase");
            miniboss.SwitchStates(miniboss.ChasingState);
        }
    }
    public override void OnCollisionEnter(MinibossStateManager miniboss, Collision collision)
    {
        
    }
}
