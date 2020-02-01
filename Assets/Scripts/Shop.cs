using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [Header("Points of Interest")]
    public List<Transform> beforeShoppingIdlePoints;
    public List<Transform> afterShoppingIdlePoints;
    public Transform repairDeskPoint;
    public Transform questBoardPoint;

    
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize()
    {
        GameManager.instance.InitializeHeroPawns();
    }
}
