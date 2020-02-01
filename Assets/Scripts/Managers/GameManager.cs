using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [Header("Objects")]
    public List<HeroPawn> heroes;
    public Shop shop;
    public UIManager uiManager;


    [Header("DataObjects")]
    public List<HeroData> heroesData;
    public List<MonsterData> monsterData;
    public List<QuestData> questData;
    public List<Advantage> advantagesData;
    public List<Disadvantage> disadvantagesData;


    [Header("Game Data")]
    public int maxTurns = 20;
    public int turnsRemaining { get; private set; }

    public AnimationCurve averageRelationshipCurve;
    public AnimationCurve relationshipIncreaseCurve;

    public int maxRelationshipLevel = 5;

    public GameStates gameState;

    

    [Header("Player Data")]
    public int gold;
    
  


    private void Awake()
    {
        // Setup GameManager Sigleton
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        
        // Load Data
        LoadDataFromResources();
        

    }
    
    // Start is called before the first frame update
    void Start()
    {
        turnsRemaining = maxTurns;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DetermineBattleOutcome( List<HeroPawn> heroes, List<MonsterData> monsters)
    {
        // TODO: Takes in party 
        // NOTE: DO NOT CHANGE MONSTERS. Make temp monsters and change the temp ones



    }

    public void LoadDataFromResources()
    {
        // TODO: Load the Data objects from the Scriptable Objects in the Resources folder(s)


    }

}

public enum Stat { Attack, Defense, Health, Gold };
public enum GameStates { Menu, Opening, HelpingCustomer, AllCustomersComplete, Combat, Epilogue    }