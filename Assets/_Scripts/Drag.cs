using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Drag : MonoBehaviour
{
    [SerializeField] private string ballLayerMask = "Attached Balls";
    [SerializeField] private int rays = 50;

    [Header("Debugging")]

    [SerializeField] private bool isDragged = false;
    [SerializeField] private GameObject drag;

    [SerializeField] private List<RaycastHit2D> attachable;

    private Rigidbody2D rigid;

    private Vector3 euler;
    private RaycastHit2D hit;


    //public Vector3 euler = new Vector3(90f, 0f, 1f);

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
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
                //attachable.Sort();
                if (drag != null) { rigid.constraints = RigidbodyConstraints2D.None; }
                isDragged = false;
                drag = null;
                gameObject.layer = LayerMask.NameToLayer("Detached Balls");
                Debug.Log("stopped dragging");
            }

        }


    }



    public void AttachRaycast(int rays, string ballLayerMask)
    {
        attachable = new List<RaycastHit2D>();
        for (int i = 0; i < rays; i++)
        {
            // the vector for the ray
            euler = new Vector3(i / (rays * 1f) * 360f, 90f, 0f);
            // show the ray
            Debug.DrawRay(transform.position, (Quaternion.Euler(euler) * Vector3.forward) * 50, Color.blue);
            // cast the ray
            hit = Physics2D.Raycast(transform.position, Quaternion.Euler(euler) * Vector3.forward, 50, LayerMask.GetMask(ballLayerMask));
            // if it hit something
            if (hit)
                attachable.Add(hit);
        }

        attachable.OrderBy(hit => hit.distance);
        attachable.Reverse();
        List<Transform> transformsUsed = new List<Transform>();
        // looping backwards
        for (int i = attachable.Count - 1; i >= 0; i--)
        {
            if (transformsUsed.IndexOf(attachable[i].transform) == -1)
                transformsUsed.Add(attachable[i].transform);
            else
                attachable.RemoveAt(i);
        }
        Debug.Log("lohl");
        foreach (RaycastHit2D item in attachable)
        {
            Debug.DrawLine(transform.position, item.point, Color.green);
        }

    }


}
