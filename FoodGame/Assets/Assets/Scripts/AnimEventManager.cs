using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventManager : MonoBehaviour
{
    public ComboManager comboManager;
    public void endAttack(){
        comboManager.FinishAttack();
    }

    public void AllowMove(){
        comboManager.AllowMove();
    }
}
