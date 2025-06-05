using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSelectButton : MonoBehaviour
{
    public GameObject selectedHero;
    public void SelectHero()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().ChooseHero(selectedHero);
    }
}
