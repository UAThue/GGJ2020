using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Objects")] public List<HeroPawn> heroes;
    public Shop shop;
    public UIManager uiManager;

    [Header("DataObjects - Loaded at Start")]
    public List<HeroData> heroesData;

    public List<QuestData> questData;

    [Header("Game Data")] public GameStates gameState;
    public int maxTurns = 20;
    public int turnsRemaining = 0;

    public AnimationCurve averageRelationshipCurve;
    public AnimationCurve relationshipIncreaseCurve;
    public AnimationCurve durabilityEffectCurve;
    public AnimationCurve bonusEffectByRelationshipAverageCurve;

    public float maxRelationshipLevel = 5;
    public float minStartingRelationshipLevel = 0;
    public float maxStartingRelationshipLevel = 2;
    public int currentDay
    {
        get { return maxTurns - turnsRemaining; }
    }

    public float
        minHeroBattleChance = 0.9f; // Hero party aggregates (atk and def) are reduced by a random amount between min and max

    public float maxHeroBattleChance = 1.0f;

    public float
        minMonsterBattleChance = 0.5f; // Monster party aggregates (atk and def) are reduced by a random amount between min and max

    public float maxMonsterBattleChance = 1.0f;

    [Header("Repair Data")] 
    public List<int> repairCosts = new List<int>();
    public List<int> repairAmounts = new List<int>();

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

        // TODO: Find better way to do this than to have to set it here, too
        turnsRemaining = maxTurns;

        // Load Data
        LoadDataFromResources();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        // Init Player
        InitializePlayer();

        // Create hero pawns
        GameManager.instance.InitializeHeroPawns();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public float ValueBasedOnDurability(float sourceValue, float durabilityValue)
    {
        // Calculate new value based on how durability and durabilityEffectCurve change the source value
        return durabilityEffectCurve.Evaluate(sourceValue);
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
            heroAggregateAttack += ValueBasedOnDurability(hero.heroData.attack, hero.weaponCondition);
            heroAggregateDefense += ValueBasedOnDurability(hero.heroData.defense, hero.armorCondition);
            heroAggregateHealth += hero.heroData.health;


            // Since durability hits EVERY fight, We can reduce durability here, and save us another iteration through heroes
            hero.weaponCondition -= Random.Range(quest.minDurabilityDamage, quest.maxDurabilityDamage);
            hero.armorCondition -= Random.Range(quest.minDurabilityDamage, quest.maxDurabilityDamage);
        }

        // Add advantages -- be sure to check for monster changes
        foreach (HeroPawn hero in heroes)
        {
            float bonusModifier = 1.0f; // Start at full bonus

            // iterate through disadvantages
            bool foundDisadvantage = false;
            foreach (Disadvantage disadvantage in hero.heroData.disadvantages)
            {
                // iterate through monsters
                foreach (MonsterData monster in quest.monsters)
                {
                    // Iterate through each monsters set of types 
                    foreach (MonsterTag tag in monster.modifiers)
                    {
                        // if we find one of those types in our disadvantage, note it and quit
                        if (disadvantage.against.Contains(tag))
                        {
                            // TODO: Add event about the disadvantage
                            Debug.Log(hero.heroData.displayName + " is terrified of the "+ tag.displayName + " " + monster.displayName );

                            foundDisadvantage = true;
                            break;
                        }
                    }

                    // If I found a disadvantage, I don't need to look at other monsters
                    if (foundDisadvantage) break;
                }

                // If I found a disadvantage, I don't need to look at other disadvantages
                if (foundDisadvantage) break;
            }

            // If we found a disadvantage
            if (foundDisadvantage)
            {
                // create a bonus modifier based on average relationships
                bonusModifier = bonusEffectByRelationshipAverageCurve.Evaluate(AveragePartyRelationship(heroes) / maxRelationshipLevel);
            }
            // Otherwise, our bonus modifier is still 1.0
            else
            {
                bonusModifier = 1.0f;
            }

            // iterate through advantages
            foreach (Advantage advantage in hero.heroData.advantages)
            {
                //Apply "advantage.amount * modifier* and add to aggregates
                if (advantage.statistic == Stat.Attack)
                {
                    heroAggregateAttack += advantage.bonusAmount * bonusModifier;
                    Debug.Log("The power of friendship adds "+ (advantage.bonusAmount * bonusModifier) + " | "+ bonusModifier + " / " + advantage.bonusAmount + " attack.");
                }
                if (advantage.statistic == Stat.Health)
                {
                    heroAggregateHealth += advantage.bonusAmount * bonusModifier;
                    Debug.Log("The power of friendship adds " + (advantage.bonusAmount * bonusModifier) + " | " + bonusModifier + " / " + advantage.bonusAmount + " health.");
                }
                if (advantage.statistic == Stat.Defense)
                {
                    heroAggregateDefense += advantage.bonusAmount * bonusModifier;
                    Debug.Log("The power of friendship adds " + (advantage.bonusAmount * bonusModifier) + " | " + bonusModifier + " / " + advantage.bonusAmount + " defense.");
                }
            }
        }

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

            Debug.Log("The party does " + damageDone + " damage. The monster party has a total of "+monsterAggregateHealth+" health left!");


            // If monster dead, break.
            if (monsterAggregateHealth <= 0)
            {
                break;
            }


            // Damage players
            damageDone = Mathf.Max(1, (monsterAggregateAttack * Random.Range(minMonsterBattleChance, maxMonsterBattleChance)) - (heroAggregateDefense * Random.Range(minHeroBattleChance, maxHeroBattleChance)));
            heroAggregateHealth -= damageDone;

            Debug.Log("The monster party does " + damageDone + " damage. The party has a total of " + heroAggregateHealth + " health left!");


            // If players dead, break
            if (heroAggregateHealth <= 0)
            {
                break;
            }
        }

        // If monsters are dead, success.
        if (monsterAggregateHealth <= 0)
        {

            // Add gold to all players
            foreach (HeroPawn hero in heroes)
            {
                // Divide gold among heroes, leftovers are destroyed into the ether
                hero.gold += (int)(quest.goldReward / heroes.Count);
            }

            // Add success events to the event list
            // TODO: Don't do one per round, that's too many. Maybe one per person.
            for (int i = 0; i <numberOfRounds; i++)
            {
                // Pick a random hero
                HeroPawn chosenOne = heroes[Random.Range(0, heroes.Count)];

                // TODO: Pick a random string from the hero's success events
                string eventString = chosenOne.name + " stabs the vile beast in the face.";

                // TODO: Add the finalized (no variables) version of that string to the list of outcome events.
                results.events.Add(eventString);
            }

            // TODO: Update relationships


        }
        // Else failed mission
        else
        {
            // No change to relationships
            // No change to gold

            // Add fail events to the event list
            // TODO: Don't do one per round, that's too many. Maybe one per person.
            for (int i = 0; i < numberOfRounds; i++)
            {
                // Pick a random hero
                HeroPawn chosenOne = heroes[Random.Range(0, heroes.Count)];

                // TODO: Pick a random string from the hero's success events
                string eventString = chosenOne.name + " stumbles.";

                // TODO: Add the finalized (no variables) version of that string to the list of outcome events.
                results.events.Add(eventString);
            }
        }

        return results;
    }

    public float AveragePartyRelationship( List<HeroPawn> party)
    {
        // If we are solo, full relationship
        if (party.Count < 2)
        {
            return 1.0f;
        }


        // Average party relationships (don't include self to self checks) 
        // Total all relationships, divide by number of relationships
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
        // Load the Data objects from the Scriptable Objects in the Resources folder(s)
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
            tempHero.name = heroData.displayName;
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


[System.Serializable]
public class QuestOutcome
{
    public List<string> events;
    public bool success;
    public int gold;
    public float relationshipGain;

    public QuestOutcome()
    {
        events = new List<string>();
    }
}

public enum Stat { Attack, Defense, Health, Gold };
public enum GameStates { Menu, Opening, HelpingCustomer, AllCustomersComplete, Combat, Epilogue }