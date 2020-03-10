using UnityEngine;

public class Walk : MonoBehaviour
{
    [Header("Walking Properties")]
    
    [SerializeField] private Vector3 startingDirection;
    [SerializeField] private float walkSpeed;
    [SerializeField] private Vector2 randomSpeedScale;

    [Header("Jumping Properties")] 
    
    [SerializeField] private bool doesCheckForStrands;
    [SerializeField] private float raycastLength = 1f;
    [SerializeField] private float jumpAnimationSpeed = 1f;

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
    
    // Animation
    private bool inJumpingAnimation = false;
    private float jumpAnimationCounter = 0f;
    private Vector3 originalJumpPosition;
    private Vector3 newJumpPosition;
    
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
        if (inJumpingAnimation)
        {
            jumpAnimationCounter += Time.deltaTime;
            float scaledCounter = Mathf.Sqrt(jumpAnimationCounter * jumpAnimationSpeed);
            if (scaledCounter >= 1f)
            {
                GetComponent<WalkOnStrand>().enabled = true;
                inJumpingAnimation = false;
            }
            else
            {
                transform.position = Vector3.Lerp(originalJumpPosition, newJumpPosition, scaledCounter);
            }
            return;
        }
        
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
            if (doesCheckForStrands && !gooball.isOnStrand)
            {
                Vector3 position = transform.position;
                
                Debug.DrawRay(position, rigidbody.velocity.normalized * raycastLength,       Color.magenta);
                
                RaycastHit2D raycastHit = Physics2D.Raycast(position, rigidbody.velocity.normalized, raycastLength,
                    LayerMask.GetMask("Strands"));
                
                if (raycastHit.transform)
                {
                    rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
                    WalkOnStrand walkOnStrand = gameObject.AddComponent<WalkOnStrand>();
                    walkOnStrand.currentStrand = raycastHit.transform.parent.gameObject;
                    walkOnStrand.Initialize();
                    walkOnStrand.enabled = false;
                    inJumpingAnimation = true;
                    originalJumpPosition = position;
                    newJumpPosition = new Vector3(raycastHit.point.x, raycastHit.point.y, position.z);
                    transform.SetParent(raycastHit.transform.parent, true);
                }
                
                Debug.DrawRay(position, rigidbody.velocity.normalized * raycastHit.distance, Color.yellow);
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
