using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterZone : MonoBehaviour
{
    //Prefabs of this zone's enemies
    public List<GameObject> enemyPrefabs;
    public int difficulty;
    private GameManager gameManager;
    public float encounterDistance;
    public float encounterDistanceTrigger;
    public float encounterChancePercentage = 30f;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(encounterDistance >= encounterDistanceTrigger)
        {
            encounterDistance = 0;
            if (Random.value <= encounterChancePercentage / 100.0f)
            {
                Debug.Log("starting battle");
                //Starting battle
                gameManager.StartBattle(enemyPrefabs, Random.Range(1, difficulty));
            }
        }
        
    }
}
