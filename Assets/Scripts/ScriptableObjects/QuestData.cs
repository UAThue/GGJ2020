﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Data/Quest", order = 3)]
public class QuestData : ScriptableObject
{
    public string ID;
    public string displayName;
    [TextArea(3,10)] public string displayText;
    public List<MonsterData> monsters;
    public int goldReward;
    public int dayPrereq;
    public int maxAdventurers;
        
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}