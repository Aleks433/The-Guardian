using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject worldCharacterPrefab;
    public List<GameObject> playerParty = new List<GameObject>();

    public string battleScene;
    public string lastScene;

    public GameObject player;
    public Vector3 playerPosition = Vector3.zero;
    public Quaternion playerRotation = Quaternion.identity;

    
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
        SpawnPlayer();
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
    }
    void SpawnPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log(player);
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
        state = GameState.BATTLE_ONGOING;

        SceneManager.activeSceneChanged -= OnBattleSceneEnter;
    }
    void OnWorldSceneEnter(Scene scene, LoadSceneMode mode)
    {
        state = GameState.WORLD;
        player.SetActive(true);

        SceneManager.sceneLoaded -= OnWorldSceneEnter;
    }
}
