using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attached : MonoBehaviour
{
    [SerializeField] float pushRadius = 3f;
    [SerializeField] Collider2D[] goosPushing = new Collider2D[0];

    private void Update()
    {
        Physics2D.OverlapCircleNonAlloc(transform.position, pushRadius, goosPushing);
    }
}
