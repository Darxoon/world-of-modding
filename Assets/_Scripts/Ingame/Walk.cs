using UnityEngine;

public class Walk : MonoBehaviour
{
    [Header("Walking Properties")]
    
    [SerializeField] private Vector3 startingDirection;
    [SerializeField] private float walkSpeed;
    [SerializeField] private Vector2 randomSpeedScale;

    [Header("Jumping Properties")] 
    
    [SerializeField] private bool doesCheckForStrands;

    [Header("Components")]
    
    [SerializeField] private BallSensor ballSensor;

    private new CircleCollider2D collider;
    private Gooball gooball;
    private new Rigidbody2D rigidbody;

    // Runtime AI
    private int walkCounter;
    private float strandCheckCounter;

    private Vector3 dynamicDirection;
    private bool isChangingDirection;
    
    private Vector2 appliedForce;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        gooball = GetComponent<Gooball>();
        collider = GetComponent<CircleCollider2D>();
        
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
        bool isWalking = !gooball.IsTower && !gooball.IsDragged && ballSensor.isGrounded;
        if(isWalking)
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
            if (doesCheckForStrands)
            {
            }

            // direction change
            if (isChangingDirection)
            {
                if (ballSensor.isTouchingWall)
                {
                    appliedForce = dynamicDirection.normalized * walkSpeed;
                    rigidbody.velocity += appliedForce;
                }
                else
                    isChangingDirection = false;
            }
            
            // walk update
            if(!isChangingDirection)
                WalkUpdate();
        }
    }

    private void WalkUpdate()
    {
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
