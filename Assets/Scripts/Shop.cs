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
    
    [Header("Settings")]
    [Range(0, 2)] public int currentArmorRepairLevel;
    [Range(0, 2)] public int currentWeaponRepairLevel;
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

        // Start today's quests
        todaysQuests = new List<QuestObject>();
        AddTodaysQuests();
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

                Debug.Log("Added quest "+temp.questData.displayName);

            }
            else
            {
                Debug.Log("Quest not today: " + GameManager.instance.questData[i].displayName + " : " + GameManager.instance.questData[i].dayPrereq + "|"+ GameManager.instance.currentDay);
            }
        }
    }


    public void NextQuest()
    {
        currentQuestBoardIndex++;
        if (currentQuestBoardIndex < 0)
        {
            currentQuestBoardIndex = todaysQuests.Count-1;
        }
    }

    public void PreviousQuest()
    {
        currentQuestBoardIndex++;
        if (currentQuestBoardIndex > todaysQuests.Count)
        {
            currentQuestBoardIndex = 0;
        }
    }


    public void startNewDay()
    {
        //TODO:  Move all pawns to start points

        //TODO:  Set store mode to start

        //Set currentCustomerIndex to 0
        currentCustomerIndex = 0;

        //TODO: Add today's quests to list quests -- 
        AddTodaysQuests();


        //TODO: Activate NextCustomer button


    }

    


    public IEnumerator DoNextCustomer()
    {
        // Increment current customer
        currentCustomerIndex++;

        // TODO: Tell that customer to start walking to the RepairStation


        // TODO: Deactivate the Next Customer button

        // End
        yield return null;

    }

    public IEnumerator DoCustomerRepair()
    {
        // Shrink index to within number of costs or amounts, whichever is lower
        Mathf.Clamp(currentWeaponRepairLevel, 0, GameManager.instance.repairCosts.Count - 1);
        Mathf.Clamp(currentWeaponRepairLevel, 0, GameManager.instance.repairAmounts.Count - 1);

        //If they have enough coin for weapon, update
        if (GameManager.instance.heroes[currentCustomerIndex].gold > GameManager.instance.repairCosts[currentWeaponRepairLevel]) {
            GameManager.instance.heroes[currentCustomerIndex].gold -= GameManager.instance.repairCosts[currentWeaponRepairLevel];
            GameManager.instance.heroes[currentCustomerIndex].weaponCondition += GameManager.instance.repairAmounts[currentWeaponRepairLevel];

            // TODO: Show floating message for "Weapon Repaired"
        }

        //If they have enough coin for armor, update
        if (GameManager.instance.heroes[currentCustomerIndex].gold > GameManager.instance.repairCosts[currentArmorRepairLevel])
        {
            GameManager.instance.heroes[currentCustomerIndex].gold -= GameManager.instance.repairCosts[currentArmorRepairLevel];
            GameManager.instance.heroes[currentCustomerIndex].armorCondition += GameManager.instance.repairAmounts[currentArmorRepairLevel];

            // TODO: Show floating message for "Armor Repaired"
        }

        // TODO: Send the customer to the QuestSignup Board

        // End
        yield return null;
    }




    public IEnumerator DoCustomerQuestSignup()
    {
        // TODO: If the quest is not full
        if (false)
        {
            // TODO: Sign the hero up for the quest

        }

        // TODO: Now that they have a quest, IF there are more customers activate the Next Customer button.
        if (GameManager.instance.heroes.Count - 1 > currentCustomerIndex)
        {
            // TODO: Activate the Next Customer button
        }
        // TODO: Else, no more customers. Activate End Day button instead.
        else
        {
            // TODO: Hide NextCustomer Button
            // TODO: Show and Activate End Day button

        }

        // TODO: Start customer moving to the exit location


        // End
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
