using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strand : MonoBehaviour
{
    public GameObject connectedBall1;
    public GameObject connectedBall2;
    public SpriteRenderer renderer;
    public GameObject rendererObject;

    bool spriteEnabled = false;

    [SerializeField] private float stretchMultiplier = 1.35f;
    public float strandThickness;

    private float leThicc;

    private void Start()
    {
        //transform.localScale *= 0.5f;
    }

    private void Update()
    {
        // interpolate between goos
        //GetComponent<SpriteRenderer>().size = new Vector2(GetComponent<SpriteRenderer>().size.x, Vector2.Distance(transform.position, connectedBall1.transform.position) * 10f);


        // calculate rotation
        Vector2 direction = connectedBall1.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // apply rotation
        rendererObject.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);


        // calculate the actual difference between this goo ball and the other gooball
        float distance = Vector3.Distance(connectedBall2.transform.position, connectedBall1.transform.position);
        // apply the scale



        //le  t h i c c  calculation

        leThicc = 1 / distance;


        rendererObject.transform.localScale = new Vector3(strandThickness * leThicc, distance / 3f * stretchMultiplier, 0);


        // calculate and apply center
        Vector3 center = (connectedBall2.transform.position + connectedBall1.transform.position) * 0.5f;
        transform.position = center;


        if(!spriteEnabled)
        {
            spriteEnabled = true;
            renderer.enabled = true;
        }


        //do the extra m a s s 






    }
}
