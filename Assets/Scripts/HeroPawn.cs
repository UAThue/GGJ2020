using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPawn : MonoBehaviour
{
    public HeroData heroData; // NOTE: NEVER CHANGE THIS DATA
    [Range(0, 1)] public float weaponCondition = 1;
    [Range(0, 1)] public float armorCondition = 1;
    public int gold;
    public List<float> relationships; // NOTE: Parallel array to GameManager.Heroes to hold the relationships


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MoveTo(Vector3 position)
    {
        // TODO: Move pawn to that position
    }
}
