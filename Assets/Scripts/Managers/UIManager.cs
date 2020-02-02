using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class UIManager : MonoBehaviour
{

    [Header("UI Windows")]
    public GameObject MainMenu;
    public GameObject QuestBoardDisplay;
    public GameObject HeroDisplay;
    public GameObject RepairSettingsDisplay;
    public GameObject QuestOutcomeDisplay;

    private GameObject currentWindow;


    public IEnumerator OpenWindow( GameObject UIWindowToOpen )
    {
        // TODO: Open the window

        // TEMP: For now, just enable it
        UIWindowToOpen.SetActive(true);

        yield return null;
    }

    public IEnumerator CloseWindow(GameObject UIWindowToOpen)
    {
        // TODO: Close the window

        // TEMP: For now, just enable it
        UIWindowToOpen.SetActive(false);

        yield return null;
    }

    public void OnClickNextQuest()
    {
        
    }

    public void OnClickPrevQuest()
    {

    }

    

    
}
