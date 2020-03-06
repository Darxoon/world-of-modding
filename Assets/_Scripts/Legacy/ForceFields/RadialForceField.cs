using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialForceField : MonoBehaviour
{

    public float ForceAtEdge;
    public float ForceAtCenter;
    public float Radius;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Rigidbody2D>() != null)
        {
            Rigidbody2D gooball = collision.GetComponent<Rigidbody2D>();
            //gooball.AddForce(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y));
            Vector2 direction = (transform.position - gooball.transform.position).normalized;

            float force = Mathf.Lerp(ForceAtCenter, ForceAtEdge, (transform.position - gooball.transform.position).magnitude / Radius);

            gooball.AddForce(direction * force * gooball.mass);
        }
    }
    
}
