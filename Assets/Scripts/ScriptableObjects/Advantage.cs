using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Advantage", menuName = "Data/Advantage", order = 4)]
public class Advantage : ScriptableObject
{
    public string ID;
    public string displayName;
    public float bonusAmount;
    public Stat statistic;
    public List<MonsterTag> against;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
