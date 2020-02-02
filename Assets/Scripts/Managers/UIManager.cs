using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[System.Serializable]
public class UIManager : MonoBehaviour
{

    [Header("UI Windows")]
    public GameObject MainMenu;
    public GameObject QuestBoardDisplay;
    public GameObject HeroDisplay;
    public GameObject RepairSettingsDisplay;
    public GameObject QuestOutcomeDisplay;

    private GameObject currentWindow;

    // TODO: This really should be different objects, but it's game jam -- JAM ON, BISHES!
    [Header("Quest Window Items")]
    public TextMeshProUGUI questTitleBox;
    public TextMeshProUGUI questDescriptionBox;
    public TextMeshProUGUI questRewardBox;
    public TextMeshProUGUI questDurabilityBox;

    [Header("Repair Window Items")]
    public TextMeshProUGUI repairTitleBox;
    public TextMeshProUGUI repairDescriptionBox;
    public TextMeshProUGUI repairCostBox;

    void Start()
    {
        UpdateRepairWindow();
        UpdateQuestWindow();
    }

    public IEnumerator OpenWindow( GameObject UIWindowToOpen )
    {
        // TODO: Open the window

        // TEMP: For now, just enable it
        UIWindowToOpen.SetActive(true);

        yield return null;
    }

    public IEnumerator CloseWindow(GameObject UIWindowToOpen)
    {
        // TODO: Close the window

        // TEMP: For now, just enable it
        UIWindowToOpen.SetActive(false);

        yield return null;
    }

    public void UpdateQuestWindow()
    {
        Debug.Log("Updating Quest Log" + GameManager.instance.shop.currentQuestBoardIndex + "/" + GameManager.instance.shop.todaysQuests.Count);

        questTitleBox.text = GameManager.instance.shop.todaysQuests[GameManager.instance.shop.currentQuestBoardIndex].questData.displayName;
        questDescriptionBox.text = GameManager.instance.shop.todaysQuests[GameManager.instance.shop.currentQuestBoardIndex].questData.displayText;
        questRewardBox.text = GameManager.instance.shop.todaysQuests[GameManager.instance.shop.currentQuestBoardIndex].questData.goldReward + " gp";
        questDurabilityBox.text = "Chance to lose " +
                                  GameManager.instance.shop.todaysQuests[GameManager.instance.shop.currentQuestBoardIndex].questData.minDurabilityDamage + " to " +
                                  GameManager.instance.shop.todaysQuests[GameManager.instance.shop.currentQuestBoardIndex].questData.maxDurabilityDamage + "% durability.";
    }


    public void UpdateRepairWindow()
    {

        Debug.Log("Updating Repair Menu" + GameManager.instance.shop.currentRepairIndex + "/" + GameManager.instance.repairData.Count);

        repairTitleBox.text = GameManager.instance.repairData[GameManager.instance.shop.currentRepairIndex].title;
        repairDescriptionBox.text = "+ "+ GameManager.instance.repairData[GameManager.instance.shop.currentRepairIndex].repairArmorAmount + "% Armor Durability \n ";
        repairDescriptionBox.text += "+ " + GameManager.instance.repairData[GameManager.instance.shop.currentRepairIndex].repairWeaponAmount + "% Weapon Durability ";
        repairCostBox.text = GameManager.instance.repairData[GameManager.instance.shop.currentRepairIndex].cost +" gp";
    }




}
