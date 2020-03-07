using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Serialization;

[SuppressMessage("ReSharper", "ConvertToAutoPropertyWhenPossible")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "ConvertToAutoProperty")]
public class Gooball : MonoBehaviour
{
    // Layer mask
    [SerializeField] private string ballLayerMask = "Attached Balls";

    // goo properties
    
    // initial strands 
    public GameObject[] initialStrands;

    // the balls it is connected during the game
    public List<GameObject> attachedBalls;
    public Dictionary<int, Gooball> gooStrands = new Dictionary<int, Gooball>();
    // TEMPORARY
    [SerializeField] public Sprite strandSprite;

    #region Gooball properties
    [Header("Gooball properties")]
    [SerializeField] private float originalMass = 3.23f;
    public float OriginalMass => originalMass;

    [SerializeField] private float towerMass = 3f;
    public float TowerMass => towerMass;
    
    public Vector3 originalScale;
    #endregion

    #region Strand Settings
    [Header("Strand Settings")]

    [SerializeField] public float dampingRatio;
    [SerializeField] public float frequency;

    public Vector2 strandDistanceRange = new Vector2(1f, 4f);
    [FormerlySerializedAs("strandMulitplier")] public float strandMultiplier = 1.01f;
    [FormerlySerializedAs("StrandThickness")] public float strandThickness = 0.5f;
    public int strandCount = 2;
    public float strandLengthMax = 1.9f;
    public float strandLengthMin = 0;
    public float strandLengthShrink = 1.8f;
    [FormerlySerializedAs("strandLenghtShrinkSpeed")] public float strandLengthShrinkSpeed = 1f;
    #endregion
    #region Attatchment system
    [Header("Attachment system")]

    public int rays = 50;
    [SerializeField] private bool isTower;
    [SerializeField] private bool isDragged;
    //[SerializeField] private GameObject drag;

    [FormerlySerializedAs("attachable_arr")] public float[] attachableArr;
    [SerializeField] private List<RaycastHit2D> attachable;

    public Rigidbody2D rigid;

    private Vector3 euler;
    private RaycastHit2D hit;
    #endregion

    public float extraMass;
    public List<Strand> strands = new List<Strand>();


    [SerializeField]public string randomID;

    #region Getters

    public bool IsDragged => isDragged;
    public bool IsTower => isTower;

    private bool hasShrunkToSize = false;
    private Camera mainCam;
    private new Rigidbody2D rigidbody2D;

    public Gooball()
    {
        isDragged = false;
    }

    #endregion


    //public Vector3 euler = new Vector3(90f, 0f, 1f);



    private void Start()
    {
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
        rigid = GetComponent<Rigidbody2D>();
        rigid.mass = OriginalMass + extraMass;
        originalScale = transform.lossyScale;
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


        if (isDragged)
        {
            transform.SetParent(StaticData.balls.transform, true);
            // positioning
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
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

                if (GameManager.instance.hoverStrand != null)
                {
                    isDragged = false;
                    GameManager.instance.drag = null;
                    gameObject.AddComponent<WalkOnStrand>().currentStrand = GameManager.instance.hoverStrand;
                    transform.SetParent(GameManager.instance.hoverStrand.transform, true);
                    return;
                }

                // remove constraints
                if (GameManager.instance.drag != null) { rigid.constraints = RigidbodyConstraints2D.None; }
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
            rigid.mass = originalMass + extraMass;
        }
    }




    public void MakeStrand(Gooball other)
    {
        Strand strand = StaticData.gameManager.MakeStrand(transform, other.transform, dampingRatio, frequency, strandThickness);
        if(strand)
        {
            strands.Add(strand);
            if (!other.strands.Contains(strand))
            {
                other.strands.Add(strand);
            }
        }
            
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        // add the other goo ball to attached list
        if(!attachedBalls.Contains(other.gameObject))
            attachedBalls.Add(other.gameObject);
        if(!other.attachedBalls.Contains(gameObject))
            other.attachedBalls.Add(gameObject);
        isTower = true;
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
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
