using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Objects")] public List<HeroPawn> heroes;
    public Shop shop;
    public UIManager uiManager;
    public Button nextStepButton;
    public TextMeshProUGUI nextStepButtonText;

    [Header("DataObjects - Loaded at Start")]
    public List<HeroData> heroesData;

    public List<QuestData> questData;
    public List<RepairData> repairData;

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

     public bool isWaitingForButton = false;

    public int currentDay
    {
        get { return maxTurns - turnsRemaining; }
    }

    public float
        minHeroBattleChance =
            0.9f; // Hero party aggregates (atk and def) are reduced by a random amount between min and max

    public float maxHeroBattleChance = 1.0f;

    public float
        minMonsterBattleChance =
            0.5f; // Monster party aggregates (atk and def) are reduced by a random amount between min and max

    public float maxMonsterBattleChance = 1.0f;

    [Header("Repair Data")] public float minStartCondition = 0.35f;
    public float maxStartCondition = 0.45f;

    [Header("Player Data")] public int gold = 0;
    public int startingGold = 0;

    [Header("Prefabs")] public GameObject heroPrefab;



    private void Awake()
    {
        // Setup GameManager Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
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
        // Init Game
        InitializeGame();

        // Create hero pawns
        GameManager.instance.InitializeHeroPawns();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            uiManager.Quit();
        }
    }


    public void InitializeGame()
    {
        // Init Player
        InitializePlayer();

        // Set button to enabled
        nextStepButton.interactable = true;

        // Set button text to START THE DAY
        nextStepButtonText.text = "Start Day";

        // Set button to start the day
        nextStepButton.onClick.RemoveAllListeners();
        nextStepButton.onClick.AddListener(StartDay);
    }

    public void StartDay()
    {
        // TODO: Make sure the "next day" image is off

        // Start the day
        StartCoroutine(DoStartDay());
    }

    public IEnumerator DoStartDay ()
    {
        // Disable the button
        nextStepButton.interactable = false;

        // Set button text to START THE DAY
        nextStepButtonText.text = "... Customers Approaching ...";

        // Move Heroes to start positions
        HeroesToYourStartingPositions();

        // Set current hero to 0
        shop.currentCustomerIndex = 0;

        // Wait a second for them to walk to positions
        yield return new WaitForSeconds(1.5f);

        // Set button text to START THE DAY
        nextStepButtonText.text = "Help Customer";

        // Set button to start the day
        nextStepButton.onClick.RemoveAllListeners();
        nextStepButton.onClick.AddListener(HelpCustomer);

        // enable the button
        nextStepButton.interactable = true;

        yield return null;
    }

    public void HelpCustomer()
    {
        StartCoroutine(DoHelpNextCustomer());
    }

    public IEnumerator DoHelpNextCustomer()
    {
        // disable button
        nextStepButtonText.text = "... Shopping ...";
        nextStepButton.interactable = false;

        // Wait for customer to move to pre-enter door
        yield return StartCoroutine(heroes[shop.currentCustomerIndex].MoveTo(shop.enterShopOutside.position));

        // Wait for customer to move to enter door
        yield return StartCoroutine(heroes[shop.currentCustomerIndex].MoveTo(shop.enterShopInside.position));
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));

        // Wait for customer to move to a random point
        yield return StartCoroutine(heroes[shop.currentCustomerIndex].MoveTo(shop.RandomShopPoints[Random.Range(0, shop.RandomShopPoints.Count)].position));

        // Wait for him to twiddle his thumbs
        yield return StartCoroutine(heroes[shop.currentCustomerIndex].StopMoving());
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));

        // Wait for him to move to the repair station
        yield return StartCoroutine(heroes[shop.currentCustomerIndex].MoveTo(shop.repairDeskPoint.position));

        // BARK repair Bark
        yield return StartCoroutine(heroes[shop.currentCustomerIndex].DoRepairBark());

        // TODO: ACTUALLY DO THE REPAIRS
        nextStepButtonText.text = "... Repairing ...";
        yield return StartCoroutine(shop.DoCustomerRepair());

        yield return new WaitForSeconds(2.0f);

        // Wait for him to move to the quest station
        nextStepButtonText.text = "... Looking for Work ...";
        yield return StartCoroutine(heroes[shop.currentCustomerIndex].MoveTo(shop.questBoardPoint.position));

        // Bark Quest Bark
        yield return StartCoroutine(heroes[shop.currentCustomerIndex].DoQuestBark());

        // TODO: ACTUALLY ASSIGN TO QUEST IF POSSIBLE
        nextStepButtonText.text = "... Choosing a Quest ...";
        yield return StartCoroutine(shop.DoCustomerQuestSignup());

        yield return new WaitForSeconds(2.0f);

        // Wait for him to twiddle his thumbs
        yield return StartCoroutine(heroes[shop.currentCustomerIndex].StopMoving());
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));


        // Leave by heading to the exit
        // Wait for customer to move to pre-exit door
        nextStepButtonText.text = "... Exiting ...";

        yield return StartCoroutine(heroes[shop.currentCustomerIndex].MoveTo(shop.leaveShopInside.position));

        // Wait for customer to move to exit door
        yield return StartCoroutine(heroes[shop.currentCustomerIndex].MoveTo(shop.leaveShopOutside.position));

        // Wait for customer to move to standing point
        yield return StartCoroutine(heroes[shop.currentCustomerIndex].MoveTo(shop.afterShoppingIdlePoints[(shop.afterShoppingIdlePoints.Count -1) - shop.currentCustomerIndex].position));

        // Wait a second for them to walk to positions
        yield return new WaitForSeconds(0.5f);

        // Help the next customer, if available
        if (shop.currentCustomerIndex < heroes.Count - 1)
        {
            shop.currentCustomerIndex++;
            nextStepButtonText.text = "Next Customer";
        }
        else
        {
            // We've helped all the customers
            nextStepButtonText.text = "Close Shoppe";
            nextStepButton.onClick.RemoveAllListeners();
            nextStepButton.onClick.AddListener(RunQuests);
        }

        // enable the button
        nextStepButton.interactable = true;
        yield return null;
    }

    public void RunQuests()
    {
        StartCoroutine(DoRunQuests());
    }


    public void ToggleIsWaitingForButton()
    {
        isWaitingForButton = !isWaitingForButton;
    }

    public void SetIsWaitingForButton(bool value)
    {
        isWaitingForButton = value;
    }

    public void StopWaitingForButton()
    {
        isWaitingForButton = false;
    }

    public IEnumerator DoRunQuests()
    {

        // Turn on the results screen
        uiManager.OpenWindow(uiManager.QuestOutcomeDisplay);
        
        List<QuestObject> questsToRemove = new List<QuestObject>();
        
        // For each quest that were available today
        foreach (QuestObject quest in shop.todaysQuests)
        {
            // If it has people assigned
            if (quest.heroes != null && quest.heroes.Count > 0)
            {

                // Disable the button
                nextStepButton.interactable = false;
                nextStepButtonText.text = "... Fighting! ...";

                isWaitingForButton = true;

                // Calculate the results
                QuestOutcome results = DetermineBattleOutcome(quest.heroes, quest.questData);

                // TODO: coroutine to Update the quest screen to show results over time.
                uiManager.combatQuestTitleBox.text = "Results: "+ quest.questData.displayName;
                uiManager.combatEventsLog.text = "";
                
                foreach (string outcomeEvent in results.events)
                {
                    // Wait some time.
                    yield return new WaitForSeconds(0.5f);

                    // Display the text 
                    uiManager.combatEventsLog.text += outcomeEvent + "\n";
                }

                isWaitingForButton = true;

                nextStepButtonText.text = "Continue";
                nextStepButton.interactable = true;
                nextStepButton.onClick.RemoveAllListeners();
                nextStepButton.onClick.AddListener(StopWaitingForButton);

                // Queue to Remove from today's quests
                questsToRemove.Add(quest);

                // Wait for button
                while (isWaitingForButton)
                {
                    yield return null;
                }
            }
        }

        // Actually handle the removals
        foreach (QuestObject quest in questsToRemove)
        {
            shop.todaysQuests.Remove(quest);
        }

        // ACTUALLY THE END OF THE DAY
        turnsRemaining--;

        if (turnsRemaining > 0)
        {
            // TODO: SHOW THE END OF DAY IMAGE
            // Turn on the results screen
            uiManager.CloseCurrentWindow();
            uiManager.OpenWindow(uiManager.EndOfDayScreen);

            // TODO: Set text to say "DAYS REMAINING..." + days

            // Enable the button
            nextStepButton.interactable = true;
            nextStepButtonText.text = "Start Next Day";

            // Wait for button
            isWaitingForButton = true;
            while (isWaitingForButton)
            {
                yield return null;
            }

            // Move all characters offscreen on right side
            foreach (HeroPawn hero in heroes)
            {
                hero.transform.position = new Vector3(10, 0 , 0);
            }

            // Start the day
            StartDay();
        }


        yield return null;
    }





public float ValueBasedOnDurability(float sourceValue, float durabilityValue)
    {
        // Calculate new value based on how durability and durabilityEffectCurve change the source value
        return sourceValue * durabilityEffectCurve.Evaluate(durabilityValue);
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
        repairData = new List<RepairData>( Resources.LoadAll<RepairData>("RepairTypes"));
    }


    public void HeroesToYourStartingPositions()
    {
        // Move pawn to their appropriate start locations
        for (int i = 0; i < heroes.Count; i++)
        {
            HeroPawn hero = heroes[i];
            hero.StartCoroutine(hero.MoveTo(shop.beforeShoppingIdlePoints[i].position));
        }
    }

    public void InitializeHeroPawns()
    {
        // For each herodata, create an actual pawn
        for (int i=0; i< heroesData.Count; i++)
        {
            HeroData heroData = heroesData[i];
            GameObject tempHero = Instantiate(heroPrefab, new Vector3(10, 0, 0), Quaternion.identity) as GameObject;
            HeroPawn tempHeroPawn = tempHero.GetComponent<HeroPawn>();
            tempHero.name = heroData.displayName;
            Animator tempHeroAnim = tempHero.GetComponent<Animator>();
            tempHeroAnim.runtimeAnimatorController = heroData.animatorController;
            tempHeroPawn.heroData = heroData;
            tempHeroPawn.weaponCondition = Random.Range(minStartCondition, maxStartCondition);
            tempHeroPawn.armorCondition = Random.Range(minStartCondition, maxStartCondition);
            tempHeroPawn.gold = tempHeroPawn.heroData.startingGold;
            tempHeroPawn.relationships = new List<float>();
            for (int relationshipIndex = 0; relationshipIndex < heroesData.Count; relationshipIndex++) tempHeroPawn.relationships.Add(0);
            heroes.Add(tempHeroPawn);

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