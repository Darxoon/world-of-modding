using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance;

    public GameObject drag;
    public GameObject hoverStrand;
    private void Awake()
    {
        if (instance)
            enabled = false;
        else
            instance = this;

        StaticData.balls = GameObject.Find("Balls");
        StaticData.geometry = GameObject.Find("Geometry");
        StaticData.sceneLayers = GameObject.Find("SceneLayers");
        StaticData.strands = GameObject.Find("Strands");
        StaticData.gameManager = this;
    }
    #endregion

    private void Update()
    {
        bool FoundStrand = false;

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 5f;
        Vector2 v = Camera.main.ScreenToWorldPoint(mousePosition);
        Collider2D[] col = Physics2D.OverlapPointAll(v);
        if (col.Length > 0)
        {
            foreach (Collider2D c in col)
            {
                //Debug.Log(c.gameObject.name + " " + c.gameObject.layer + " " + LayerMask.NameToLayer("Strands"));
                if(c.gameObject.layer == LayerMask.NameToLayer("Strands"))
                {
                    Debug.Log("Found a strand");
                    hoverStrand = c.gameObject.transform.parent.gameObject;
                    FoundStrand = true;
                }
            }
        }
        if (!FoundStrand)
        {
            hoverStrand = null;
        }
    }

    /// <summary>
    /// Creates a strand between gooball b1 and gooball b2
    /// </summary>
    /// <param name="b1">The gooball that invokes MakeStrand</param>
    /// <param name="b2">The gooball that we connect the strand to</param>
    public Strand MakeStrand(Transform b1, Transform b2, float dampingRatio, float frequency, float strandThickness)
    {
        //check if we already have a strand that is connected to the same gooballs
        foreach (Transform Strand in StaticData.strands.transform)
        {
            Strand component = Strand.GetComponent<Strand>();
            GameObject ball1 = component.connectedBall1;
            GameObject ball2 = component.connectedBall2;            
            if(ball1 == b1.gameObject && ball2 == b2.gameObject || ball2 == b1.gameObject && ball1 == b2.gameObject)
            {
                Debug.LogWarning($"found a duplicate strand between {b1} and {b2} \n " +
                    $"Strand's properties: \n" +
                    $"  ball1: {component.connectedBall1} \n" +
                    $"  ball2: {component.connectedBall2}");
                return null;
            }
        }


        //spring joint
        SpringJoint2D joint = b1.gameObject.AddComponent<SpringJoint2D>();
        joint.dampingRatio = dampingRatio;
        joint.frequency = frequency;
            
        joint.connectedBody = b2.gameObject.GetComponent<Rigidbody2D>();
        joint.autoConfigureDistance = false;
        joint.dampingRatio = b1.GetComponent<Gooball>().dampingRatio;
        joint.frequency = b1.GetComponent<Gooball>().frequency;
        Gooball b1Drag = b1.GetComponent<Gooball>();
        joint.distance = Mathf.Clamp(joint.distance, b1Drag.strandDistanceRange.x, b1Drag.strandDistanceRange.y) * b1Drag.strandMulitplier;

        //physical
        GameObject child = new GameObject($"Strand ({GenerateRandomID(5)})");
        child.tag = "Strand";
        child.transform.SetParent(StaticData.strands.transform);


        //visual
        GameObject visual = new GameObject("StrandDisplay");
        visual.transform.SetParent(child.transform);
        SpriteRenderer spriteRenderer = visual.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = b1Drag.strandSprite;
        spriteRenderer.flipY = true;
        spriteRenderer.sortingLayerName = "Strands";
        spriteRenderer.enabled = false;

        Strand strand = child.AddComponent<Strand>();
        strand.connectedBall1 = b2.gameObject;
        strand.connectedBall1Class = b2.GetComponent<Gooball>();
        //child.AddComponent<Strand>().connectedBall1 = b2.gameObject;
        strand.connectedBall2 = b1.gameObject;
        strand.connectedBall2Class = b1.GetComponent<Gooball>();
        strand.renderer = spriteRenderer;
        strand.rendererObject = visual;
        strand.strandThickness = strandThickness;
        visual.AddComponent<BoxCollider2D>().isTrigger = true;

        child.transform.localPosition = Vector3.zero;

        visual.layer = LayerMask.NameToLayer("Strands");

        return strand;
    }

    /// <summary>
    /// uses b1 and b2 gooballs to get the strand that is connecting them
    /// </summary>
    /// <param name="b1"></param>
    /// <param name="b2"></param>
    public Strand getStrandBetweenBalls(GameObject b1, GameObject b2)
    {
        foreach(Transform strand in StaticData.strands.transform)
        {
            Strand component = strand.GetComponent<Strand>();
            GameObject ball1 = component.connectedBall1;
            GameObject ball2 = component.connectedBall2;
            if (ball1 == b1.gameObject && ball2 == b2.gameObject || ball2 == b1.gameObject && ball1 == b2.gameObject)
            {
                return component;
            }
        }
        return null;
    }

    public static string GenerateRandomID(int length)
    {
        Random random = new Random();
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Range(0, s.Length)]).ToArray());
    }

    public bool isDragging = false;
}
