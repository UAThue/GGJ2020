using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Hero", menuName = "Data/Hero", order = 1)]
public class Hero : ScriptableObject
{
    public string ID;
    public string displayName;
    [TextArea(3,10)] public string biographyText;
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
}
