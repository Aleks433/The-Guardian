using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject heroCharacterPrefab;

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
        if(!GameObject.Find("Player"))
        {
            GameObject hero = Instantiate(heroCharacterPrefab, new Vector3(0f,5f,0f), Quaternion.identity);
            hero.name = "Player";
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
