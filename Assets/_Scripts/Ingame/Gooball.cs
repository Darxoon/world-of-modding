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
    [SerializeField] public SpriteData strandSprite;
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
    [SerializeField] public bool isDragged;
    [SerializeField] private bool isDetaching;
    public bool isOnStrand = false;

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
    public List<SpringJoint2D> springs = new List<SpringJoint2D>();
    private int minStrands = 0;
    private void Awake()
    {
        randomID = GameManager.GenerateRandomID(10);
        attachable = new List<Gooball>();
        attachablePoint = new List<Vector2>();
        
        StaticData.existingGooballs.Add(gameObject, this);
    }


    private void Start()
    {
        //random shit idk
        data ??= new JSONGooball();
        if(data.ball.strands > 2)
            minStrands = 2;
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        if(rigidbody == null)
            rigidbody = gameObject.AddComponent<Rigidbody2D>();
        rigidbody.mass = OriginalMass + extraMass;
        //do the loading move
        initialStrands = new Gooball[] { };
        
        CircleCollider2D mainCol = gameObject.AddComponent<CircleCollider2D>();
        mainCol.radius = data.ball.radius/100/2;
        GameObject forcefieldeffector = new GameObject("ffeffector");
        var eff = forcefieldeffector.AddComponent<CircleCollider2D>();
        eff.callbackLayers = LayerMask.GetMask("Forcefield");
        eff.contactCaptureLayers = LayerMask.GetMask("Forcefield");
        eff.radius = mainCol.radius;
        eff.transform.SetParent(transform);
        eff.excludeLayers = Physics2D.AllLayers & ~LayerMask.GetMask("Forcefield");
        eff.includeLayers = LayerMask.GetMask("Forcefield");
        //forcefieldeffector.layer = LayerMask.GetMask("Forcefield");
        forcefieldeffector.AddComponent<ForcefieldEffector>().rb = rigidbody;

        GameObject sensor = new GameObject("Sensor");
        sensor.transform.localScale = new Vector2(mainCol.radius/3, mainCol.radius/3);
        sensor.transform.SetParent(transform);
        
        CapsuleCollider2D capsuleCollider = sensor.AddComponent<CapsuleCollider2D>();
        //TODO: ADD A WAY TO DEFINE THOSE TWO VARIABLES AUTOMATICALLY
        capsuleCollider.size = new Vector2(3.257942f, 0.9263445f);
        capsuleCollider.offset = new Vector2(0, -2.43f);
        capsuleCollider.direction = CapsuleDirection2D.Horizontal;
        capsuleCollider.isTrigger = true;
        
        GameObject wallColliderObject = new GameObject("WallCollider");
        wallColliderObject.transform.SetParent(sensor.transform);
        wallColliderObject.transform.localScale = Vector3.one;
        
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
            int randomsel = Random.Range(0, part.image.Length - 1);
            if(!GameManager.imageFiles.TryGetValue(part.image[randomsel], out SpriteData sprite)) {
                Debug.LogWarning($"Could not find the texture for {part.image[randomsel]}");
                continue;
            }
            GameObject partObject = new GameObject(part.name);
            partObject.transform.localScale = part.scale.ToVector2();
            partObject.transform.localPosition = new Vector2(Random.Range(part.x.x, part.x.y), Random.Range(part.y.x, part.y.y));
            SpriteRenderer spriteRenderer = partObject.AddComponent<SpriteRenderer>();
            partObject.transform.SetParent(transform);
            spriteRenderer.sortingOrder = part.layer;
            if(sprite.sprite2x != null){
                spriteRenderer.sprite = sprite.sprite2x;
                partObject.transform.localScale /= 2;
            } else
                spriteRenderer.sprite = sprite.sprite;
        }

        GameManager.imageFiles.TryGetValue(data.strand.image, out strandSprite);
        //strand
        Transform transform1 = transform;
        float scale = Random.Range(1f, 1f + (data.ball.sizeVariation/5));
        transform1.localScale = new Vector3(scale, scale);
        transform1.localPosition = gooballPosition;
        
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
        return; //this method causes the structure to spaz out
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
        /*
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
        */
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
            Vector2 point = mainCam.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0f));
            if(!isTower)
                transform.position = new Vector3(point.x, point.y, 0);
            if(isTower){
                float dist = Vector3.Distance(point, transform.position);
                if(dist > data.detachstrand.maxLen){
                    //StopDrag();
                    RemoveStrand();
                    isTower = false;
                    //gameObject.layer = LayerMask.NameToLayer("Selected Ball");
                    //return;
                    //gameObject.layer = LayerMask.NameToLayer("Detached Balls");
                    //rigidbody.constraints = RigidbodyConstraints2D.None;
                }
            }

            // changing the layer
            gameObject.layer = LayerMask.NameToLayer("Selected Ball");

            //disable all collisions so we dont cause any problems
            GetComponent<CircleCollider2D>().enabled = false;

            // attaching 
            foreach(var walkOnStrand in GetComponents<WalkOnStrand>())
                Destroy(walkOnStrand);

            // stop dragging
            if (Input.GetMouseButtonUp(0))
                StopDrag();
        }

        if (isTower)
        {
            StrandMass();
            rigidbody.mass = originalMass + extraMass;
            foreach(var spring in springs){
                if(spring.distance > data.strand.maxLen1){
                    spring.distance = Mathf.Lerp(spring.distance, data.strand.maxLen1, 0.1f*Time.deltaTime);
                } else if(spring.distance < data.strand.minLen){
                    spring.distance = Mathf.Lerp(spring.distance, data.strand.minLen, 0.1f*Time.deltaTime);
                }
            }
        }
    }
    public void StopDrag(){
        GameManager.instance.isDragging = false;
        // update fields
        isDragged = false;
        GameManager.instance.drag = null;
        Debug.Log("stopped dragging");

        if(!isTower){
            gameObject.layer = LayerMask.NameToLayer("Detached Balls");
            if (GameManager.instance.hoverStrand)
            {
                isDragged = false;
                GameManager.instance.drag = null;
                WalkOnStrand walkOnStrand = gameObject.AddComponent<WalkOnStrand>();
                walkOnStrand.currentStrand = GameManager.instance.hoverStrand;
                walkOnStrand.speed = data.ball.climbspeed * randomSpeedMultiplier;
                walkOnStrand.Initialize();
                transform.SetParent(GameManager.instance.hoverStrand.transform, true);
                return;
            }
            //check if we are in front of a levelExit
            Vector3 pos = Input.mousePosition;
            pos.z = 5f;
            Vector2 worldMousePos = mainCam.ScreenToWorldPoint(pos);
            Collider2D[] result = new Collider2D[16];
            int size = Physics2D.OverlapPointNonAlloc(worldMousePos, result, LayerMask.GetMask("Geometry"));
            for(int i = 0; i < size; i++){
                Collider2D r = result[i];
                if(r.CompareTag("levelExit"))
                {
                    var comp = r.GetComponent<LevelExitComponent>();
                    if(comp.pipeOpen)
                        if(comp.GooballApproached(gameObject)){
                            RemoveStrand();
                            Destroy(gameObject);
                            return;
                        }
                }
            }
            AttachRaycast();

            // remove constraints
            rigidbody.constraints = RigidbodyConstraints2D.None;
            //if (GameManager.instance.drag != null) {  }
            

            //TODO: check if we are hovering over a strand

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
                    if (attachable.Count >= minStrands)
                    {
                        // attach to them normally
                        for (int i = 0; i < (attachable.Count < strandCount ? attachable.Count : strandCount); i++)
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
                    Gooball other1 = attachable[0];
                    other1.gooStrands.Add(other1.attachedBalls.Count, this);
                    other1.MakeStrand(attachable[1], true, data);
                    Destroy(gameObject);
                }
            }
            else
            {
                
            }
            GetComponent<CircleCollider2D>().enabled = true;
        }
        else{
            rigidbody.constraints = RigidbodyConstraints2D.None;
            gameObject.layer = LayerMask.NameToLayer("Attached Balls");
            GetComponent<CircleCollider2D>().enabled = true;
        }
    }
    public void MakeStrand(Gooball other, bool shouldDropSelf = false, JSONGooball baseData = null)
    {
        Strand strand = GameManager.MakeStrand(this, other, data.strand.dampFac, data.strand.springConst.x, strandThickness);
        //Debug.Log(Vector3.Distance(this.transform.position, other.transform.position));
        if(strand)
        {
            strands.Add(strand);
            if (!other.strands.Contains(strand))
            {
                other.strands.Add(strand);
            }
        }
        if(shouldDropSelf){
            strand.shouldDropSelf = true;
            strand.baseJSON = baseData;
        }
        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        // add the other goo ball to attached list
        if(!attachedBalls.Contains(other))
            attachedBalls.Add(other);
        if(!other.attachedBalls.Contains(this))
            other.attachedBalls.Add(this);
        isTower = true;
    }

    public void RemoveStrand()
    {
        List<Strand> Strands = new List<Strand>();
        foreach(Gooball gooball in attachedBalls)
        {
            Strands.Add(GameManager.GetStrandBetweenBalls(this, gooball));
            gooball.GetComponent<Gooball>().attachedBalls.Remove(this);
            foreach(SpringJoint2D spring in gooball.GetComponents<SpringJoint2D>())
            {
                if(spring.connectedBody == rigidbody)
                {
                    Destroy(spring);
                }
            }
            springs.Clear();   
        }
        foreach (SpringJoint2D thing in GetComponents<SpringJoint2D>())
        {
            Destroy(thing);
        }
        foreach(Strand strand in Strands)
        {
            if(strand.connectedBall1Class != this)
            {
                strand.connectedBall1Class.attachedBalls.Remove(this);
            }
            else
            {
                strand.connectedBall2Class.attachedBalls.Remove(this);
            }
            for(int i = 0; i < strand.transform.childCount; i++){
                Transform obj = strand.transform.GetChild(i);
                if(obj.CompareTag("Ball")){
                    obj.SetParent(StaticData.balls.transform);
                    obj.GetComponent<Gooball>().isOnStrand = false;
                    foreach(var walkOnStrand in obj.GetComponents<WalkOnStrand>())
                        Destroy(walkOnStrand);
                    obj.GetComponent<Rigidbody2D>().constraints = 0;
                    obj.GetComponent<CircleCollider2D>().enabled = true;
                }
            }
            if(strand.shouldDropSelf){
                JSONLevelLoader.SpawnGooball(strand.baseJSON, "", strand.transform.position);
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
        if(byPrefab && rigidbody == null){
            rigidbody = gameObject.GetComponent<Rigidbody2D>();
        }
        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
