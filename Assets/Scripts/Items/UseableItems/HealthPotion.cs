using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : BaseUseableItem
{
    int health;
    HealthPotion(int _health, string _name, string _description)
    {
        health = _health;
        name = _name;
        description = _description; 
    }
    HealthPotion()
    {
        health = 50;
        itemName = "Health Potion(Regular)";
        description = "Just a regular health potion";
    }
    public override void UseItem(BaseHero character)
    {
        character.currentHP = (character.currentHP + health <= character.baseHP) ? character.currentHP + health : character.baseHP;
    }
}
