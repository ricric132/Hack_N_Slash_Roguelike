using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirbAnimEventHandler : MonoBehaviour
{
    public BirbScript mainScript;

    public void AimPeck(){
        StartCoroutine(mainScript.AimPeck());
    }

    public void PeckLunge(){
        StartCoroutine(mainScript.PeckLunge());
    }

    public void PeckDamage(){
        mainScript.DoPeckDamage();
    }

    public void EndPecking(){
        mainScript.isPecking = false;
    }

    public void Jump(){
        StartCoroutine(mainScript.Jump());
    }

    public void TailWhipAttack(){
        mainScript.TailHitbox();
    }

    public void TailWhipEnd(){
        mainScript.TailEnd();
    }

    public void FireTornado(){
        mainScript.FireTornado();
    }

    public void StartSpin(){
        mainScript.StartSpin();

    }

    public void EndSpin(){
        StartCoroutine(mainScript.SpinAttackEnd());
    }

    public void EnterVolcano(){

    }


}
