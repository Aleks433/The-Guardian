using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUseableItem : MonoBehaviour
{
    public string itemName;
    public string description;
    public abstract void UseItem(BaseHero character);
}
