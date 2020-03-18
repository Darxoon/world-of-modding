using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Collections;
using UnityEngine;

[SuppressMessage("ReSharper", "ConvertToAutoPropertyWhenPossible")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "ConvertToAutoProperty")]
public class Gooball : MonoBehaviour
{

    // TODO: TEMPORARY (replaced with level loading)
    [SerializeField] public Sprite strandSprite;
    
    [SerializeField] public string randomID;
    public bool finishedLoading = false;
    #region Components
    
    private new Rigidbody2D rigidbody;
    private Camera mainCam;
    
    #endregion
    
    [Header("Inspector Initialization")]
    public GameObject[] initialStrands;


    [Header("In game Strands")]
    public List<GameObject> attachedBalls;
    public List<Strand> strands = new List<Strand>();
    public Dictionary<int, Gooball> gooStrands = new Dictionary<int, Gooball>();
    
    
    [Header("Gooball properties")]
    [SerializeField] private string ballLayerMask = "Attached Balls";
    [SerializeField] private float originalMass = 3.23f;

    [SerializeField] private float towerMass = 3f;
    public float extraMass;

    public JSONGooball data = null;
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
    [SerializeField] private bool isDetaching;
    public bool isOnStrand = false;

    [SerializeField] private List<RaycastHit2D> attachable;


    private Vector3 euler;
    private RaycastHit2D hit;

    private float randomSpeedMultiplier;

    #region Getters
    
    public float OriginalMass => originalMass;
    public float TowerMass => towerMass;
    public bool IsDragged => isDragged;
    public bool IsTower => isTower;

    #endregion

    public Vector3 position = new Vector3(); //i had to add this because for some reason unity decides to do weird shit and just make everything offset

    private void Start()
    {
        #region Creating the gooball
        //do the loading move
        initialStrands = new GameObject[] { };
        rigidbody = gameObject.AddComponent<Rigidbody2D>();

        CircleCollider2D mainCol = gameObject.AddComponent<CircleCollider2D>();
        mainCol.radius = data.ball.radius;
        GameObject Sensor = new GameObject("Sensor");
        Sensor.transform.SetParent(transform);
        var ccol = Sensor.AddComponent<CapsuleCollider2D>();

        //TODO: ADD A WAY TO DEFINE THOSE TWO VARIABLES AUTOMAGICALLY
        ccol.size = new Vector2(3.257942f, 0.9263445f);
        ccol.offset = new Vector2(0, -2.43f);
        ccol.direction = CapsuleDirection2D.Horizontal;
        ccol.isTrigger = true;

        GameObject WallCol = new GameObject("WallCollider");
        WallCol.transform.SetParent(Sensor.transform);
        var wcol = WallCol.AddComponent<CapsuleCollider2D>();
        wcol.offset = new Vector2(-0.02958627f, -0.09866164f);
        wcol.size = new Vector2(5.809811f, 1.444782f);
        wcol.direction = CapsuleDirection2D.Horizontal;
        wcol.isTrigger = true;
        Sensor.AddComponent<BallSensor>();
        Walk walkscript = gameObject.AddComponent<Walk>();

        

        //if 1 its going left, if 0 its right or the other way around idk
        if(Random.Range(0,1) == 1)
        {
            walkscript.startingDirection = new Vector3(1, 0, 0);
        }
        else
            walkscript.startingDirection = new Vector3(-1, 0, 0);

        walkscript.walkSpeed = data.ball.walkSpeed;
        walkscript.randomSpeedScale = data.ball.speedDifference.ToVector2();
        walkscript.doesCheckForStrands = data.ball.climber;
        randomSpeedMultiplier = Random.Range(data.ball.speedDifference.x, data.ball.speedDifference.y);
        rigidbody.mass = data.ball.mass;
        towerMass = data.ball.towerMass;
        strandCount = data.ball.strands;
        
        //ayy graphics  
        foreach(var part in data.parts)
        {
            GameObject tada = new GameObject(part.name);
            SpriteRenderer spr = tada.AddComponent<SpriteRenderer>();
            Sprite sprait = null;
            GameManager.imageFiles.TryGetValue(part.image[Random.Range(0, part.image.Length-1)], out sprait);
            spr.sprite = sprait;
            tada.transform.SetParent(transform);
        }
        //Sprite sprait = null;
        GameManager.imageFiles.TryGetValue(data.strand.image, out strandSprite);
        //strand
        transform.localScale = new Vector3(0.1f, 0.1f);
        transform.localPosition = position;
        //rigidbody.mass = OriginalMass + extraMass;
        #endregion
        mainCam = Camera.main;
        randomID = GameManager.GenerateRandomID(10);
        attachable = new List<RaycastHit2D>();

        if(initialStrands.Length > 0)
        {
            SetTowered();
            foreach (GameObject other in initialStrands)
            {
                MakeStrand(other.GetComponent<Gooball>());
                other.GetComponent<Gooball>().SetTowered();
            }
            attachedBalls = new List<GameObject>(initialStrands);
        } else
        {
            attachedBalls = new List<GameObject>();
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
                    float distancePercent = Vector3.Distance(position, gooball.transform.position) / Vector3.Distance(position, strand.otherBall(this).transform.position);
                    distancePercent = 1 - distancePercent;
                    extraMass += gooball.TowerMass * distancePercent;
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
        if (data.ball.detachable)
        {
            if (Input.GetMouseButtonDown(0) && isTower)
            {
                RaycastHit2D[] hits = new RaycastHit2D[500];
                int size = Physics2D.GetRayIntersectionNonAlloc(mainCam.ScreenPointToRay(Input.mousePosition), hits, Mathf.Infinity);
                if (size > 0)
                {
                    foreach (RaycastHit2D raycastHit2D in hits)
                    {
                        //Debug.DrawLine(mainCam.transform.position, raycastHit2D.point, Color.red);


                        if (raycastHit2D.transform == transform)
                        {
                            isDetaching = true;
                            GameManager.instance.isDetaching = true;
                            GameManager.instance.detach = transform.gameObject;
                        }
                    }
                }
            }

            if (isDetaching)
            {
                Vector3 mousepos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                mousepos = new Vector3(mousepos.x, mousepos.y);
                Debug.DrawLine(transform.position, mousepos, Color.red);

                if(Vector3.Distance(mousepos, transform.position) > data.detachstrand.maxLen)
                {
                    RemoveStrand();
                    isDragged = true;
                    GameManager.instance.isDragging = true;
                    GameManager.instance.drag = gameObject;
                    GameManager.instance.detach = null;
                    GameManager.instance.isDetaching = false;
                    isDetaching = false;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    GameManager.instance.detach = null;
                    GameManager.instance.isDetaching = false;
                    isDetaching = false;
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
                    MakeStrand(attachable[0].transform.gameObject.GetComponent<Gooball>());
                    SetTowered();
                }
                else if (attachable.Count > 1)
                {
                    // are they connected?
                    if (attachable[0].transform.gameObject.GetComponent<Gooball>().attachedBalls.Contains(attachable[1].transform.gameObject)
                        || attachable[1].transform.gameObject.GetComponent<Gooball>().attachedBalls.Contains(attachable[0].transform.gameObject))
                    {
                        // if there are enough balls to attach to
                        if (attachable.Count >= strandCount)
                        {
                            // attach to them normally
                            for (int i = 0; i < strandCount; i++)
                            {
                                MakeStrand(attachable[i].transform.GetComponent<Gooball>());
                            }
                            SetTowered();
                        }
                    }
                    // else
                    else
                    {
                        // act as a strand
                        //Debug.Log("I'm a strand!", gameObject);

                        Gooball other1 = attachable[0].transform.gameObject.GetComponent<Gooball>();
                        other1.gooStrands.Add(other1.attachedBalls.Count, this);
                        other1.MakeStrand(attachable[1].transform.GetComponent<Gooball>());
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
        Strand strand = StaticData.gameManager.MakeStrand(transform, other.transform, dampingRatio, jointFrequency, strandThickness);
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
        if(!attachedBalls.Contains(other.gameObject))
            attachedBalls.Add(other.gameObject);
        if(!other.attachedBalls.Contains(gameObject))
            other.attachedBalls.Add(gameObject);
        isTower = true;
    }

    public void RemoveStrand()
    {
        List<Strand> Strands = new List<Strand>();
        foreach(GameObject gooball in attachedBalls)
        {
            Strands.Add(GameManager.instance.getStrandBetweenBalls(gameObject, gooball));
            gooball.GetComponent<Gooball>().attachedBalls.Remove(gameObject);
            foreach(SpringJoint2D spring in gooball.GetComponents<SpringJoint2D>())
            {
                if(spring.connectedBody == rigidbody)
                {
                    Destroy(spring);
                }
            }
            
        }
        foreach (SpringJoint2D thing in GetComponents<SpringJoint2D>())
        {
            Destroy(thing);
        }
        foreach(Strand strand in Strands)
        {
            if(strand.connectedBall1Class != this)
            {
                strand.connectedBall1Class.attachedBalls.Remove(gameObject);
            }
            else
            {
                strand.connectedBall2Class.attachedBalls.Remove(gameObject);
            }
            Destroy(strand.gameObject);

        }
        attachedBalls.Clear();
        isTower = false;
        gameObject.layer = LayerMask.NameToLayer("Detached Balls");
        rigidbody.constraints = RigidbodyConstraints2D.None;
    }

    public void AttachRaycast()
    {
        Vector3 position = transform.position;
        // make the empty list
        attachable.Clear();
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
                attachable.Add(hit);
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
            Debug.DrawLine(transform.position, attachable[i].point, i < strandCount ? Color.magenta : Color.green);
        }
#endif

    }

    public void SetTowered()
    {
        isTower = true;
        gameObject.layer = LayerMask.NameToLayer("Attached Balls");
        // freeze THIS BALL's rotation
        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
