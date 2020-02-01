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
    public List<QuestData> questData;

    [Header("Game Data")]
    public GameStates gameState;
    public int maxTurns = 20;
    public int turnsRemaining { get; private set; }

    public AnimationCurve averageRelationshipCurve;
    public AnimationCurve relationshipIncreaseCurve;
    public AnimationCurve durabilityEffectCurve;

    public float maxRelationshipLevel = 5;
    public float minStartingRelationshipLevel = 0;
    public float maxStartingRelationshipLevel = 2;

    public float minHeroBattleChance = 0.9f; // Hero party aggregates (atk and def) are reduced by a random amount between min and max
    public float maxHeroBattleChance = 1.0f;
    public float minMonsterBattleChance = 0.5f; // Monster party aggregates (atk and def) are reduced by a random amount between min and max
    public float maxMonsterBattleChance = 1.0f;

    [Header("Repair Data")]
    public int minRepairCost = 0;

    public int medRepairCost = 10;

    public int maxRepairCost = 100;

    public int minRepairAmount = 10;

    public int medRepairAmount = 25;

    public int maxRepairAmount = 50;

    public float minStartCondition = 0.35f;
    public float maxStartCondition = 0.45f;

    [Header("Player Data")]
    public int gold = 0;
    public int startingGold = 0;

    [Header("Prefabs")]
    public GameObject heroPrefab;



    private void Awake()
    {
        // Setup GameManager Singleton
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


    public float valueBasedOnDurability(float sourceValue, float durabilityValue)
    {
        // TODO: Calculate new value based on how durability and durabilityEffectCurve change the source value


        return sourceValue;
    }

    public QuestOutcome DetermineBattleOutcome( List<HeroPawn> heroes, QuestData quest)
    {
        // Takes in party and quest, determines battle outcomes
        QuestOutcome results = new QuestOutcome();


        // Aggregate total party
        float heroAggregateHealth = 0;
        float heroAggregateAttack = 0;
        float heroAggregateDefense = 0;

        foreach (HeroPawn hero in heroes)
        {
            heroAggregateAttack += hero.heroData.attack;
            heroAggregateDefense += hero.heroData.defense;
            heroAggregateHealth += hero.heroData.health;


            // Since durability hits EVERY fight, We can reduce durability here, and save us another iteration through heroes
            hero.weaponCondition -= Random.Range(quest.minDurabilityDamage, quest.maxDurabilityDamage);
            hero.armorCondition -= Random.Range(quest.minDurabilityDamage, quest.maxDurabilityDamage);
        }

        // TODO: Add bonuses -- be sure to check for monster changes




        // NOTE: DO NOT CHANGE MONSTERS. Make temp monsters and change the temp ones
        float monsterAggregateHealth = 0;
        float monsterAggregateAttack = 0;
        float monsterAggregateDefense = 0;
        foreach (MonsterData monster in quest.monsters)
        {
            monsterAggregateAttack += monster.attack;
            monsterAggregateDefense += monster.defense;
            monsterAggregateHealth += monster.health;
        }


        // Track number of rounds
        int numberOfRounds = 0;

        // As long as the monsters are alive
        while (monsterAggregateHealth > 0)
        {
            // Increase round count
            numberOfRounds++;

            // Damage monsters
            float damageDone = Mathf.Max(1, (heroAggregateAttack * Random.Range(minHeroBattleChance, maxHeroBattleChance)) - (monsterAggregateDefense * Random.Range(minMonsterBattleChance, maxMonsterBattleChance)));
            monsterAggregateHealth -= damageDone;

            // If monster dead, break.
            if (monsterAggregateHealth <= 0)
            {
                break;
            }


            // Damage players
            damageDone = Mathf.Max(1, (monsterAggregateAttack * Random.Range(minMonsterBattleChance, maxMonsterBattleChance)) - (heroAggregateDefense * Random.Range(minHeroBattleChance, maxHeroBattleChance)));
            monsterAggregateHealth -= damageDone;

            // If players dead, break
            if (heroAggregateHealth <= 0)
            {
                break;
            }
        }

        // If monsters are dead, success.
        if (monsterAggregateHealth > 0)
        {

            // Add gold to all players
            foreach (HeroPawn hero in heroes)
            {
                // Divide gold among heroes, leftovers are destroyed into the ether
                hero.gold += (int)(quest.goldReward / heroes.Count);
            }

            // Add success events to the event list
            for (int i = 0; i <numberOfRounds; i++)
            {

                // TODO: Pick a random hero
                // TODO:Pick a random string from the hero's success events
                // TODO: Add the finalized (no variables) version of that string to the list of outcome events.
            }

            // TODO: Update relationships


        }
        // Else failed mission
        else
        {
            // No change to relationships
            // No change to gold

            // Add fail events to the event list
            for (int i = 0; i < numberOfRounds; i++)
            {
                // TODO: Pick a random hero
                // TODO: Pick a random string from the hero's success events
                // TODO: Add the finalized (no variables) version of that string to the list of outcome events.
            }
        }

        return results;
    }

    public float AveragePartyRelationship( List<HeroPawn> party)
    {

        // Average party relationships (don't include self to self checks) 
        // Total all relationships, divide by number of relationships
        // 
        float totalValue = 0;
        float numberOfRelationshipsTested = 0;

        for (int indexOne = 0; indexOne < party.Count; indexOne++)
        {
            for (int indexTwo = 0; indexTwo < party.Count; indexTwo++)
            {
                // Only include if not comparing to ourselves
                if (indexOne != indexTwo)
                {
                    totalValue += heroes[indexOne].relationships[indexTwo];
                    numberOfRelationshipsTested++;
                }
            }
        }

        float average = totalValue / numberOfRelationshipsTested;

        // Find percent of total that our average is at
        float percentAverage = average / maxRelationshipLevel;

        // Find where average lies on curve
        float percentOnCurve = averageRelationshipCurve.Evaluate(percentAverage);

        // Find the value based on that curve and max level
        return (percentOnCurve * maxRelationshipLevel);
    }


    public void LoadDataFromResources()
    {
        // TODO: Load the Data objects from the Scriptable Objects in the Resources folder(s)
        heroesData = new List<HeroData>(Resources.LoadAll<HeroData>("Heroes/"));
        questData = new List<QuestData>(Resources.LoadAll<QuestData>("Quests/"));
    }

    public void InitializeHeroPawns()
    {
        // For each herodata, create an actual pawn
        foreach (HeroData heroData in heroesData)
        {
            GameObject tempHero = Instantiate(heroPrefab) as GameObject;
            HeroPawn tempHeroPawn = tempHero.GetComponent<HeroPawn>();
            tempHeroPawn.heroData = heroData;
            tempHeroPawn.weaponCondition = Random.Range(minStartCondition, maxStartCondition);
            tempHeroPawn.armorCondition = Random.Range(minStartCondition, maxStartCondition);
            tempHeroPawn.gold = tempHeroPawn.heroData.startingGold;
            tempHeroPawn.relationships = new List<float>();
            for (int i = 0; i<heroesData.Count; i++) tempHeroPawn.relationships.Add(0);
            heroes.Add(tempHeroPawn);

            // TODO: Move pawn to their appropriate start locations

        }

        // Now that heroes exist, we can give them relationships
        InitializeRelationships();
    }


    public void InitializeRelationships()
    {
        // TODO: Set all relationships to value between 0 and maxRelationshipLevel
        // IMPORTANT: Make sure that arrays in each hero stays parallel to Heroes array -- so, must be done AFTER we load all the heroes
        for (int heroOneIndex = 0; heroOneIndex < heroes.Count; heroOneIndex++)
        {
            for (int heroTwoIndex = heroOneIndex; heroTwoIndex < heroes.Count; heroTwoIndex++)
            {
                // set ourselves to max
                if (heroOneIndex == heroTwoIndex)
                {
                    heroes[heroOneIndex].relationships[heroTwoIndex] = maxRelationshipLevel;
                    heroes[heroTwoIndex].relationships[heroOneIndex] = maxRelationshipLevel;
                }
                // Set our relationship to others to random value
                else
                {
                    float tempValue = Random.Range(minStartingRelationshipLevel, maxStartingRelationshipLevel);
                    heroes[heroOneIndex].relationships[heroTwoIndex] = tempValue;
                    heroes[heroTwoIndex].relationships[heroOneIndex] = tempValue;
                }
            }
        }
    }


    public void InitializePlayer()
    {
        turnsRemaining = maxTurns;
        gold = startingGold;
    }




}


public class QuestOutcome
{
    public List<string> events;
    public bool success;
    public int gold;
    public float relationshipGain;
}

public enum Stat { Attack, Defense, Health, Gold };
public enum GameStates { Menu, Opening, HelpingCustomer, AllCustomersComplete, Combat, Epilogue }