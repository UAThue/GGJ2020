using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeroPawn : MonoBehaviour
{
    public HeroData heroData; // NOTE: NEVER CHANGE THIS DATA
    [Range(0, 1)] public float weaponCondition = 1;
    [Range(0, 1)] public float armorCondition = 1;
    public int gold;
    public List<float> relationships; // NOTE: Parallel array to GameManager.Heroes to hold the relationships
    public float moveSpeed = 1;

    [SerializeField] private TextMeshProUGUI barkTextBox;
    [SerializeField] private GameObject barkBox;

    private Animator anim;
    private SpriteRenderer sr;
    private float lastKnownXPosition;
    private ClickableObject clickHandler;


    //   is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        clickHandler = GetComponent<ClickableObject>();
    }

    void Start()
    {
        if (clickHandler != null)
        {
            //Debug.Log("has clickhandler");

            clickHandler.OnClick.AddListener(OpenCharacterWindowAction);
        }

        // Hide the barkbox
        barkBox.SetActive(false);
    }

    public void OpenCharacterWindowAction()
    {
        Debug.Log("here");
        GameManager.instance.uiManager.OpenWindow(GameManager.instance.uiManager.HeroDisplay);
        GameManager.instance.uiManager.UpdateCharacterWindow(this);
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator DoRepairBark()
    {
        Debug.Log("REPAIRING!");
        yield return null;
    }

    public IEnumerator DoQuestBark()
    {
        Debug.Log("I EITHER LIKE OR DISLIKE THIS QUEST!");
        yield return null;
    }

    public IEnumerator DoBark(string bark)
    {
        barkTextBox.text = bark;
        barkBox.SetActive(true);

        yield return new WaitForSeconds(3.5f);

        barkBox.SetActive(false);
        yield return null;
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
