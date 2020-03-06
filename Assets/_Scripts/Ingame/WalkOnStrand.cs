using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkOnStrand : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        thisGooballObject = GetComponent<Drag>();
        //find out which ball are we gonna go to
        System.Random rand = new System.Random();
        int gooball = rand.Next(0, 1);
        //get stuff into an array
        Strand strand = currentStrand.GetComponent<Strand>();
        List<GameObject> gballs = new List<GameObject>();
        gballs.Add(strand.connectedBall1);
        gballs.Add(strand.connectedBall2);
        //Debug.Log($"next gooball index is {gooball}, the size of list is {gballs.Count}");
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
    public Drag currentGooballObject;

    public Drag thisGooballObject;
    //Vector3 nextPos;
    public GameObject nextBall;
    public Drag nextGooballObject;

    bool isMoving = false;
    public float speed = 0.03f;
    public GameObject currentStrand;
    public Strand currentStrandObject;

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

                //double distnace = Vector3.Distance(currentStrandObject.connectedBall1.transform.position, currentStrandObject.connectedBall2.transform.position);
                
            }
            //applyMass(); THIS IS FUCKING BROKEN, IT DOESNT WORK, VERY GLITCHY, END MY SUFFERING
        }
    }

    private float massAppliedToPrevious;
    private float currentMassApplied;
    public void applyMass()
    {
        /*
        if (currentGooball != null)
        {
            float percent = Vector3.Distance(transform.position, nextBall.transform.position) / Vector3.Distance(currentGooball.transform.position, nextBall.transform.position);

            massAppliedToPrevious = thisGooballObject.originalMass * percent;
            //know what mass we gon add
            currentMassApplied = thisGooballObject.originalMass - massAppliedToPrevious;


            //make sure its not negative
            if(massAppliedToPrevious < 0)
            {
                massAppliedToPrevious = massAppliedToPrevious * -1;
            }
            if(currentMassApplied < 0)
            {
                currentMassApplied = currentMassApplied * -1;
            }
        }
        */

    }

    public void TowardsWhichGooball()
    {
        if (nextBall != null)
        {
            //nextBall.GetComponent<Drag>().ExtraMass -= GetComponent<Drag>().originalMass;
            currentGooball = nextBall;
            currentGooballObject = currentGooball.GetComponent<Drag>();
        }
        System.Random check = new System.Random();
        int whichGooball = check.Next(0, gooballs.Count);
        //Debug.Log($"next gooball index is {whichGooball}, the size of list is {gooballs.Count}");
        nextBall = gooballs[whichGooball];
        nextGooballObject = nextBall.GetComponent<Drag>();
        //nextPos = nextBall.transform.position;
        isMoving = true;
        if (nextBall != null)
        {
            currentStrand = GameManager.instance.getStrandBetweenBalls(currentGooball, nextBall);
            currentStrandObject = currentStrand.GetComponent<Strand>();
            transform.SetParent(StaticData.balls.transform); //did this shenanigan because the game kept gradually increasing the ball size when going from strand to strand
            transform.localScale = thisGooballObject.originalScale;
            transform.SetParent(currentStrand.transform, true);
            //Debug.Log(currentStrand);
            //nextBall.GetComponent<Drag>().ExtraMass += thisGooballObject.originalMass;
        }
    }

}
