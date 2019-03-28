﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
    // Layer mask
    [SerializeField] private string ballLayerMask = "Attached Balls";
    // goo properties
    public int rays = 50;
    public int strands = 2;
    // TEMPORARY
    [SerializeField] private Sprite strandSprite;

    [Header("Debugging")]

    [SerializeField] private bool isDragged = false;
    [SerializeField] private GameObject drag;

    public float[] attachable_arr;
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
                AttachRaycast(rays, ballLayerMask);

                // remove constraints
                if (drag != null) { rigid.constraints = RigidbodyConstraints2D.None; }
                // update fields
                isDragged = false;
                drag = null;

                // are the strands 1?
                if(strands == 1)
                {
                    MakeSpringJoint(attachable[0]);
                } else if(attachable.Count > 1)
                {
                    for (int i = 0; i < strands; i++)
                    {
                        MakeSpringJoint(attachable[i]);
                    }
                }

                Debug.Log("stopped dragging");
            }

        }


    }


    void MakeSpringJoint(RaycastHit2D other)
    {
        // make the joint
        SpringJoint2D joint = gameObject.AddComponent<SpringJoint2D>();
        joint.connectedBody = other.rigidbody;
        joint.autoConfigureDistance = false;

        // make the visual strand
        GameObject child = new GameObject();
        child.transform.SetParent(transform);
        // add the sprite renderer
        child.AddComponent<SpriteRenderer>().sprite = strandSprite;
        child.GetComponent<SpriteRenderer>().flipY = true;
        // add the strand controller 
        child.AddComponent<Strand>().connectedBall = other.transform.gameObject;
        // reset the position
        child.transform.localPosition = Vector3.zero;
        // freeze THIS BALL's rotation
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;

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
        
        attachable.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position)));

        List<Transform> transformsUsed = new List<Transform>();
        // looping backwards
        for (int i = attachable.Count - 1; i >= 0; i--)
        {
            if (transformsUsed.IndexOf(attachable[i].transform) == -1)
                transformsUsed.Add(attachable[i].transform);
            else
                attachable.RemoveAt(i);
        }

        attachable.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position)));

        foreach (RaycastHit2D item in attachable)
        {
            Debug.DrawLine(transform.position, item.point, Color.green);
        }

    }


}
