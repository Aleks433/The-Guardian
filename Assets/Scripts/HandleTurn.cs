using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTurn{
    public string attacker; //name of attacker
    public string type;
    public GameObject attackerGameObject;//who attacks
    public GameObject attackerTarget;//who is going to be targeted

    public BaseAttack chosenAttack;
}
