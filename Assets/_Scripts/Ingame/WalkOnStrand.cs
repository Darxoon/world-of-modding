using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WalkOnStrand : MonoBehaviour
{
    private bool initialized;

    [FormerlySerializedAs("currentGooballObject")] public Gooball currentGooball;

    [FormerlySerializedAs("thisGooballObject")] public Gooball gooball;
    
    public Gooball nextBall;

    private bool isMoving;
    public float speed = 0.03f;
    public Strand currentStrand;
    [FormerlySerializedAs("currentStrandObject")] public Strand strand;


    public void Initialize()
    {
        if(initialized)
            return;
        gooball = GetComponent<Gooball>();
        gooball.isOnStrand = true;
        initialized = true;
        //find out which ball are we gonna go to
        System.Random rand = new System.Random();
        int next = rand.Next(0, 1);
        //get stuff into an array
        strand = currentStrand.GetComponent<Strand>();
        
        //set the next ball and make it go to it
        nextBall = next == 0 ? strand.connectedBall1Class : strand.connectedBall2Class;
        
        isMoving = true;
    }


    private void FixedUpdate()
    {
        if(!initialized)
            return;
        
        if (isMoving == false)
        {
            TowardsWhichGooball(nextBall.attachedBalls);
        }

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, nextBall.transform.position, speed);
            if (transform.position == nextBall.transform.position) {
                isMoving = false;                
            }
        }
    }

    private void TowardsWhichGooball(IReadOnlyList<Gooball> gooballs)
    {
        if (nextBall)
        {
            strand.ExitStrand(transform);
            currentGooball = nextBall;
        }
        System.Random check = new System.Random();
        int whichGooball = check.Next(0, gooballs.Count);
        nextBall = gooballs[whichGooball];
        isMoving = true;
        if (nextBall)
        {
            currentStrand = GameManager.GetStrandBetweenBalls(currentGooball, nextBall);
            strand = currentStrand.GetComponent<Strand>();
            strand.EnterStrand(transform);
            transform.SetParent(currentStrand.transform, true);
        }
    }

}
