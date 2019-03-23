using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attach_1 : MonoBehaviour
{
    private Drag drag;

    private void Start()
    {
        drag = GetComponent<Drag>();
    }

    public void Attach(int rays, string ballLayerMask)
    {
        for (int i = 0; i < rays; i++)
        {
            // the vector for the ray
            Vector3 euler = new Vector3(i / (rays * 1f) * 360f, 90f, 0f);
            // show the ray
            Debug.DrawRay(transform.position, (Quaternion.Euler(euler) * Vector3.forward) * 50, Color.blue);
            // cast the ray
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Quaternion.Euler(euler) * Vector3.forward, 50, LayerMask.GetMask(ballLayerMask));
            if (hit)
            {
                // if it hit something
                Debug.DrawLine(transform.position, hit.point, Color.green);
            }
        }

    }
}
