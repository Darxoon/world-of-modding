using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkOnStrand : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //find out which ball are we gonna go to
        System.Random rand = new System.Random();
        int gooball = rand.Next(0, 1);
        //get stuff into an array
        Strand strand = currentStrand.GetComponent<Strand>();
        List<GameObject> gballs = new List<GameObject>();
        gballs.Add(strand.connectedBall1);
        gballs.Add(strand.connectedBall2);
        Debug.Log($"next gooball index is {gooball}, the size of list is {gballs.Count}");
        //set the next ball and make it go to it
        nextBall = gballs[gooball];
        //nextPos = nextBall.transform.position;
        isMoving = true;
    }

    public List<GameObject> gooballs = new List<GameObject>();

    void GetGooballs(GameObject gooball) {
        gooballs.Clear();
        foreach (GameObject ball in gooball.GetComponent<Drag>().attachedBalls)
        {
            gooballs.Add(ball);
        }
    }

    public GameObject currentGooball;

    Vector3 nextPos;
    public GameObject nextBall;

    bool isMoving = false;
    public float speed = 0.03f;
    public GameObject currentStrand;


    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localScale = transform.localScale;
        if (isMoving == false)
        {
            GetGooballs(nextBall);
            TowardsWhichGooball();
        }

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, nextBall.transform.position, speed);
            if (transform.position == nextBall.transform.position) {
                isMoving = false;
            }
                
        }
    }

    public void TowardsWhichGooball()
    {
        if (nextBall != null)
        {
            currentGooball = nextBall;
        }
        System.Random check = new System.Random();
        int whichGooball = check.Next(0, gooballs.Count);
        //Debug.Log($"next gooball index is {whichGooball}, the size of list is {gooballs.Count}");
        nextBall = gooballs[whichGooball];
        //nextPos = nextBall.transform.position;
        isMoving = true;
        if (nextBall != null)
        {
            currentStrand = GameManager.instance.getStrandBetweenBalls(currentGooball, nextBall);
            transform.SetParent(StaticData.balls.transform); //did this shenanigan because the game kept gradually increasing the ball size when going from strand to strand
            transform.localScale = GetComponent<Drag>().originalScale;
            transform.SetParent(currentStrand.transform, true);
            Debug.Log(currentStrand);   
            
        }
    }

}
