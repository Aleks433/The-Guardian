using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : BaseAttack
{
    public Slash()
    {
        attackName = "Slash";
        attackDescription = "A swing with your sword";
        attackDamage = 10f;
        attackCost = 0;
    }
}
