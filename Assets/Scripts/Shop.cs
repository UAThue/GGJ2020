using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [Header("Points of Interest")]
    public List<Transform> beforeShoppingIdlePoints;
    public List<Transform> afterShoppingIdlePoints;
    public Transform repairDeskPoint;
    public Transform questBoardPoint;
    public List<Transform> RandomShopPoints;
    public Transform enterShopOutside;
    public Transform enterShopInside;
    public Transform leaveShopOutside;
    public Transform leaveShopInside;


    [Header("Settings")]
    public int currentRepairIndex;
    public int currentQuestBoardIndex;
    
    [Header("Working Data")]
    public int currentCustomerIndex = 0;

    public List<QuestObject> todaysQuests;

    
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize()
    {
        // Start quest board at 0
        currentQuestBoardIndex = 0;
        currentRepairIndex = 0;

        // Start today's quests
        todaysQuests = new List<QuestObject>();
        StartCoroutine(DoStartNewTurn());
    }


    public void AddTodaysQuests()
    {
        // Iterate through all questData
        for (int i = 0; i < GameManager.instance.questData.Count; i++)
        {
            // If the quest is set for today (dayReq is maxTurns - turns remaining)
            if (GameManager.instance.questData[i].dayPrereq == GameManager.instance.currentDay)
            {
                // Create a new quest object for it
                QuestObject temp = new QuestObject();
                temp.questData = GameManager.instance.questData[i];
                // Make sure it has slots for heroes
                temp.heroes = new List<HeroPawn>();
                // Add it to our list of available quests
                todaysQuests.Add(temp);

                //Debug.Log("Added quest "+temp.questData.displayName);
            }
            else
            {
                //Debug.Log("Quest not today: " + GameManager.instance.questData[i].displayName + " : " + GameManager.instance.questData[i].dayPrereq + "|"+ GameManager.instance.currentDay);
            }
        }
    }

    public void NextRepair()
    {
        currentRepairIndex--;
        if (currentRepairIndex < 0)
        {
            currentRepairIndex = GameManager.instance.repairData.Count - 1;
        }

        // Update the UI
        GameManager.instance.uiManager.UpdateRepairWindow();
    }

    public void PreviousRepair()
    {
        currentRepairIndex++;
        if (currentRepairIndex >= GameManager.instance.repairData.Count)
        {
            currentRepairIndex = 0;
        }

        // Update the UI
        GameManager.instance.uiManager.UpdateRepairWindow();
    }


    public void NextQuest()
    {
        currentQuestBoardIndex--;
        if (currentQuestBoardIndex < 0)
        {
            currentQuestBoardIndex = todaysQuests.Count-1;
        }

        // Update the UI
        GameManager.instance.uiManager.UpdateQuestWindow();
    }

    public void PreviousQuest()
    {
        currentQuestBoardIndex++;
        if (currentQuestBoardIndex >= todaysQuests.Count)
        {
            currentQuestBoardIndex = 0;
        }

        // Update the UI
        GameManager.instance.uiManager.UpdateQuestWindow();
    }


    public IEnumerator DoStartNewTurn()
    {
        //TODO:  Move all pawns to start points

        //TODO:  Show some kind of "A new day..." or "Day X of Y"
        // yield return StartCoroutine(ShowDayIntroText);

        //TODO: Add today's quests to list quests -- 
        AddTodaysQuests();

        //Set currentCustomerIndex to 0
        currentCustomerIndex = 0;

        // TODO: Wait????
        yield return new WaitForSeconds(0.0f);

        //TODO: Activate NextCustomer button

        yield return null;
    }

    


    public IEnumerator DoNextCustomer()
    {
        // TODO: Deactivate the Next Customer button



        // TODO: Tell that customer to start walking to the RepairStation



        // End
        yield return null;

    }

    public IEnumerator DoCustomerRepair()
    {
        // Shrink index to within number of costs or amounts, whichever is lower
        Mathf.Clamp(currentRepairIndex, 0, GameManager.instance.repairData.Count - 1);
        Mathf.Clamp(currentRepairIndex, 0, GameManager.instance.repairData.Count - 1);

        //If they have enough coin for weapon, update
        if (GameManager.instance.heroes[currentCustomerIndex].gold > GameManager.instance.repairData[currentRepairIndex].cost) {
            GameManager.instance.heroes[currentCustomerIndex].gold -= GameManager.instance.repairData[currentRepairIndex].cost;
            GameManager.instance.heroes[currentCustomerIndex].weaponCondition += GameManager.instance.repairData[currentRepairIndex].repairWeaponAmount;

            // TODO: Show floating message for "Weapon Repaired"
        }

        //If they have enough coin for armor, update
        if (GameManager.instance.heroes[currentCustomerIndex].gold > GameManager.instance.repairData[currentRepairIndex].cost)
        {
            GameManager.instance.heroes[currentCustomerIndex].gold -= GameManager.instance.repairData[currentRepairIndex].cost;
            GameManager.instance.heroes[currentCustomerIndex].armorCondition += GameManager.instance.repairData[currentRepairIndex].repairArmorAmount;

            // TODO: Show floating message for "Armor Repaired"
        }
        // End
        yield return null;
    }




    public IEnumerator DoCustomerQuestSignup()
    {
        // If the quest is not full
        if (todaysQuests[currentQuestBoardIndex].heroes.Count < todaysQuests[currentQuestBoardIndex].questData.maxAdventurers) 
        {
            // Sign the hero up for the quest
            todaysQuests[currentQuestBoardIndex].heroes.Add(GameManager.instance.heroes[currentCustomerIndex]);

            Debug.Log(GameManager.instance.heroes[currentCustomerIndex].heroData.displayName + " signed up for quest "+ todaysQuests[currentQuestBoardIndex].questData.displayName);

            // TODO: Quest Bark
        }
        else
        {
            Debug.Log(GameManager.instance.heroes[currentCustomerIndex].heroData.displayName + " failed to join quest " + todaysQuests[currentQuestBoardIndex].questData.displayName);
            // TODO: Bark that quest is too full
        }

        // End
        yield return null;

    }


    public IEnumerator DoEndCustomer()
    {
        // Increment current customer
        currentCustomerIndex++;

        // TODO: Now that they have finished, IF there are more customers activate the Next Customer button.
        if (GameManager.instance.heroes.Count >= currentCustomerIndex)
        {
            // TODO: Activate the Next Customer button


        }
        // TODO: Else, no more customers. Activate End Day button instead.
        else
        {
            // TODO: Hide NextCustomer Button
            // TODO: Show and Activate End Day button

        }


        yield return null;
    }


    public IEnumerator DoEndDay()
    {
        //TODO: Deactivate end day button

        //TODO: Iterate through quests
        for (int i = 0; i < todaysQuests.Count; i++)
        {
            // TODO: IF there are heroes on the quest
            
            //TODO: Show Quest Page
            //TODO: Run quest "cutscene"
            //TODO: Remove from Today's Quests
        }

        // End
        yield return null;
    }

}
