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


        if (Input.GetKeyDown(KeyCode.A))
        {
            GameManager.instance.uiManager.UpdateCharacterWindow(heroes[0]);
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            StartCoroutine(TestMove());
        }




    }

    public IEnumerator TestMove()
    {
        Debug.Log("Moving");

        yield return heroes[0].MoveTo(new Vector3(0, 0, 0));

        yield return new WaitForSeconds(0.5f);

        yield return heroes[0].MoveTo(new Vector3(3, 2, 0));

        yield return new WaitForSeconds(0.5f);

        yield return heroes[0].MoveTo(new Vector3(-2, -1, 0));

        yield return null;
    }
}
