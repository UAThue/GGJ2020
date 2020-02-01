using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterTag", menuName = "Data/MonsterTag", order = 15)]
public class MonsterTag : ScriptableObject
{
    public string id;
    public string displayName;
}
