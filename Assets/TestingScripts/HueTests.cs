using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HueTests : MonoBehaviour
{

    public List<HeroPawn> heroes;
    public QuestData quest;
    public QuestOutcome results;

    // Start is called before the first frame update
    void Start()
    {
    
        


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            results = GameManager.instance.DetermineBattleOutcome(heroes, quest);
            Debug.Log(results);

        }


    }
}
