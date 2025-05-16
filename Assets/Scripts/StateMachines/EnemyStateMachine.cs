using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    private BattleStateMachine BSM;
    public BaseEnemy enemy;
    public enum TurnState
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD
    }
    public TurnState currentState;

    //for the ProgressBar
    private float currentCooldown = 0f;
    private float maxCooldown = 5f;

    //this gameobject
    private Vector3 startPosition;

    //TimeForAction stuff
    private bool actionStarted = false;
    public GameObject heroToAttack;
    private HandleTurn chosenAction;
    private float animSpeed = 10f;

    //alive
    private bool alive = true;

    //Selector
    public GameObject selector;


    void Start()
    {
        currentState = TurnState.PROCESSING;
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        selector.SetActive(false);
        startPosition = transform.position;
    }
    void Update()
    {
        switch (currentState) { 
            case TurnState.PROCESSING:
                UpgradeProgressBar();
                break;
            case TurnState.CHOOSEACTION:
                ChooseAction();
                currentState = TurnState.WAITING;
                break;
            case TurnState.WAITING:
                //idle
                break;
            case TurnState.ACTION:
                StartCoroutine(TimeForAction());
                break;
            case TurnState.DEAD:
                if(!alive)
                {
                    return;
                }
                else
                {
                    //change tag of enemy
                    tag = "DeadEnemy";
                    //not attackable by heroes
                    BSM.enemiesInBattle.Remove(gameObject);
                    //disable selector
                    selector.SetActive(false);
                    //remove all inputs enemyattacks
                    if (BSM.enemiesInBattle.Count > 0)
                    {
                        for (int i = 0; i < BSM.performList.Count; i++)
                        {
                            if (BSM.performList[i].attackerTarget == gameObject)
                            {
                                BSM.performList[i].attackerTarget = BSM.enemiesInBattle[Random.Range(0, BSM.enemiesInBattle.Count)];
                            }
                            if (BSM.performList[i].attackerGameObject == gameObject)
                            {
                                BSM.performList.Remove(BSM.performList[i]);
                            }
                        }
                    }
                    //change the color to gray / play dead animation

                    alive = false;
                    //refresh enemy buttons
                    BSM.EnemyButtons();
                    //check alive
                    BSM.battleState = BattleStateMachine.PerformAction.CHECKALIVE;
                }
                break;
        
        }
    }


    void UpgradeProgressBar()
    {
        currentCooldown += Time.deltaTime;
        if(currentCooldown >= maxCooldown)
        {
            currentState = TurnState.CHOOSEACTION;
        }
    }

    void ChooseAction()
    {
        HandleTurn myAttack = new HandleTurn();
        myAttack.attacker = enemy.characterName;
        myAttack.type = "Enemy";
        myAttack.attackerGameObject = this.gameObject;
        myAttack.attackerTarget = BSM.heroesInBattle[Random.Range(0, BSM.heroesInBattle.Count)];

        myAttack.chosenAttack = enemy.attacks[Random.Range(0, enemy.attacks.Count)];
        BSM.CollectActions(myAttack);
        chosenAction = myAttack;
    }

    private IEnumerator TimeForAction()
    {
        if(actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        // animate enemy near the hero to attack
        Vector3 heroPosition = new Vector3(heroToAttack.transform.position.x , heroToAttack.transform.position.y, heroToAttack.transform.position.z + 1.5f);
        while (MoveTo(heroPosition))
        {
            yield return null;
        }

        //wait
        yield return new WaitForSeconds(0.5f);

        //do damage
        DoDamage();

        //animate back to start position
        Vector3 firstPosition = startPosition;
        while (MoveTo(firstPosition))
        {
            yield return null;
        }

        //remove this performer from the list in BSM
        BSM.performList.RemoveAt(0);
        chosenAction = null;

        //reset BSM -> wait
        BSM.battleState = BattleStateMachine.PerformAction.WAIT;

        actionStarted = false;

        //reset enemy state
        currentCooldown = 0;
        currentState = TurnState.PROCESSING;
    }

    private bool MoveTo(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));

    }

    void DoDamage()
    {
        if (chosenAction == null)
        {
            return;
        }
        float totalDamage = enemy.currentATK + chosenAction.chosenAttack.attackDamage;
        heroToAttack.GetComponent<HeroStateMachine>().TakeDamage(totalDamage);
    }

    public void TakeDamage(float damage)
    {
        enemy.currentHP -= damage;
        if (enemy.currentHP <= 0)
        {
            enemy.currentHP = 0;
            currentState = TurnState.DEAD;
        }
    }

}
