using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[SuppressMessage("ReSharper", "ConvertToAutoPropertyWhenPossible")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "ConvertToAutoProperty")]
public class Gooball : MonoBehaviour
{

    // TODO: TEMPORARY (replaced with level loading)
    [SerializeField] public Sprite strandSprite;
    [SerializeField] private bool byPrefab;
    [SerializeField] public string randomID;
    public bool finishedLoading;
    #region Components
    
    private new Rigidbody2D rigidbody;
    private Camera mainCam;
    
    #endregion
    
    [Header("Inspector Initialization")]
    public Gooball[] initialStrands;


    [Header("In game Strands")]
    public List<Gooball> attachedBalls;
    public List<Strand> strands = new List<Strand>();
    public Dictionary<int, Gooball> gooStrands = new Dictionary<int, Gooball>();
    
    
    [Header("Gooball properties")]
    [SerializeField] private string ballLayerMask = "Attached Balls";
    [SerializeField] private float originalMass = 3.23f;

    [SerializeField] private float towerMass = 3f;
    public float extraMass;

    public JSONGooball data;
    [Header("Strand physics")]

    [SerializeField] public float dampingRatio;
    [SerializeField] public float jointFrequency;

    public Vector2 strandDistanceRange = new Vector2(1f, 4f);
    public float strandMultiplier = 1.01f;
    public float strandThickness = 0.5f;
    public int strandCount = 2;
    public float strandLengthMax = 1.9f;
    public float strandLengthMin = 0; // TODO: Implement strandLengthMin (Polishing)
    public float strandLengthShrink = 1.8f; // TODO: Implement strandLengthShrink (Polishing)
    public float strandLengthShrinkSpeed = 1f;
    
    [Header("Attaching")]

    public int rays = 50;
    [SerializeField] private bool isTower;
    [SerializeField] private bool isDragged;
    public bool isOnStrand;

    [SerializeField] private List<Gooball> attachable;
    [SerializeField] private List<Vector2> attachablePoint;


    private Vector3 euler;
    private RaycastHit2D hit;

    private float randomSpeedMultiplier;

    #region Getters
    
    public float OriginalMass => originalMass;
    public bool IsDragged => isDragged;
    public bool IsTower => isTower;

    #endregion

    [FormerlySerializedAs("position")] 
    public Vector3 gooballPosition; //i had to add this because for some reason unity decides to do weird shit and just make everything offset



    private void Awake()
    {
        randomID = GameManager.GenerateRandomID(10);
        attachable = new List<Gooball>();
        attachablePoint = new List<Vector2>();
        
        StaticData.existingGooballs.Add(gameObject, this);
        
        
    }


    private void Start()
    {
        if (byPrefab)
        {
            rigidbody = gameObject.GetComponent<Rigidbody2D>();
            rigidbody.mass = OriginalMass + extraMass;
        }
        else
        {
            //do the loading move
            initialStrands = new Gooball[] { };
            
            rigidbody = gameObject.AddComponent<Rigidbody2D>();

            CircleCollider2D mainCol = gameObject.AddComponent<CircleCollider2D>();
            mainCol.radius = data.ball.radius;
            
            GameObject sensor = new GameObject("Sensor");
            sensor.transform.SetParent(transform);
            
            CapsuleCollider2D capsuleCollider = sensor.AddComponent<CapsuleCollider2D>();
            //TODO: ADD A WAY TO DEFINE THOSE TWO VARIABLES AUTOMATICALLY
            capsuleCollider.size = new Vector2(3.257942f, 0.9263445f);
            capsuleCollider.offset = new Vector2(0, -2.43f);
            capsuleCollider.direction = CapsuleDirection2D.Horizontal;
            capsuleCollider.isTrigger = true;
            
            GameObject wallColliderObject = new GameObject("WallCollider");
            wallColliderObject.transform.SetParent(sensor.transform);
            
            CapsuleCollider2D wallCollider = wallColliderObject.AddComponent<CapsuleCollider2D>();
            wallCollider.offset = new Vector2(-0.02958627f, -0.09866164f);
            wallCollider.size = new Vector2(5.809811f, 1.444782f);
            wallCollider.direction = CapsuleDirection2D.Horizontal;
            wallCollider.isTrigger = true;
            
            sensor.AddComponent<BallSensor>();
            Walk walkScript = gameObject.AddComponent<Walk>();
            //if 1 its going left, if 0 its right or the other way around idk
            walkScript.startingDirection = Random.Range(0, 1) == 1 ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0);
            walkScript.walkSpeed = data.ball.walkSpeed;
            walkScript.randomSpeedScale = data.ball.speedDifference.ToVector2();
            walkScript.doesCheckForStrands = data.ball.climber;
            
            randomSpeedMultiplier = Random.Range(data.ball.speedDifference.x, data.ball.speedDifference.y);
            rigidbody.mass = data.ball.mass;
            towerMass = data.ball.towerMass;
            strandCount = data.ball.strands;

            // parts  
            foreach (Part part in data.parts)
            {
                GameObject partObject = new GameObject(part.name);
                SpriteRenderer spriteRenderer = partObject.AddComponent<SpriteRenderer>();
                GameManager.imageFiles.TryGetValue(part.image[Random.Range(0, part.image.Length - 1)], out Sprite sprite);
                spriteRenderer.sprite = sprite;
                partObject.transform.SetParent(transform);
            }

            GameManager.imageFiles.TryGetValue(data.strand.image, out strandSprite);
            //strand
            Transform transform1 = transform;
            transform1.localScale = new Vector3(0.1f, 0.1f);
            transform1.localPosition = gooballPosition;
        }

        mainCam = Camera.main;
        
        if(initialStrands.Length > 0)
        {
            SetTowered();
            foreach (Gooball other in initialStrands)
            {
                MakeStrand(other);
                other.SetTowered();
            }
            attachedBalls = new List<Gooball>(initialStrands);
        } else
        {
            attachedBalls = new List<Gooball>();
        }

        finishedLoading = true;
    }

    private void StrandMass()
    {
        extraMass = 0;
        foreach (Strand strand in strands)
        {
            if(strand.gooballs.Count > 0)
                foreach (Gooball gooball in strand.gooballs)
                {
                    Vector3 position = transform.position;
                    float distancePercent = Vector3.Distance(position, gooball.transform.position) / Vector3.Distance(position, strand.OtherBall(this).transform.position);
                    distancePercent = 1 - distancePercent;
                    extraMass += gooball.towerMass * distancePercent;
                }
        }
    }

    private void Update()
    {


        if (Input.GetMouseButtonDown(0) && !IsTower && !GameManager.instance.isDragging)
        {
            RaycastHit2D[] hits = new RaycastHit2D[500];
            int size = Physics2D.GetRayIntersectionNonAlloc(mainCam.ScreenPointToRay(Input.mousePosition), hits, Mathf.Infinity);
            if (size > 0)
            {
                foreach (RaycastHit2D raycastHit2D in hits)
                {
                    Debug.DrawLine(mainCam.transform.position, raycastHit2D.point, Color.red);


                    if (raycastHit2D.transform == transform)
                    {
                        isDragged = true;
                        GameManager.instance.isDragging = true;
                        GameManager.instance.drag = raycastHit2D.transform.gameObject;
                    }
                }
            }
        }


        if (isDragged)
        {
            transform.SetParent(StaticData.balls.transform, true);
            isOnStrand = false;
            // positioning
            rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            Vector3 mousePosition = Input.mousePosition;
            Vector2 point = mainCam.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 3600f));
            transform.position = new Vector3(point.x, point.y, 0);

            // changing the layer
            gameObject.layer = LayerMask.NameToLayer("Selected Ball");

            // attaching 

            AttachRaycast();


            // stop dragging
            if (Input.GetMouseButtonUp(0))
            {
                GameManager.instance.isDragging = false;
                AttachRaycast();

                if (GameManager.instance.hoverStrand)
                {
                    gameObject.layer = LayerMask.NameToLayer("Detached Balls");
                    isDragged = false;
                    GameManager.instance.drag = null;
                    WalkOnStrand walkOnStrand = gameObject.AddComponent<WalkOnStrand>();
                    walkOnStrand.currentStrand = GameManager.instance.hoverStrand;
                    walkOnStrand.speed = data.ball.climbspeed * randomSpeedMultiplier;
                    walkOnStrand.Initialize();
                    transform.SetParent(GameManager.instance.hoverStrand.transform, true);
                    return;
                }

                // remove constraints
                if (GameManager.instance.drag != null) { rigidbody.constraints = RigidbodyConstraints2D.None; }
                // update fields
                isDragged = false;
                GameManager.instance.drag = null;

                //check if we are hovering over a strand



                // are the strands 1?
                if (strandCount == 1)
                {
                    MakeStrand(attachable[0]);
                    SetTowered();
                }
                else if (attachable.Count > 1)
                {
                    // are they connected?
                    if (attachable[0].attachedBalls.Contains(attachable[1])
                        || attachable[1].attachedBalls.Contains(attachable[0]))
                    {
                        // if there are enough balls to attach to
                        if (attachable.Count >= strandCount)
                        {
                            // attach to them normally
                            for (int i = 0; i < strandCount; i++)
                            {
                                MakeStrand(attachable[i]);
                            }
                            SetTowered();
                        }
                    }
                    // else
                    else
                    {
                        // act as a strand
                        //Debug.Log("I'm a strand!", gameObject);

                        Gooball other1 = attachable[0];
                        other1.gooStrands.Add(other1.attachedBalls.Count, this);
                        other1.MakeStrand(attachable[1]);
                        gameObject.SetActive(false);
                    }
                }
                else
                {
                    
                }
                Debug.Log("stopped dragging");
            }

        }

        if (isTower)
        {
            StrandMass();
            rigidbody.mass = originalMass + extraMass;
        }
    }




    public void MakeStrand(Gooball other)
    {
        Strand strand = GameManager.MakeStrand(this, other, dampingRatio, jointFrequency, strandThickness);
        if(strand)
        {
            strands.Add(strand);
            if (!other.strands.Contains(strand))
            {
                other.strands.Add(strand);
            }
        }
            
        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        // add the other goo ball to attached list
        if(!attachedBalls.Contains(other))
            attachedBalls.Add(other);
        if(!other.attachedBalls.Contains(this))
            other.attachedBalls.Add(this);
        isTower = true;
    }



    public void AttachRaycast()
    {
        Vector3 position = transform.position;
        // make the empty list
        attachable.Clear();
        attachablePoint.Clear();
        // cast the rays
        for (int i = 0; i < rays; i++)
        {
            // the vector for the ray
            euler = new Vector3(i / (rays * 1f) * 360f, 90f, 0f);
            // show the ray
            // ReSharper disable once Unity.InefficientMultiplicationOrder
            Debug.DrawRay(position, Quaternion.Euler(euler) * Vector3.forward * strandLengthMax, Color.blue);
            // cast the ray
            hit = Physics2D.Raycast(position, Quaternion.Euler(euler) * Vector3.forward, strandLengthMax, LayerMask.GetMask(ballLayerMask));
            // if it hit something
            if (hit)
            {
                attachable.Add(StaticData.existingGooballs[hit.transform.gameObject]);
                attachablePoint.Add(hit.point);
            }
        }
        
        // sort the array
        attachable.Sort((a, b) => Vector3.Distance(position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position)));

        // look for duplicate gooballs
        List<Transform> transformsUsed = new List<Transform>();
        // looping backwards
        for (int i = attachable.Count - 1; i >= 0; i--)
        {
            if (transformsUsed.IndexOf(attachable[i].transform) == -1)
                transformsUsed.Add(attachable[i].transform);
            else
                attachable.RemoveAt(i);
        }

        // DEBUG draws the rays
#if DEBUG
        for (int i = 0; i < attachable.Count; i++)
        {
            Debug.DrawLine(transform.position, attachablePoint[i], i < strandCount ? Color.magenta : Color.green);
        }
#endif

    }

    public void SetTowered()
    {
        isTower = true;
        gameObject.layer = LayerMask.NameToLayer("Attached Balls");
        // freeze THIS BALL's rotation
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
