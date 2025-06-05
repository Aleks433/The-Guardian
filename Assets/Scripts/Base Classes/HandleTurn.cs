using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTurn{
    public string initiatorName; //name of attacker
    public string type;
    public enum TurnType
    {
        ATTACK,
        ITEM
    }
    public TurnType turnType;
    public GameObject initiatorGameObject;//who attacks
    public GameObject target;//who is going to be targeted

    public BaseAttack chosenAttack;
    public BaseUseableItem chosenItem;
}
