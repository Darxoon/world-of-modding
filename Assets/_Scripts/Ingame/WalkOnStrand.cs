using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkOnStrand : MonoBehaviour
{
    private bool initialized = false;
    void Start()
    {
        currentStrandObject = currentStrand.GetComponent<Strand>();
        thisGooballObject = GetComponent<Gooball>();
        
    }

    public void Initialize()
    {
        if(initialized)
            return;
        thisGooballObject = GetComponent<Gooball>();
        thisGooballObject.isOnStrand = true;
        initialized = true;
        //find out which ball are we gonna go to
        System.Random rand = new System.Random();
        int gooball = rand.Next(0, 1);
        //get stuff into an array
        Strand strand = currentStrand.GetComponent<Strand>();
        
        //set the next ball and make it go to it
        nextBall = gooball == 0 ? strand.connectedBall1Class : strand.connectedBall2Class;
        
        isMoving = true;
    }
    
    public List<Gooball> gooballs = new List<Gooball>();



    void GetGooballs(Gooball gooball) {
        gooballs.Clear();
        foreach (GameObject ball in gooball.attachedBalls)
        {
            gooballs.Add(ball.GetComponent<Gooball>());
        }
    }

    public GameObject currentGooball;
    public Gooball currentGooballObject;

    public Gooball thisGooballObject;
    //Vector3 nextPos;
    public Gooball nextBall;
    public Gooball nextGooballObject;

    bool isMoving = false;
    public float speed = 0.03f;
    public GameObject currentStrand;
    public Strand currentStrandObject;

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!initialized)
            return;
        
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
            currentStrandObject.ExitStrand(transform);
            currentGooball = nextBall.gameObject;
            currentGooballObject = currentGooball.GetComponent<Gooball>();
        }
        System.Random check = new System.Random();
        int whichGooball = check.Next(0, gooballs.Count);
        nextBall = gooballs[whichGooball];
        nextGooballObject = nextBall.GetComponent<Gooball>();
        isMoving = true;
        if (nextBall != null)
        {
            currentStrand = GameManager.instance.getStrandBetweenBalls(currentGooball, nextBall.gameObject).gameObject;
            currentStrandObject = currentStrand.GetComponent<Strand>();
            currentStrandObject.EnterStrand(transform);
            transform.SetParent(currentStrand.transform, true);
        }
    }

}
