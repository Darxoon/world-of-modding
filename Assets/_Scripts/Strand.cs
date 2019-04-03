using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strand : MonoBehaviour
{
    public GameObject connectedBall;

    bool spriteEnabled = false;

    [SerializeField] private float stretchMultiplier = 12f;

    private void Start()
    {
        transform.localScale *= 0.5f;
    }

    private void Update()
    {
        // interpolate between goos
        GetComponent<SpriteRenderer>().size = new Vector2(GetComponent<SpriteRenderer>().size.x, Vector2.Distance(transform.position, connectedBall.transform.position) * 10f);


        // calculate rotation
        Vector2 direction = connectedBall.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // apply rotation
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);


        // calculate the actual difference between this goo ball and the other gooball
        float distance = Vector3.Distance(transform.parent.position, connectedBall.transform.position);
        // apply the scale
        transform.localScale = new Vector3(transform.localScale.x, distance / 3f * stretchMultiplier, transform.localScale.z);


        // calculate and apply center
        Vector3 center = (transform.parent.position + connectedBall.transform.position) * 0.5f;
        transform.position = center;


        if(!spriteEnabled)
        {
            spriteEnabled = true;
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}
