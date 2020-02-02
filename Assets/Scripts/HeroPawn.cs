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
    public float moveSpeed = 1;

    private Animator anim;
    private SpriteRenderer sr;
    private float lastKnownXPosition;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator MoveTo(Vector3 position)
    {

        // Until we reach position
        while (transform.position != position) {
            // Save their last known x
            lastKnownXPosition = transform.position.x;

            // Move pawn to that position
            transform.position = Vector2.MoveTowards(transform.position, position, moveSpeed * Time.deltaTime);
            anim.SetBool("isMoving", true);

            if (transform.position.x > lastKnownXPosition) {
                sr.flipX = false;
            }
            else {
                sr.flipX = true;
            }

            // Do next frame draw
            yield return null;
        }
        yield return StartCoroutine(StopMoving());
    }

    public IEnumerator StopMoving ()
    {
        anim.SetBool("isMoving", false);
        yield return null;
    }
}
