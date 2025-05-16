using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySelectButton : MonoBehaviour
{
    public GameObject enemy;

    public void SelectEnemy() 
    {
        GameObject selector = enemy.GetComponent<EnemyStateMachine>().selector;
        selector.SetActive(false);
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().Input2(enemy);//save input of enemy prefab
    }

    public void ToggleSelector()
    {
        GameObject selector = enemy.GetComponent<EnemyStateMachine>().selector;
        selector.SetActive(!selector.activeInHierarchy);
    }
} 
