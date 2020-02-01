using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPawn : MonoBehaviour
{

    public string ID;
    public string displayName;
    [TextArea(3, 10)] public string biographyText;
    public int attack;
    public int defense;
    public int health;
    [Range(0, 1)] public float weaponCondition;
    [Range(0, 1)] public float armorCondition;
    public List<Advantage> advantages;
    public List<Disadvantage> disadvantages;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MoveTo(Vector3 position)
    {
        // TODO: Move pawn to that position
    }

    public void LoadFromHeroData(string heroFilename)
    {
        HeroData temp = Resources.Load<HeroData>("Heroes/"+ heroFilename);
        if (temp != null)
        {
            LoadFromHeroData(temp);
        }
        else
        {
            Debug.LogError("ERROR - TRIED TO LOAD HERO THAT DOESN'T EXIST + "+heroFilename);
        }
    }

    public void LoadFromHeroData(HeroData heroData)
    {
        ID = heroData.ID;
        displayName = heroData.displayName;
        biographyText = heroData.biographyText;
        attack = heroData.attack;
        defense = heroData.defense;
        health = heroData.health;
        weaponCondition = heroData.weaponCondition;
        armorCondition = heroData.armorCondition;
        foreach (Advantage advantage in heroData.advantages)
        {
            Advantage temp = new Advantage();
            temp.ID = advantage.ID;
            temp.displayName = advantage.displayName;
            temp.against = advantage.against; // TODO: THIS NEEDS BE BE CLONED, NOT JUST REFERENCE, TOO.
            temp.bonusAmount = advantage.bonusAmount;
            temp.statistic = advantage.statistic;

            advantages.Add(temp);
        }

        foreach (Disadvantage disadvantage in heroData.disadvantages)
        {
            Disadvantage temp = new Disadvantage();
            temp.ID = disadvantage.ID;
            temp.displayName = disadvantage.displayName;
            temp.against = disadvantage.against;  // TODO: THIS NEEDS BE BE CLONED, NOT JUST REFERENCE, TOO.

            disadvantages.Add(temp);
        }
    }

}
