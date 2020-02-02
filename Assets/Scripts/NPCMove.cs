using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMove : MonoBehaviour
{
    public Transform NPC;

    public float moveSpeed;
    // this is for later if we need it
    public Transform idlePointOne;
    // final stopping point
    public Transform idlePointTwo;

    // sets array for the random move points
    public Transform[] movePoints;
    private int randomSpot;

    // tracks the number of points moved to
    public int numOfPoints;

    private float waitTime;
    public float startWaitTime;

    private bool lastPosition = false;

    private float lastKnownXPosition;
    // Start is called before the first frame update

    public Animator animator;
    void Start()
    {
        waitTime = startWaitTime;
        randomSpot = Random.Range(0, movePoints.Length);
        lastKnownXPosition = NPC.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        // if the npc is not in the final position
        if (!lastPosition)
        {
            // move to a random point
            transform.position = Vector2.MoveTowards(transform.position, movePoints[randomSpot].position, moveSpeed * Time.deltaTime);
            // animator.SetBool("isMoving", true);

            //check if the npc has moved in the positive x Pos
            if (NPC.position.x > lastKnownXPosition)
            {
                animator.SetBool("isFacingLeft", false);
                animator.SetBool("isMoving", true);
            }
            //check f the npc has moved in the negative x Pos 
            else if (NPC.position.x < lastKnownXPosition)
            {
                animator.SetBool("isFacingLeft", true);
                animator.SetBool("isMoving", false);
            }
            //if he has not moved in the x pos
            else
            {

            }
        }

        // if the npc is in the final location
        if (NPC.position.x == idlePointTwo.position.x)
        {
            // stop walking
            animator.SetBool("isMoving", false);
            animator.SetBool("isFacingLeft", false);
        }
        // if the distance between npc current position and the move point is less than 0.2 or it is at the last position
        if (Vector2.Distance(transform.position, movePoints[randomSpot].position) < 0.2f || lastPosition)
        {
            if (waitTime <= 0)
            {
                // if NPC moved less than twice
                if (numOfPoints < 2)
                {
                    numOfPoints++;
                    randomSpot = Random.Range(0, movePoints.Length);
                    waitTime = startWaitTime;
                }
                // if npc moved twice or more
                else if (numOfPoints >= 2)
                {
                    // if the number of points moved to is greater-than or equal to 2, move to the last position
                    transform.position = Vector2.MoveTowards(transform.position, idlePointTwo.position, moveSpeed * Time.deltaTime);
                    lastPosition = true;

                    if (lastPosition == true)
                    {
                        if (NPC.position.x > lastKnownXPosition)
                        {
                            animator.SetBool("isFacingLeft", false);
                            animator.SetBool("isMoving", true);
                        }
                        //check f the npc has moved in the negative x Pos 
                        else if (NPC.position.x < lastKnownXPosition)
                        {
                            animator.SetBool("isFacingLeft", true);
                            animator.SetBool("isMoving", false);
                        }
                        //if he has not moved in the x pos
                        else
                        {

                        }
                    }
                }
            }
            else
            {
                // set wait timer and stop walking
                waitTime -= Time.deltaTime;
                animator.SetBool("isMoving", false);
                animator.SetBool("isFacingLeft", false);

            }
        }
        lastKnownXPosition = NPC.position.x;
    }
}

