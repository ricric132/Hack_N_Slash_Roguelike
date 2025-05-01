using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;

public class MinibossChase : MinibossBaseState
{
    public override void EnterState(MinibossStateManager miniboss)
    {
        miniboss.animator.SetTrigger("Chase");
        miniboss.Puke.Stop();
    }
    public override void UpdateState(MinibossStateManager miniboss)
    {
        Vector3 directionToPlayer = (miniboss.player.transform.position - miniboss.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        miniboss.transform.rotation = Quaternion.Slerp(miniboss.transform.rotation, lookRotation, Time.deltaTime * 5f);

        if(miniboss.distanceToPlayer >= miniboss.MinibossEngageRange)
        {
            miniboss.SwitchStates(miniboss.IdleState);
        }

        if(miniboss.distanceToPlayer <= miniboss.AttackRange)
        {
            if (miniboss.attackindex == 0)
            {
                miniboss.SwitchStates(miniboss.MeleeState);
            }
            else if (miniboss.attackindex == 1)
            {
                miniboss.SwitchStates(miniboss.PukingState);
            }
            else if (miniboss.attackindex == 2)
            {
                miniboss.SwitchStates(miniboss.SpwaningState);
            }
            else if (miniboss.attackindex == 3)
            {
                miniboss.SwitchStates(miniboss.SpinningState);
            }
            
        }
        else{
            miniboss.transform.position += directionToPlayer * 5 * Time.deltaTime;
        }
    }
    public override void OnCollisionEnter(MinibossStateManager miniboss, Collision collision)
    {
        miniboss.SwitchStates(miniboss.SpinningState);
    }
}
