using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Disadvantage", menuName = "Data/Disadvantage", order = 5)]
public class Disadvantage : ScriptableObject
{
    public string ID;
    public string displayName;
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
