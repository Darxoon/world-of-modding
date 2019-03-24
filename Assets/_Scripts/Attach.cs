using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attach : MonoBehaviour
{
    private Vector3 euler;
    private RaycastHit2D hit;

    public void AttachRaycast(int rays, string ballLayerMask)
    {
        for (int i = 0; i < rays; i++)
        {
            // the vector for the ray
            euler = new Vector3(i / (rays * 1f) * 360f, 90f, 0f);
            // show the ray
            Debug.DrawRay(transform.position, (Quaternion.Euler(euler) * Vector3.forward) * 50, Color.blue);
            // cast the ray
            hit = Physics2D.Raycast(transform.position, Quaternion.Euler(euler) * Vector3.forward, 50, LayerMask.GetMask(ballLayerMask));
            if (hit)
            {
                // if it hit something
                Debug.DrawLine(transform.position, hit.point, Color.green);
            }
        }

    }
}
