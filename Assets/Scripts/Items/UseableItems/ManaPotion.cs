using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPotion : BaseUseableItem
{
    int mana;

    ManaPotion()
    {
        mana = 30;
        itemName = "Mana Potion(Regular)";
        description = "A regular mana potion";
    }


    public override void UseItem(BaseHero character)
    {
        character.currentMP = character.currentMP + mana <= character.baseMP ? character.currentMP + mana : character.baseMP;
    }
}
