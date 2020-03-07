using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Walk : MonoBehaviour
{
    public Vector3 direction;
    public float walkSpeed;
    public Vector2 randomSpeedScale;

    [SerializeField] private Vector3 dynamicDirection;

    [SerializeField] private bool isRotating = false;

    [FormerlySerializedAs("walkcounter")] public int walkCounter = 0;
    private float strandCheckCounter = 0;

    // Scripts and Components
    [FormerlySerializedAs("dragScript")] [SerializeField] private Gooball gooball;
    [FormerlySerializedAs("sensorScript")] [SerializeField] private BallSensor ballSensor;

    private new Rigidbody2D rigidbody;

    private Vector2 appliedForce;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        dynamicDirection = direction;
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
                    isRotating = true;
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
            if (isRotating)
            {
                if (ballSensor.isTouchingWall)
                {
                    appliedForce = dynamicDirection.normalized * walkSpeed;
                    rigidbody.velocity += appliedForce;
                }
                else
                {
                    isRotating = false;
                    WalkUpdate();
                }
            }
            else
                // do this function
                WalkUpdate();
        }
    }


    public void WalkUpdate()
    {
        // touching a wall?
        if (ballSensor.isTouchingWall)
        {
            dynamicDirection = -dynamicDirection;
            isRotating = true;
        }
        else
        {

            appliedForce = dynamicDirection.normalized * walkSpeed;
            rigidbody.velocity += appliedForce;

        }
    }

}
