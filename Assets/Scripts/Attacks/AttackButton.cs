using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButton : MonoBehaviour
{
    public BaseAttack magicAttackToPeform;

    public void CastMagicAttack()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().ChooseMagicAttack(magicAttackToPeform);
    }

}
