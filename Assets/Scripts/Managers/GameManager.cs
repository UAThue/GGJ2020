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


    [Header("DataObjects - Loaded at Start")]
    public List<HeroData> heroesData;
    public List<MonsterData> monsterData;
    public List<QuestData> questData;
    public List<Advantage> advantagesData;
    public List<Disadvantage> disadvantagesData;


    [Header("Game Data")]
    public GameStates gameState;
    public int maxTurns = 20;
    public int turnsRemaining { get; private set; }

    public AnimationCurve averageRelationshipCurve;
    public AnimationCurve relationshipIncreaseCurve;

    public float maxRelationshipLevel = 5;
    public float maxStartingRelationshipLevel = 2;

    [Header("Repair Data")]
    public int minRepairCost = 0;

    public int medRepairCost = 10;

    public int maxRepairCost = 100;

    public int minRepairAmount = 10;

    public int medRepairAmount = 25;

    public int maxRepairAmount = 50;


    [Header("Player Data")]
    public int gold;
    public int startingGold;
 


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
        // TODO: Takes in party, determines battle outcomes
        // NOTE: DO NOT CHANGE MONSTERS. Make temp monsters and change the temp ones



    }

    public void LoadDataFromResources()
    {
        // TODO: Load the Data objects from the Scriptable Objects in the Resources folder(s)


    }

    public void InitializeRelationships()
    {
        // TODO: Set all relationships to value between 0 and maxRelationshipLevel
        // IMPORTANT: Make sure that arrays in each hero stays parallel to Heroes array -- so, must be done AFTER we load all the heroes

    }






}

public enum Stat { Attack, Defense, Health, Gold };
public enum GameStates { Menu, Opening, HelpingCustomer, AllCustomersComplete, Combat, Epilogue    }