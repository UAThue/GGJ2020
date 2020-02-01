using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [Header("Points of Interest")]
    public List<Transform> beforeShoppingIdlePoints;
    public List<Transform> afterShoppingIdlePoints;
    public Transform repairDeskPoint;
    public Transform questBoardPoint;
    [Header("Settings")]
    [Range(0,2)] public int currentArmorRepairLevel;
    [Range(0, 2)] public int currentWeaponRepairLevel;
    public int currentQuestBoardIndex;


    [Header("Working Data")]
    public int currentCustomerIndex = 0;

    public List<QuestData> todaysQuests = new List<QuestData>();

    
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
   
    }

    public void NextQuest()
    {

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

        //TODO: Get list of today's quests -- need to be quests where dayPrereq is < (totalDays - daysRemaining)
        todaysQuests = new List<QuestData>();



    }

    public void UpdateGear( HeroPawn hero )
    {
        // TODO: This should update the hero's gear based on the store settings --- if they have the gold
    }



}
