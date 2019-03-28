using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
    // Layer mask
    [SerializeField] private string ballLayerMask = "Attached Balls";
    // goo properties
    public int rays = 50;
    public int strandCount = 2;
    public int strandLength = 10;
    // initial strands 
    public GameObject[] initialStrands;
    // TEMPORARY
    [SerializeField] private Sprite strandSprite;

    [Header("Debugging")]

    [SerializeField] private bool isTower = false;
    [SerializeField] private bool isDragged = false;
    [SerializeField] private GameObject drag;

    public float[] attachable_arr;
    [SerializeField] private List<RaycastHit2D> attachable;

    public Rigidbody2D rigid;

    private Vector3 euler;
    private RaycastHit2D hit;


    //public Vector3 euler = new Vector3(90f, 0f, 1f);

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        attachable = new List<RaycastHit2D>();

        if(initialStrands.Length > 0)
        {
            SetTowered();
            for (int i = 0; i < initialStrands.Length; i++)
            {
                MakeStrand(initialStrands[i].transform);
                initialStrands[i].GetComponent<Drag>().SetTowered();
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && !isTower)
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
                        drag = hit.transform.gameObject;
                    }

                }
            }
        }

    }


    private void FixedUpdate()
    {
        if(isDragged)
        {
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
                AttachRaycast(rays, ballLayerMask);

                // remove constraints
                if (drag != null) { rigid.constraints = RigidbodyConstraints2D.None; }
                // update fields
                isDragged = false;
                drag = null;

                // are the strands 1?
                if(strandCount == 1)
                {
                    MakeStrand(attachable[0].transform);
                } else if(attachable.Count > 1)
                {
                    for (int i = 0; i < strandCount; i++)
                    {
                        MakeStrand(attachable[i].transform);
                    }
                }
                SetTowered();

                Debug.Log("stopped dragging");
            }

        }


    }


    void MakeStrand(Transform other)
    {
        // make the joint
        SpringJoint2D joint = gameObject.AddComponent<SpringJoint2D>();
        joint.connectedBody = other.GetComponent<Rigidbody2D>();
        joint.autoConfigureDistance = false;
        joint.dampingRatio = 0.73f;
        joint.frequency = 1.91f;

        // make the visual strand
        GameObject child = new GameObject("Strand");
        child.transform.SetParent(transform);
        // add the sprite renderer
        child.AddComponent<SpriteRenderer>().sprite = strandSprite;
        child.GetComponent<SpriteRenderer>().flipY = true;
        // add the strand controller 
        child.AddComponent<Strand>().connectedBall = other.gameObject;
        // reset the position
        child.transform.localPosition = Vector3.zero;
        // freeze THIS BALL's rotation
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        
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
            Debug.DrawRay(transform.position, (Quaternion.Euler(euler) * Vector3.forward) * 50, Color.blue);
            // cast the ray
            hit = Physics2D.Raycast(transform.position, Quaternion.Euler(euler) * Vector3.forward, strandLength, LayerMask.GetMask(ballLayerMask));
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
        for (int i = 0; i < attachable.Count; i++)
        {
            if(i < strandCount)
                Debug.DrawLine(transform.position, attachable[i].point, Color.magenta);
            else
                Debug.DrawLine(transform.position, attachable[i].point, Color.green);

        }

    }

    public void SetTowered()
    {
        isTower = true;
        gameObject.layer = LayerMask.NameToLayer("Attached Balls");
    }
}
