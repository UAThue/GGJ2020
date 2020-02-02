using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RepairMenuItem", menuName = "Data/RepairMenuItem", order = 15)]
public class RepairData : ScriptableObject
{
    public string title;
    public int cost;
    public int repairWeaponAmount;
    public int repairArmorAmount;
}
