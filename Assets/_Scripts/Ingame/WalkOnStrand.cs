using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkOnStrand : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public List<GameObject> gooballs = new List<GameObject>();

    void GetGooballs(GameObject gooball) {
        gooballs.Clear();
        foreach (GameObject ball in gooball.GetComponent<Drag>().attachedBalls)
        {
            gooballs.Add(ball);
        }
    }

    Vector3 nextPos;
    public GameObject nextBall;

    bool isMoving = false;
    public float speed = 5f;

    // Update is called once per frame
    void Update()
    {

        if (isMoving == false)
        {
            GetGooballs(nextBall);
            TowardsWhichGooball();
        }

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, nextPos, speed);
            if (transform.position == nextPos) {
                isMoving = false;
            }
                
        }
    }

    public void TowardsWhichGooball()
    {
        System.Random check = new System.Random();
        int whichGooball = check.Next(-1, gooballs.Capacity);
        nextBall = gooballs[whichGooball];
        nextPos = nextBall.transform.position;
        isMoving = true;
    }

}
