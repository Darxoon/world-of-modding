using UnityEngine;

public class Walk : MonoBehaviour
{
    [Header("Walking Properties")]
    
    [SerializeField] private Vector3 startingDirection;
    [SerializeField] public float walkSpeed;
    [SerializeField] public Vector2 randomSpeedScale;

    [Header("Jumping Properties")] 
    
    [SerializeField] public bool doesCheckForStrands;
    [SerializeField] private float raycastLength = 1f;

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
        ballSensor = transform.GetChild(0).GetComponent<BallSensor>();
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
