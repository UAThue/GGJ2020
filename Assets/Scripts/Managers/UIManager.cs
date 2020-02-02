using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


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

    [Header("Repair Window Items")]
    public TextMeshProUGUI characterNameBox;
    public TextMeshProUGUI characterDescriptionBox;
    public TextMeshProUGUI characterAttackBox;
    public TextMeshProUGUI characterDefenseBox;
    public TextMeshProUGUI characterHealthBox;
    public TextMeshProUGUI characterGoldBox;
    public Image characterImage;
    public List<RelationshipBox> relationshipImages;



    void Start()
    {
        UpdateRepairWindow();
        UpdateQuestWindow();
    }

    public IEnumerator DoOpenWindow( GameObject UIWindowToOpen )
    {
        // TODO: Open the window

        // TEMP: For now, just enable it
        UIWindowToOpen.SetActive(true);

        yield return null;
    }

    public IEnumerator DoCloseWindow(GameObject UIWindowToOpen)
    {
        // TODO: Close the window

        // TEMP: For now, just enable it
        UIWindowToOpen.SetActive(false);

        yield return null;
    }

    public void SetCurrentWindow(GameObject window)
    {
        currentWindow = window;
    }

    public void CloseCurrentWindow ()
    {
        StartCoroutine(DoCloseWindow(currentWindow));
    }

    public void OpenWindow(GameObject window)
    {
        CloseCurrentWindow();
        currentWindow = window;
        StartCoroutine(DoOpenWindow(window));
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

    public void Quit()
    {
        Application.Quit();
    }

    public void UpdateCharacterWindow( HeroPawn hero )
    {
        characterNameBox.text = hero.heroData.displayName;
        characterDescriptionBox.text = hero.heroData.biographyText;
        characterAttackBox.text = hero.heroData.attack + " ATK\n("+ToPercent(hero.weaponCondition)+")";
        characterDefenseBox.text = hero.heroData.attack + " DEF\n(" + ToPercent(hero.armorCondition) + ")";
        characterGoldBox.text = hero.gold + " gp";
        characterHealthBox.text = hero.heroData.health + " hp";
        characterImage.sprite = hero.heroData.displaySprite;

        int currentRelationshipBox = 0;

        // For each of the heroes in the game
        for (int otherHeroIndex = 0; otherHeroIndex < GameManager.instance.heroes.Count; otherHeroIndex++)
        {
            // If this relationship is with yourself, skip this hero
            if (hero == GameManager.instance.heroes[otherHeroIndex])
            {
                Debug.Log("Skipping relationship with self!");
                continue;
            }
            // Otherwise
            else
            {
                // Enable the relationship box
                relationshipImages[currentRelationshipBox].characterImage.gameObject.SetActive(true);

                //Set the image
                relationshipImages[currentRelationshipBox].characterImage.sprite = GameManager.instance.heroes[otherHeroIndex].heroData.displaySprite;

                // Set the relationship value
                relationshipImages[currentRelationshipBox].RelationshipImage.fillAmount = hero.relationships[otherHeroIndex] / GameManager.instance.maxRelationshipLevel;

                // Advance to next relationshipbox
                currentRelationshipBox++;
                
                // If we are out of boxes, we have to quit
                if (currentRelationshipBox >= relationshipImages.Count)
                {
                    Debug.Log("More heroes than boxes.");
                    break;
                }
            }
        }

        // If there are leftover relationship boxes, disable them
        for (int i = currentRelationshipBox; i < relationshipImages.Count; i++)
        {
            Debug.Log("Turning off leftover box #" + i);
            relationshipImages[i].characterImage.gameObject.SetActive(false);
        }

    }

    public string ToPercent(float input)
    {
        float output = input * 1000;
        output = Mathf.Floor(output);
        output /= 10;
        return output.ToString() + "%";
    }

}

[System.Serializable]
public class RelationshipBox
{
    public Image characterImage;
    public Image RelationshipImage;
}

