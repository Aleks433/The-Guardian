using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class HeroStateMachine : MonoBehaviour
{
    private BattleStateMachine BSM;
    public BaseHero hero;

    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        SELECTING,
        ACTION,
        DEAD
    }
    public TurnState currentState;
    public HandleTurn action;

    //for ProgressBar
    private float currentCooldown = 0f;
    private float maxCooldown = 5f;
    public Image progressBar;
    public GameObject selector;

    //IEnumerator
    public GameObject enemyToAttack;
    private bool actionStarted = false;
    private Vector3 startPosition;
    private float animSpeed = 5f;

    //Dead
    private bool alive = true;

    //heroPanel
    private HeroPanelStats stats;
    public GameObject heroBarPrefab;
    private Transform heroPanelSpacer;

    void Start()
    {
        //find spacer object
        heroPanelSpacer = GameObject.Find("BattleCanvas").transform.Find("HeroPanel").transform.Find("HeroSpacer");

        //create panel, fill in info
        CreateHeroBar();
        startPosition = transform.position;
        selector.SetActive(false);
        currentCooldown = Random.Range(0, 2.5f);
        currentState = TurnState.PROCESSING;
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
    }

    void Update()
    {
        switch (currentState) { 
            case TurnState.PROCESSING:
                UpgradeProgressBar();
                break;
            case TurnState.ADDTOLIST:
                BSM.heroesToManage.Add(gameObject);
                currentState = TurnState.WAITING;
                break;
            case TurnState.WAITING:
                //idle
                break;
            case TurnState.SELECTING:
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
                    //Change tag
                    gameObject.tag = "DeadHero";

                    //not attackable by enemy
                    BSM.heroesInBattle.Remove(gameObject);

                    //not manageable
                    BSM.heroesToManage.Remove(gameObject);

                    //deactivate selector
                    selector.SetActive(false);

                    //reset GUI
                    BSM.actionPanel.SetActive(false);
                    BSM.enemySelectPanel.SetActive(false);

                    //remove turn from performList
                    if (BSM.heroesInBattle.Count > 0)
                    {
                        for (int i = 0; i < BSM.performList.Count; i++)
                        {
                            HandleTurn choice = BSM.performList[i];
                            if (choice.initiatorGameObject == gameObject)
                            {
                                BSM.performList.Remove(choice);
                            }
                            if (BSM.performList[i].target == gameObject)
                            {
                                BSM.performList[i].target = BSM.heroesInBattle[Random.Range(0, BSM.heroesInBattle.Count)];
                            }
                        }
                    }

                    //change color / play death animation

                    //reset heroInput
                    BSM.battleState = BattleStateMachine.PerformAction.CHECKALIVE;
                    alive = false;
                }
                break;
        }
        UpdateHeroBar();
    }


    void UpgradeProgressBar()
    {
        currentCooldown += Time.deltaTime;
        float calculatedCooldown = currentCooldown / maxCooldown;
        progressBar.transform.localScale= new Vector3(Mathf.Clamp01(calculatedCooldown), 1, 1);
        if(currentCooldown >= maxCooldown)
        {
            currentState = TurnState.ADDTOLIST;
        }
    }

    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        switch (action.turnType)
        {
            case HandleTurn.TurnType.ATTACK:
                {
                    enemyToAttack = action.target;
                    // animate enemy near the hero to attack
                    Vector3 heroPosition = new Vector3(enemyToAttack.transform.position.x, enemyToAttack.transform.position.y, enemyToAttack.transform.position.z - 1.5f);
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
                    break;
                }
            case HandleTurn.TurnType.ITEM:
                {
                    //animation

                    //wait
                    yield return new WaitForSeconds(1.0f);

                    HeroStateMachine targetHero = action.target.GetComponent<HeroStateMachine>();
                    action.chosenItem.UseItem(targetHero.hero);

                    break;
                }
        } 

        //remove this performer from the list in BSM
        BSM.performList.RemoveAt(0);

        if (BSM.battleState != BattleStateMachine.PerformAction.WIN && BSM.battleState != BattleStateMachine.PerformAction.LOSE)
        {
            //reset BSM -> wait
            BSM.battleState = BattleStateMachine.PerformAction.WAIT;

            actionStarted = false;

            //reset enemy state
            currentCooldown = 0;
            currentState = TurnState.PROCESSING;
        }
        else
        {
            currentState = TurnState.WAITING;
        }
    }

    private bool MoveTo(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));

    }

    public void TakeDamage(int damage)
    {
        hero.currentHP -= damage;
        if(hero.currentHP <= 0) {
            hero.currentHP = 0;
            currentState = TurnState.DEAD;
        }
    }
    public void DoDamage()
    {
        int calc_damage = hero.currentATK + BSM.performList[0].chosenAttack.attackDamage;
        hero.currentMP -= BSM.performList[0].chosenAttack.attackCost;
        enemyToAttack.GetComponent<EnemyStateMachine>().TakeDamage(calc_damage);
    }

    void CreateHeroBar()
    {
        GameObject panel = Instantiate(heroBarPrefab);
        stats = panel.GetComponent<HeroPanelStats>();
        stats.heroName.text = hero.characterName;
        stats.heroHP.text = "HP: " + hero.currentHP + "/" + hero.baseHP;
        stats.heroMP.text = "MP: " + hero.currentMP + "/" + hero.baseMP;

        progressBar = stats.progressBar;
        panel.transform.SetParent(heroPanelSpacer, false);
    }
    void UpdateHeroBar()
    {
        stats.heroHP.text = "HP: " + (hero.currentHP > 0 ? (hero.currentHP + "/" + hero.baseHP) : "DEAD");
        stats.heroMP.text = "MP: " + hero.currentMP + "/" + hero.baseMP;
    }
}
