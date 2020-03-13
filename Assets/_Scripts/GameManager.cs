using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject drag;
    public Strand hoverStrand;
    
    public bool isDragging;

    public Camera mainCam;
    
    
    private Collider2D[] hoverStrandsUnfiltered = new Collider2D[8];
    private Collider2D[] hoverStrands = new Collider2D[8];
    
    private void Awake()
    {
        if (instance)
            enabled = false;
        else
            instance = this;
        
        
        mainCam = Camera.main;
        
        StaticData.balls = GameObject.Find("Balls");
        StaticData.geometry = GameObject.Find("Geometry");
        StaticData.sceneLayers = GameObject.Find("SceneLayers");
        StaticData.strands = GameObject.Find("Strands");
        StaticData.gameManager = this;

    }

    private void Update()
    {
        // get world mouse position
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 5f;
        Vector2 worldMousePos = mainCam.ScreenToWorldPoint(mousePosition);
        
        // get strand on mouse position
        int size = Physics2D.OverlapPointNonAlloc(worldMousePos, hoverStrandsUnfiltered, LayerMask.GetMask("Strands"));

        hoverStrands = hoverStrandsUnfiltered.Where(x => x != null).ToArray();
        if(hoverStrands.Length == 0)
            return;
        Transform hoverStrandTransform = hoverStrands.Last().transform.parent;
        if(hoverStrand == null || hoverStrandTransform != hoverStrand.transform)
            hoverStrand = size > 0 ? hoverStrandTransform.GetComponent<Strand>() : null;
    }

    public static Strand MakeStrand(Gooball b1, Gooball b2, float dampingRatio, float frequency, float strandThickness)
    {
        //check if we already have a strand that is connected to the same gooballs
        foreach (Transform strandTransform in StaticData.strands.transform)
        {
            Strand strandComponent = strandTransform.GetComponent<Strand>();
            GameObject ball1 = strandComponent.connectedBall1;
            GameObject ball2 = strandComponent.connectedBall2;            
            if(ball1 == b1.gameObject && ball2 == b2.gameObject || ball2 == b1.gameObject && ball1 == b2.gameObject)
            {
                Debug.LogWarning($"found a duplicate strand between {b1} and {b2} \n " +
                    "Strand's properties: \n" +
                    $"  ball1: {strandComponent.connectedBall1} \n" +
                    $"  ball2: {strandComponent.connectedBall2}");
                return null;
            }
        }


        //spring joint
        SpringJoint2D joint = b1.gameObject.AddComponent<SpringJoint2D>();
        joint.dampingRatio = dampingRatio;
        joint.frequency = frequency;
            
        joint.connectedBody = b2.gameObject.GetComponent<Rigidbody2D>();
        joint.autoConfigureDistance = false;
        joint.dampingRatio = b1.dampingRatio;
        joint.frequency = b1.jointFrequency;
        joint.distance = Mathf.Clamp(joint.distance, b1.strandDistanceRange.x, b1.strandDistanceRange.y) * b1.strandMultiplier;

        //physical
        GameObject child = new GameObject($"Strand ({GenerateRandomID(5)})") {tag = "Strand"};
        child.transform.SetParent(StaticData.strands.transform);


        //visual
        GameObject visual = new GameObject("StrandDisplay");
        visual.transform.SetParent(child.transform);
        SpriteRenderer spriteRenderer = visual.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = b1.strandSprite;
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

    public static Strand GetStrandBetweenBalls(Gooball b1, Gooball b2)
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach(Transform strand in StaticData.strands.transform)
        {
            Strand component = strand.GetComponent<Strand>();
            Gooball ball1 = component.connectedBall1Class;
            Gooball ball2 = component.connectedBall2Class;
            if (ball1 == b1 && ball2 == b2 || ball2 == b1 && ball1 == b2)
            {
                return component;
            }
        }
        return null;
    }

    public static string GenerateRandomID(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Range(0, s.Length)]).ToArray());
    }

}
