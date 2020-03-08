using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Walk : MonoBehaviour
{
    [Header("Walking Properties")]
    
    [SerializeField] private Vector3 startingDirection;
    [SerializeField] private float walkSpeed;
    [SerializeField] private Vector2 randomSpeedScale;
    
    [Header("Walk Counters")]
    private int walkCounter = 0;
    private float strandCheckCounter = 0;

    [Header("Components")]
    [SerializeField] private BallSensor ballSensor;
    private Gooball gooball;
    private new Rigidbody2D rigidbody;

    // Runtime AI
    private Vector2 appliedForce;

    private Vector3 dynamicDirection;
    private bool isChangingDirection;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        gooball = GetComponent<Gooball>();
        dynamicDirection = startingDirection;
        // facing left or right?
        if(Random.value > 0.5f)
        {
            dynamicDirection.x = -dynamicDirection.x;
        }
        // random speed 
        walkSpeed *= Mathf.Lerp(randomSpeedScale.x, randomSpeedScale.y, Random.value) / 100;
    }

    private void Update()
    {
        
        // is it a walking goo
        if(!gooball.IsTower && !gooball.IsDragged && ballSensor.isGrounded)
        {
            //random direction change
            if (walkCounter == 50)
            {
                walkCounter = 0;
                System.Random check = new System.Random();
                int probability = check.Next(1, 3);
                if (probability == 1)
                {
                    
                    dynamicDirection = -dynamicDirection;
                    isChangingDirection = true;
                }
            }
            walkCounter += 1;
            
            // jumping on strands
            if (strandCheckCounter >= 0)
            {
                strandCheckCounter = 2f;
                Debug.Log($"Checking for strands lol | { gameObject }", this);
            }
            else
                strandCheckCounter -= Time.deltaTime;

            // is rotating? 
            if (isChangingDirection)
            {
                if (ballSensor.isTouchingWall)
                {
                    appliedForce = dynamicDirection.normalized * walkSpeed;
                    rigidbody.velocity += appliedForce;
                }
                else
                {
                    isChangingDirection = false;
                    WalkUpdate();
                }
            }
            else
                // do this function
                WalkUpdate();
        }
    }


    private void WalkUpdate()
    {
        // touching a wall?
        if (ballSensor.isTouchingWall)
        {
            dynamicDirection = -dynamicDirection;
            isChangingDirection = true;
        }
        else
        {

            appliedForce = dynamicDirection.normalized * walkSpeed;
            rigidbody.velocity += appliedForce;

        }
    }

}
