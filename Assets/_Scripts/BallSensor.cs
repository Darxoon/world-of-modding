using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSensor : MonoBehaviour
{
    public bool isGrounded = false;
    public bool isTouchingWall = false;

    [SerializeField] private CapsuleCollider2D groundCollider;
    [SerializeField] private CapsuleCollider2D wallCollider;

    private void Update()
    {
        // reset rotation
        transform.rotation = Quaternion.identity;
        // update isGrounded and isTouchingWall
        isGrounded = groundCollider.IsTouchingLayers(LayerMask.GetMask("Geometry"));
        isTouchingWall = wallCollider.IsTouchingLayers(LayerMask.GetMask("Geometry"));
    }
    

    

}
