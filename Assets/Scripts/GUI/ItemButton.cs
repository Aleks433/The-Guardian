using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemButton : MonoBehaviour
{
    public BaseUseableItem itemToUse;
    
    public void UseItem()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().ChooseItem(itemToUse);
    }

}
