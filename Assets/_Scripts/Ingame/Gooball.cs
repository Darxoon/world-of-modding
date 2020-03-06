using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    [SerializeField] private float _originalMass = 3.23f;
    public float originalMass
    {
        get
        {
            return _originalMass;
        }
    }

    private float _towerMass = 3f;
    public float towerMass
    {
        get
        {
            return _originalMass;
        }
    }
    public Vector3 originalScale;
    #endregion

    #region Strand Settings
    [Header("Strand Settings")]

    [SerializeField] public float dampingRatio;
    [SerializeField] public float frequency;

    public Vector2 strandDistanceRange = new Vector2(1f, 4f);
    public float strandMulitplier = 1.01f;
    public float StrandThickness = 0.5f;
    public int strandCount = 2;
    public float strandLengthMax = 1.9f;
    public float strandLengthMin = 0;
    public float strandLengthShrink = 1.8f;
    public float strandLenghtShrinkSpeed = 1f;
    #endregion
    #region Attatchment system
    [Header("Attatchment system")]

    public int rays = 50;
    [SerializeField] private bool isTower = false;
    [SerializeField] private bool isDragged = false;
    //[SerializeField] private GameObject drag;

    public float[] attachable_arr;
    [SerializeField] private List<RaycastHit2D> attachable;

    public Rigidbody2D rigid;

    private Vector3 euler;
    private RaycastHit2D hit;
    #endregion

    public float extraMass = 0;
    public List<Strand> strands = new List<Strand>();


    [SerializeField]public string randomID;

    #region Getters

    public bool IsDragged => isDragged;
    public bool IsTower => isTower;

    private bool hasShrunkToSize = false;
    #endregion


    //public Vector3 euler = new Vector3(90f, 0f, 1f);



    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.mass = originalMass + extraMass;
        originalScale = transform.lossyScale;
        randomID = GameManager.GenerateRandomID(10);
        attachable = new List<RaycastHit2D>();

        if(initialStrands.Length > 0)
        {
            SetTowered();
            for (int i = 0; i < initialStrands.Length; i++)
            {
                MakeStrand(initialStrands[i].transform);
                initialStrands[i].GetComponent<Gooball>().SetTowered();
            }
            attachedBalls = new List<GameObject>(initialStrands);
        } else
        {
            attachedBalls = new List<GameObject>();
        }
    }

    private void strandMass()
    {
        extraMass = 0;
        float distancePercent = 0;
        foreach (Strand strand in strands)
        {
            if(strand.gooballs.Count > 0)
            foreach (Gooball gooball in strand.gooballs)
            {
                distancePercent = Vector3.Distance(transform.position, gooball.transform.position) / Vector3.Distance(transform.position, strand.otherBall(this).transform.position);
                distancePercent = 1 - distancePercent;
                extraMass += gooball.towerMass * distancePercent;
            }
        }
    }

    private void Update()
    {


        if (Input.GetMouseButtonDown(0) && !IsTower && !GameManager.instance.isDragging)
        {
            RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(Camera.main.ScreenPointToRay(Input.mousePosition), Mathf.Infinity);
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    RaycastHit2D hit = hits[i];
                    Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);


                    if (hit.transform == transform)
                    {
                        isDragged = true;
                        GameManager.instance.isDragging = true;
                        GameManager.instance.drag = hit.transform.gameObject;
                    }

                }
            }
        }


        if (isDragged)
        {
            if(GetComponent<Gooball>() != null)
                Destroy(GetComponent<WalkOnStrand>());
            transform.SetParent(StaticData.balls.transform, true);
            // positioning
            gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            Vector3 mousePosition = Input.mousePosition;
            Vector2 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 3600f));
            transform.position = new Vector3(point.x, point.y, 0);

            // changing the layer
            gameObject.layer = LayerMask.NameToLayer("Selected Ball");

            // attaching 

            AttachRaycast(rays, ballLayerMask);


            // stop dragging
            if (Input.GetMouseButtonUp(0))
            {
                GameManager.instance.isDragging = false;
                AttachRaycast(rays, ballLayerMask);

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
                    MakeStrand(attachable[0].transform);
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
                                MakeStrand(attachable[i].transform);
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
                        other1.MakeStrand(attachable[1].transform);
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
            strandMass();
            rigid.mass = _originalMass + extraMass;
        }
    }




    public void MakeStrand(Transform other)
    {
        Strand strand = StaticData.gameManager.MakeStrand(transform, other, dampingRatio, frequency, StrandThickness);
        if(strand != null)
        {
            strands.Add(strand);
            if (!other.GetComponent<Gooball>().strands.Contains(strand))
            {
                other.GetComponent<Gooball>().strands.Add(strand);
            }
        }
            
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        // add the other goo ball to attached list
        if(!attachedBalls.Contains(other.gameObject))
            attachedBalls.Add(other.gameObject);
        if(!other.GetComponent<Gooball>().attachedBalls.Contains(gameObject))
            other.GetComponent<Gooball>().attachedBalls.Add(gameObject);
        isTower = true;
    }



    public void AttachRaycast(int rays, string ballLayerMask)
    {
        // make the empty list
        attachable.Clear();
        // cast the rays
        for (int i = 0; i < rays; i++)
        {
            // the vector for the ray
            euler = new Vector3(i / (rays * 1f) * 360f, 90f, 0f);
            // show the ray
            Debug.DrawRay(transform.position, (Quaternion.Euler(euler) * Vector3.forward) * strandLengthMax, Color.blue);
            // cast the ray
            hit = Physics2D.Raycast(transform.position, Quaternion.Euler(euler) * Vector3.forward, strandLengthMax, LayerMask.GetMask(ballLayerMask));
            // if it hit something
            if (hit)
                attachable.Add(hit);
        }
        
        // sort the array
        attachable.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position)));

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
            if(i < strandCount)
                Debug.DrawLine(transform.position, attachable[i].point, Color.magenta);
            else
                Debug.DrawLine(transform.position, attachable[i].point, Color.green);

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
