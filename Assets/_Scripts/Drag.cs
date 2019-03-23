using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
    [SerializeField] private string ballLayerMask = "Attached Balls";
    [SerializeField] private int rays = 50;

    [Header("Debugging")]

    [SerializeField] private bool isDragged = false;
    [SerializeField] private GameObject drag;

    private Attach_1 attach_1;
    private Rigidbody2D rigid;

    //public Vector3 euler = new Vector3(90f, 0f, 1f);

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        attach_1 = GetComponent<Attach_1>();
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
                    //Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);


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
            attach_1.Attach(rays, ballLayerMask);


            // stop dragging
            if (Input.GetMouseButtonUp(0))
            {
                if (drag != null) { drag.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None; }
                isDragged = false;
                drag = null;
                gameObject.layer = LayerMask.NameToLayer("Detached Balls");
                Debug.Log("stopped dragging");
            }

        }


    }



}
