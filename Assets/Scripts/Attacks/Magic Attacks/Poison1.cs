using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison1 : BaseAttack
{
    public Poison1()
    {
        attackName = "Poison 1";
        attackDescription = "Simple Poison Spell";
        attackDamage = 10;
        attackCost = 5;
    }
}
