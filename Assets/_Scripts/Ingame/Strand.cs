using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strand : MonoBehaviour
{
    [Header("Connected gooballs")]
    public GameObject connectedBall1;
    public GameObject connectedBall2;
    public Gooball connectedBall1Class;
    public Gooball connectedBall2Class;


    [Header("Render stuff")]
    public SpriteRenderer renderer;
    public GameObject rendererObject;

    [Header("Distance stuff")]  
    public List<Gooball> gooballs = new List<Gooball>();

    [Header("Towermass stuff")]
    private List<float> b1Weights = new List<float>();
    private List<float> b2Weights = new List<float>();
    private float b1TotalWeight = 0f;
    private float b2TotalWeight = 0f;

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
        //extraMass();

    }
    public Gooball otherBall(Gooball request)
    {
        if (request.randomID == connectedBall1Class.randomID)
            return connectedBall2Class;
        else if (request.randomID == connectedBall2Class.randomID)
            return connectedBall1Class;
        return null;
    } 


    public void EnterStrand(Transform gooball)
    {
        gooball.SetParent(transform);
        gooballs.Add(gooball.gameObject.GetComponent<Gooball>());
    }

    public void ExitStrand(Transform gooball)
    {
        gooballs.Remove(gooball.gameObject.GetComponent<Gooball>());
    }
}
