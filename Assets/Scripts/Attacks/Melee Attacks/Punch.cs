using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : BaseAttack {
    public Punch() {
        attackName = "Punch";
        attackDescription = "A punch with your fist";
        attackDamage = 5;
        attackCost = 0;
    }
}
