using UnityEngine;

public abstract class MinibossBaseState
{
    private float delayStartTime = -1f;
    public abstract void EnterState(MinibossStateManager miniboss);
    public abstract void UpdateState(MinibossStateManager miniboss);
    public abstract void OnCollisionEnter(MinibossStateManager miniboss, Collision collision);
    protected void StartDelayedAction()
    {
        delayStartTime = Time.time;
    }

    protected bool IsDelayComplete(float duration)
    {
        return Time.time - delayStartTime >= duration;
    }
}
