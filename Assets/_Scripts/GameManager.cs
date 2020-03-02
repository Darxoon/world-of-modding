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
                if(c.gameObject.layer == LayerMask.NameToLayer("Strands"))
                {
                    hoverStrand = c.gameObject;
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
    public void MakeStrand(Transform b1, Transform b2)
    {
        //check if we already have a strand that is connected to the same gooballs
        foreach (Transform strand in StaticData.strands.transform)
        {
            Strand component = strand.GetComponent<Strand>();
            GameObject ball1 = component.connectedBall1;
            GameObject ball2 = component.connectedBall2;            
            if(ball1 == b1.gameObject && ball2 == b2.gameObject || ball2 == b1.gameObject && ball1 == b2.gameObject)
            {
                Debug.LogWarning($"found a duplicate strand between {b1} and {b2} \n " +
                    $"Strand's properties: \n" +
                    $"  ball1: {component.connectedBall1} \n" +
                    $"  ball2: {component.connectedBall2}");
                return;
            }
        }


        //spring joint
        SpringJoint2D joint = b1.gameObject.AddComponent<SpringJoint2D>();
        joint.connectedBody = b2.gameObject.GetComponent<Rigidbody2D>();
        joint.autoConfigureDistance = false;
        joint.dampingRatio = b1.GetComponent<Drag>().dampingRatio;
        joint.frequency = b1.GetComponent<Drag>().frequency;
        Drag b1Drag = b1.GetComponent<Drag>();
        joint.distance = Mathf.Clamp(joint.distance, b1Drag.strandDistanceRange.x, b1Drag.strandDistanceRange.y) * b1Drag.strandMulitplier;

        //visual
        GameObject child = new GameObject($"Strand ({GenerateRandomID(5)})");
        child.tag = "Strand";
        child.transform.SetParent(StaticData.strands.transform);

        SpriteRenderer spriteRenderer = child.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = b1Drag.strandSprite;
        spriteRenderer.flipY = true;
        spriteRenderer.sortingLayerName = "Strands";
        spriteRenderer.enabled = false;

        child.AddComponent<Strand>().connectedBall1 = b2.gameObject;
        child.GetComponent<Strand>().connectedBall2 = b1.gameObject;
        child.AddComponent<BoxCollider2D>().isTrigger = true;

        child.transform.localPosition = Vector3.zero;

        child.layer = LayerMask.NameToLayer("Strands");
        //b1.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    /// <summary>
    /// uses b1 and b2 gooballs to get the strand that is connecting them
    /// </summary>
    /// <param name="b1"></param>
    /// <param name="b2"></param>
    public GameObject getStrandBetweenBalls(GameObject b1, GameObject b2)
    {
        foreach(Transform strand in StaticData.strands.transform)
        {
            Strand component = strand.GetComponent<Strand>();
            GameObject ball1 = component.connectedBall1;
            GameObject ball2 = component.connectedBall2;
            if (ball1 == b1.gameObject && ball2 == b2.gameObject || ball2 == b1.gameObject && ball1 == b2.gameObject)
            {
                return strand.gameObject;
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
