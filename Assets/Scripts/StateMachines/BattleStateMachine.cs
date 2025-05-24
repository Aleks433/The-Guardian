using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleStateMachine : MonoBehaviour
{
    public enum PerformAction
    {
        INIT,
        WAIT,
        TAKEACTION,
        PERFORMACTION,
        CHECKALIVE,
        WIN,
        LOSE
    }
    public PerformAction battleState;

    public enum HeroGUI
    {
        ACTIVATE,
        WAITING,
        DONE
    }
    public HeroGUI heroInput;

    public List<HandleTurn> performList = new List<HandleTurn>();
    public List<GameObject> heroesInBattle = new List<GameObject> ();
    public List<GameObject> enemiesInBattle = new List<GameObject> ();

    public List<GameObject> heroesToManage = new List<GameObject>();
    private HandleTurn heroChoice;

    public GameObject enemyButtonPrefab;
    public Transform spacer;


    //panels
    public GameObject actionButtonPrefab;
    public GameObject actionPanel;
    public Transform actionSpacer;
    public GameObject magicButtonPrefab;
    public GameObject magicPanel;
    public Transform magicSpacer;
    public GameObject enemySelectPanel;
    private List<GameObject> actionButtons = new List<GameObject>();

    //enemy buttons
    private List<GameObject> enemyButtons = new List<GameObject>();


    public List<GameObject> allyPrefabs = new List<GameObject>();
    public List<Transform> allySpawns = new List<Transform>();
    public List <GameObject> enemyPrefabs = new List<GameObject>();
    public List <Transform> enemySpawns = new List<Transform>();

    private void Awake()
    {
        battleState = PerformAction.INIT;
    }
    void Start()
    {
    }
    void InitBattle()
    {
        //wait for allies/enemies to init
        if(allyPrefabs.Count == 0 || enemyPrefabs.Count == 0)
        {
            return;
        }

        //Instantiate allies
        for(int i = 0;i < allyPrefabs.Count;i++)
        {
            GameObject ally = Instantiate(allyPrefabs[i]);
            HeroStateMachine allyStateMachine = ally.GetComponent<HeroStateMachine>();
            ally.transform.position = allySpawns[i].transform.position;
            ally.gameObject.name = allyStateMachine.hero.characterName;
            heroesInBattle.Add(ally);
        }

        //Instantiate enemies
        for(int i=0;i < enemyPrefabs.Count;i++)
        {
            GameObject enemy = Instantiate(enemyPrefabs[i]);
            EnemyStateMachine enemyStateMachine = enemy.GetComponent<EnemyStateMachine>();
            enemy.transform.position = enemySpawns[i].transform.position;
            enemy.name = enemyStateMachine.enemy.characterName;
            enemiesInBattle.Add(enemy);
        }

        battleState = PerformAction.WAIT;
        heroInput = HeroGUI.ACTIVATE;

        actionPanel.SetActive(false);
        enemySelectPanel.SetActive(false);
        magicPanel.SetActive(false);

        EnemyButtons();
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        switch (battleState)
        {
            case PerformAction.INIT:
                InitBattle();
                return;
            case PerformAction.WAIT:
                 if(performList.Count > 0)
                 {
                     battleState = PerformAction.TAKEACTION;
                 }
                 break;
            case PerformAction.TAKEACTION:
                GameObject performer = GameObject.Find(performList[0].attacker);
                if (performList[0].type == "Enemy")
                {
                   EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();
                   bool heroAlive = false;
                   for(int i = 0; i < heroesInBattle.Count;i++)
                   {
                       if (performList[0].attackerTarget == heroesInBattle[i])
                       {
                           ESM.heroToAttack = performList[0].attackerTarget;
                           ESM.currentState = EnemyStateMachine.TurnState.ACTION;
                           heroAlive = true;
                           break;
                       }
                   }
                   if(!heroAlive)
                   {
                       performList[0].attackerTarget = heroesInBattle[Random.Range(0, heroesInBattle.Count)];
                       ESM.heroToAttack = performList[0].attackerTarget;
                       ESM.currentState = EnemyStateMachine.TurnState.ACTION;

                   }

                } else if (performList[0].type == "Hero")
                {
                   HeroStateMachine HSM = performer.GetComponent<HeroStateMachine>();
                   HSM.enemyToAttack = performList[0].attackerTarget;
                   HSM.currentState = HeroStateMachine.TurnState.ACTION;

                }
                battleState = PerformAction.PERFORMACTION;
                break;
            case PerformAction.PERFORMACTION:
                //idle
                break;
            case PerformAction.CHECKALIVE:
                if(heroesInBattle.Count < 1)
                {
                    //lose game
                    battleState = PerformAction.LOSE;
                }
                else if (enemiesInBattle.Count < 1)
                {
                    //win game
                    battleState = PerformAction.WIN;
                }
                else 
                { 
                    ClearActionPanel();
                    heroInput = HeroGUI.ACTIVATE;
                }
                break;
            case PerformAction.LOSE:
                Debug.Log("Player lost");
                break;
            case PerformAction.WIN:
                Debug.Log("Player won");
                for (int i = 0; i < heroesInBattle.Count; i++)
                {
                    heroesInBattle[i].GetComponent<HeroStateMachine>().currentState = HeroStateMachine.TurnState.WAITING;
                }
                GameObject.Find("GameManager").GetComponent<GameManager>().state = GameManager.GameState.BATTLE_OVER;
                break;
        }
        switch(heroInput)
        {
            case HeroGUI.ACTIVATE:
                if(heroesToManage.Count > 0)
                {
                    heroesToManage[0].transform.Find("Selector").gameObject.SetActive(true);
                    heroChoice = new HandleTurn();


                    actionPanel.SetActive(true);

                    //populate action panel
                    CreateActionButtons();

                    heroInput = HeroGUI.WAITING;

                }
                break;
            case HeroGUI.WAITING:
                break;
            case HeroGUI.DONE:
                HeroInputDone();
                break;

        }
    }

    public void CollectActions(HandleTurn input) 
    {
        performList.Add(input);
    }
    
    public void EnemyButtons() 
    {
        //Cleanup
        foreach(GameObject enemyButton in enemyButtons)
        {
            Destroy(enemyButton);
        }
        enemyButtons.Clear();


        //Create buttons
        foreach(GameObject enemy in enemiesInBattle)
        {
            GameObject newButton = Instantiate(enemyButtonPrefab) as GameObject;

            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();
            EnemyStateMachine currentEnemy = enemy.GetComponent<EnemyStateMachine>();

            TextMeshProUGUI buttonText = newButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            buttonText.text = currentEnemy.name;

            button.enemy = enemy;

            newButton.transform.SetParent(spacer, false);
            enemyButtons.Add(newButton);
        }
    }

    public void Input1()
    {
        heroChoice.attacker = heroesToManage[0].name;
        heroChoice.attackerGameObject = heroesToManage[0];
        heroChoice.type = "Hero";
        heroChoice.chosenAttack = heroesToManage[0].GetComponent<HeroStateMachine>().hero.attacks[0];

        actionPanel.SetActive(false);
        enemySelectPanel.SetActive(true);
    }

    public void Input2(GameObject chosenEnemy)
    {
        heroChoice.attackerTarget = chosenEnemy;
        heroInput = HeroGUI.DONE;
    }

    void HeroInputDone()
    {
        performList.Add(heroChoice);
        ClearActionPanel();
        heroesToManage[0].transform.Find("Selector").gameObject.SetActive(false);
        heroesToManage.RemoveAt(0);
        heroInput = HeroGUI.ACTIVATE;
    }

    void ClearActionPanel()
    {
        enemySelectPanel.SetActive(false);
        actionPanel.SetActive(false);
        magicPanel.SetActive(false);
        //Clean action panel
        foreach(GameObject actionButton in actionButtons)
        {
            Destroy(actionButton);
        }
        actionButtons.Clear();

         
    }

    void CreateActionButtons()
    {
        {
            GameObject attackButton = Instantiate(actionButtonPrefab);

            TextMeshProUGUI attackButtonText = attackButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            attackButtonText.text = "Attack";
            attackButton.GetComponent<Button>().onClick.AddListener(() => Input1());

            attackButton.transform.SetParent(actionSpacer, false);
            actionButtons.Add(attackButton);
        }


        {
            GameObject magicButton = Instantiate(actionButtonPrefab);

            TextMeshProUGUI magicButtonText = magicButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            magicButtonText.text = "Magic";
            magicButton.GetComponent<Button>().onClick.AddListener(() => Input3());

            magicButton.transform.SetParent(actionSpacer, false);
            actionButtons.Add(magicButton);

            List<BaseAttack> playerMagicAttacks = heroesToManage[0].GetComponent<HeroStateMachine>().hero.magicAttacks;
            if (playerMagicAttacks.Count > 0)
            {
                foreach(BaseAttack magicAttack in playerMagicAttacks)
                {
                    GameObject magicAttackButton =  Instantiate(magicButtonPrefab);
                    TextMeshProUGUI magicAttackText = magicAttackButton.transform.Find("Text").GetComponent <TextMeshProUGUI>();
                    magicAttackText.text = magicAttack.name;

                    AttackButton ATB = magicAttackButton.GetComponent<AttackButton>();
                    ATB.magicAttackToPeform = magicAttack;
                    magicAttackButton.transform.SetParent(magicSpacer, false);
                    actionButtons.Add(magicAttackButton);
                }
            } else 
            {
                magicButton.GetComponent<Button>().interactable = false;
            }
        }
    }

    //chosen magic attack
    public void Input4(BaseAttack magicAttack)
    {
        heroChoice.attacker = heroesToManage[0].name;
        heroChoice.attackerGameObject = heroesToManage[0];
        heroChoice.type = "Hero";

        heroChoice.chosenAttack = magicAttack;
        magicPanel.SetActive(false);
        enemySelectPanel.SetActive(true);
    }
    public void Input3()
    {
        actionPanel.SetActive(false);
        magicPanel.SetActive(true);
    }
}
