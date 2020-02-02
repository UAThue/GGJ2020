using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Hero", menuName = "Data/Hero", order = 1)]
public class HeroData : ScriptableObject
{
    public string ID;
    public string displayName;
    [TextArea(3,10)] public string biographyText;
    public int attack;
    public int defense;
    public int health;
    public int startingGold;
    public List<Advantage> advantages;
    public List<Disadvantage> disadvantages;
    public List<nearOtherBark> idleBarks;
	public List<string> soloIdleBarks;
	public List<string> advantageQuestBarks;
    public List<string> disadvantageQuestBarks;
	public List<string> successQuestBarks;
	public List<string> failQuestBarks;
    public List<string> teamMissionFailedBarks;
    public List<string> soloMissionFailedBarks;
}

[System.Serializable]
public class nearOtherBark
{
    public string bark;
    public float minRelationshipLevel;
    public float maxRelationshipLevel;
}
