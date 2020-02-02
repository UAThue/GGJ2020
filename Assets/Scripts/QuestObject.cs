using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestObject 
{
    public QuestData questData;
    public List<HeroPawn> heroes;

    public QuestObject ()
    {
        heroes = new List<HeroPawn>();
    }



}
