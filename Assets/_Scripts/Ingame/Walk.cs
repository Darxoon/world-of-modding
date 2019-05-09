using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : MonoBehaviour
{
    public Vector3 direction;
    public float walkSpeed;
    public Vector2 randomSpeedScale;

    [SerializeField] private Vector3 dynamicDirection;

    [SerializeField] private bool isRotating = false;

    public int walkcounter = 0;

    // Scripts and Components
    [SerializeField] private Drag dragScript;
    [SerializeField] private BallSensor sensorScript;

    private Rigidbody2D rigid;

    private Vector2 appliedForce;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
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
        
        // is it a lying unattached goo
        if(!dragScript.IsTower && !dragScript.IsDragged && sensorScript.isGrounded)
        {
            //random direction change
            if (walkcounter == 50)
            {
                walkcounter = 0;
                System.Random check = new System.Random();
                int probability = check.Next(1, 3);
                if (probability == 1)
                {
                    
                    dynamicDirection = -dynamicDirection;
                    isRotating = true;
                }
            }
            walkcounter = walkcounter + 1;
            //

            // is rotating? 
            if (isRotating)
            {
                if (sensorScript.isTouchingWall)
                {
                    appliedForce = dynamicDirection.normalized * walkSpeed;
                    rigid.velocity += appliedForce;
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
        if (sensorScript.isTouchingWall)
        {
            dynamicDirection = -dynamicDirection;
            isRotating = true;
        }
        else
        {

            appliedForce = dynamicDirection.normalized * walkSpeed;
            rigid.velocity += appliedForce;

        }
    }

}
