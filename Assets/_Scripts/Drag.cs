using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
    [SerializeField] private string ballLayerMask;

    [SerializeField] private bool isDragged = false;
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private GameObject drag;

    //public Vector3 euler = new Vector3(90f, 0f, 1f);

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
        if(isDragged||true)
        {
            Debug.Log("what's up=");
            // positioning
            //gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            //Vector3 mousePosition = Input.mousePosition;
            //Vector2 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 3600f));
            //transform.position = new Vector3(point.x, point.y, 0);

            //// changing the layer
            //gameObject.layer = LayerMask.NameToLayer("Selected Ball");

            // attaching 
            for (int i = 0; i < 10; i++)
            {
                Debug.Log("new iteration");
                Vector3 euler = new Vector3(90f, 0f, i / 10f * 360f);
                Debug.Log(euler);
                // show where the ray started
                Debug.DrawLine(transform.position, transform.position + new Vector3(0f, 10f, 0f));
                // show the ray
                Debug.Log(Quaternion.Euler(euler) * Vector3.forward);
                Debug.DrawRay(transform.position, (Quaternion.Euler(euler) * Vector3.forward) * 50, Color.blue);
                // cast the ray
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Quaternion.Euler(new Vector3(0f, 0f, i / 10) * 360) * Vector3.forward, 50, LayerMask.GetMask(ballLayerMask));
                if (hit)
                {
                    Debug.DrawLine(hit.point, Camera.main.transform.position, Color.green);
                    Debug.Log(hit);
                }
            }

            // stop dragging
            //if (Input.GetMouseButtonUp(0))
            //{
            //    if (drag != null) { drag.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None; }
            //    isDragged = false;
            //    drag = null;
            //    gameObject.layer = LayerMask.NameToLayer("Detached Balls");
            //    Debug.Log("stopped dragging");
            //}

        }
        

    }



}
