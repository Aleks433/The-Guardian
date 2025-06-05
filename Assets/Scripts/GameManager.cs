using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

[System.Serializable]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject worldCharacterPrefab;
    public List<GameObject> playerParty = new List<GameObject>();

    public string battleScene;
    public string lastScene;

    //Player;
    public GameObject player;
    public Vector3 playerPosition = Vector3.zero;
    public Quaternion playerRotation = Quaternion.identity;

    //Status panel
    public GameObject statusPanel;
    public Transform heroButtonContainer;
    public Transform useableItemContainer;
    public List<GameObject> heroButtons = new List<GameObject>();
    public List<GameObject> useableItemButtons = new List<GameObject>();
    public GameObject heroButtonPrefab;
    public GameObject useableItemPrefab;
    bool statusPanelEnabled;



    //Dialogue
    DialogueManager dialogueManager;


    //Items
    public List<BaseUseableItem> items = new List<BaseUseableItem>();

    
    public enum GameState
    {
        WORLD,
        BATTLE_INIT,
        BATTLE_ONGOING,
        BATTLE_OVER
    }
    public GameState state;

    public List<GameObject> enemiesToBattle = new List<GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        state = GameState.WORLD;
        dialogueManager = GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<DialogueManager>();
        SpawnPlayer();
        StatusButtons();
/*        Dialogue testDialogue = new Dialogue();
        testDialogue.dialogueTitle = "test";
        testDialogue.dialogueContent.Enqueue("This is a test dialogue to show if the dialogue system works correctly. Press continue to advance the dialogue");
        testDialogue.dialogueContent.Enqueue("This is the second sentence of the dialogue");
        testDialogue.dialogueSpeed = 0.1f;
        dialogueManager.AddDialogue(testDialogue);
*/    
    }

    // Update is called once per frame
    void Update()
    {
        switch (state) {
            case GameState.WORLD:
                break;
            case GameState.BATTLE_ONGOING:
                //idle
                break;
            case GameState.BATTLE_OVER:
                //Save current hero party
                //SaveHeroes();

                SceneManager.sceneLoaded += OnWorldSceneEnter;
                SceneManager.LoadScene(lastScene);
                break;
        } 
        if(Input.GetKeyDown(KeyCode.LeftControl)) 
        {
            statusPanelEnabled = !statusPanelEnabled;
        }
        if(!statusPanelEnabled)
        {
            statusPanel.SetActive(false);
            player.GetComponent<PlayerMovement>().enabled = true;
        }
        else
        {
            statusPanel.SetActive(true);
            player.GetComponent<PlayerMovement>().enabled = false;
        }
    }

    void SpawnPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            player = Instantiate(worldCharacterPrefab);
            player.name = "Player";
            DontDestroyOnLoad(player);
        }
    }

    void SaveHeroes() 
    {
        playerParty.Clear();
        playerParty.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
        foreach(GameObject hero in playerParty)
        {
            HeroStateMachine HSM = hero.GetComponent<HeroStateMachine>();
            if(HSM.hero.baseHP == 0)
            {

            }
        }
    }

    //Start battle with a list of enemy prefabs and how many enemies
    public void StartBattle(List<GameObject> enemies, int count)
    {
        enemiesToBattle.Clear();
        for (int i = 0; i < count; i++)
        {
            enemiesToBattle.Add(enemies[Random.Range(0, enemies.Count)]);
        }
        player.SetActive(false);
        lastScene = SceneManager.GetActiveScene().name;
        SceneManager.activeSceneChanged += OnBattleSceneEnter;
        SceneManager.LoadScene(battleScene);
    }

    //Scene switching logic
    void OnBattleSceneEnter(Scene worldScene, Scene BattleScene)
    {
        BattleStateMachine BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        BSM.allyPrefabs = playerParty;
        BSM.enemyPrefabs = enemiesToBattle;
        BSM.items = items;
        state = GameState.BATTLE_ONGOING;

        SceneManager.activeSceneChanged -= OnBattleSceneEnter;
    }
    void OnWorldSceneEnter(Scene scene, LoadSceneMode mode)
    {
        state = GameState.WORLD;
        player.SetActive(true);

        SceneManager.sceneLoaded -= OnWorldSceneEnter;
    }
    void StatusButtons()
    {
        foreach(GameObject heroSelectButton in heroButtons)
        {
            Destroy(heroSelectButton);
        }
        heroButtons.Clear();

        foreach(GameObject itemSelectButton in useableItemButtons)
        {
            Destroy (itemSelectButton);
        }
        useableItemButtons.Clear();

        {
            foreach (GameObject heroObject in playerParty)
            {
                BaseHero hero = heroObject.GetComponent<HeroStateMachine>().hero;
                GameObject button = Instantiate(heroButtonPrefab);
                SelectHeroButton heroButton = button.GetComponent<SelectHeroButton>();
                heroButton.hero = hero;
                heroButton.heroNameText.text = hero.characterName;
                heroButton.heroHPText.text = "HP: " + hero.currentHP + "/" + hero.baseHP;
                heroButton.heroMPText.text = "MP: " + hero.currentMP + "/" + hero.baseMP;
                heroButton.itemSelector = statusPanel.GetComponent<ItemSelector>();
                button.transform.SetParent(heroButtonContainer, false);
                heroButtons.Add(button);
                statusPanel.GetComponent<ItemSelector>().heroButtons.Add(heroButton);
            }
        }
        {
            foreach(BaseUseableItem item in items)
            {
                GameObject button = Instantiate(useableItemPrefab);
                SelectItemButton itemButton = button.GetComponentInParent<SelectItemButton>();
                itemButton.item = item;
                itemButton.itemText.text = item.name;
                itemButton.itemSelector = statusPanel.GetComponent<ItemSelector>();
                button.transform.SetParent(useableItemContainer, false);
                useableItemButtons.Add(button);
                statusPanel.GetComponent<ItemSelector>().itemButtons.Add(itemButton);
            }
        }
    }

    public void UseItemOnHero(BaseUseableItem item, BaseHero hero)
    {
        item.UseItem(hero);
        items.Remove(item);
        StatusButtons();

    }
}
