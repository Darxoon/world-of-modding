using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearForceField : MonoBehaviour
{
    public float forceX;
    public float forceY;
    public bool antigrav = false;
    public List<GameObject> things = new List<GameObject>();
    int matches = 0;
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Rigidbody2D>() != null && collision != null)
        {
            Rigidbody2D gooball = collision.GetComponent<Rigidbody2D>();
            //gooball.AddForce(new Vector2(this.gameObject.transform.position.x, this.gameObject.transform.position.y));
            Vector2 direction = (transform.position - gooball.transform.position).normalized;

            //float force = Mathf.Lerp(ForceAtCenter, ForceAtEdge, (transform.position - gooball.transform.position).magnitude / Radius); //not used in linear forcefield, since n

            //gooball.AddForce(direction * force * gooball.mass);
            gooball.AddForce(new Vector2(forceX, forceY));


            foreach (GameObject thing in things)
            {
                if (thing == collision.gameObject)
                {
                    matches = matches + 1;
                }
            }
            if (matches == 0)
            {
                things.Add(collision.gameObject);
            }
        }
    }
    
    public void FixedUpdate()
    {
        foreach (GameObject thing in things)
        {
            Rigidbody2D gooball = thing.GetComponent<Rigidbody2D>();
            Vector2 direction = new Vector2(forceX, forceY);
            gooball.AddForce(direction);
        }
    }
}
