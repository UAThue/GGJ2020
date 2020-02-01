using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Monster", menuName = "Data/Monster", order = 2)]
public class Monster : ScriptableObject
{
    public string ID;
    public string displayName;
    public int attack;
    public int defense;
    public int health;
    public List<MonsterTag> modifiers;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
