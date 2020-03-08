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
        Debug.LogWarning("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
        initialized = true;
        //find out which ball are we gonna go to
        System.Random rand = new System.Random();
        int gooball = rand.Next(0, 1);
        Debug.LogWarning("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb");
        //get stuff into an array
        Strand strand = currentStrand.transform.parent.gameObject.GetComponent<Strand>();
        Debug.Log(currentStrand, currentStrand);
        Debug.Log(strand, strand);
        
        Debug.LogWarning("cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc");
        //set the next ball and make it go to it
        Debug.Log(gooball);
        Debug.Log(strand, strand);
        Debug.Log(strand.connectedBall1Class);
        Debug.Log(strand.connectedBall2Class);
        nextBall = gooball == 0 ? strand.connectedBall1Class : strand.connectedBall2Class;
        Debug.Log("#############################################################################");
        Debug.Log(nextBall);
        
        isMoving = true;
    }
    
    public List<Gooball> gooballs = new List<Gooball>();



    void GetGooballs(Gooball gooball) {
        gooballs.Clear();
        Debug.Log(gooball);
        Debug.Log(gooball.attachedBalls);
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
        Debug.Log(initialized);
        if(!initialized)
            return;
        
        if (isMoving == false)
        {
            Debug.LogWarning(nextBall);
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
