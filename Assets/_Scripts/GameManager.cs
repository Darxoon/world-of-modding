using System.Collections.Generic;
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
    
    
    private Collider2D[] hoverStrandsUnfiltered = new Collider2D[16];
    private Collider2D[] hoverStrands = new Collider2D[16];

    [Header("Positioning fine tuning")]

    [SerializeField] private float scale;
    [SerializeField] public float positiveDistanceScale;
    [SerializeField] public float negativeDistanceScale;
    public JSONLevel currentLevel = null;
    
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
        StaticData.forcefields = GameObject.Find("Forcefields");
        StaticData.ui = GameObject.Find("UI");
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
        if (size == 0)
            hoverStrand = null;
        else
        {
            GameObject hoverStrandGameObject = hoverStrands.Last().transform.parent.gameObject;
            hoverStrand = StaticData.existingStrands[hoverStrandGameObject];
        }
        DragGooball();
    }

    private void DragGooball(){
        if(Input.GetMouseButtonDown(0) && !isDragging){
            RaycastHit2D[] hits = new RaycastHit2D[500];
            int size = Physics2D.GetRayIntersectionNonAlloc(mainCam.ScreenPointToRay(Input.mousePosition), hits, Mathf.Infinity, 
            LayerMask.GetMask("Detached Balls", "Attached Balls"));
            if (size > 0)
            {
                foreach (RaycastHit2D raycastHit2D in hits)
                {
                    //Debug.DrawLine(mainCam.transform.position, raycastHit2D.point, Color.red);
                    if(raycastHit2D.transform == null)
                        continue;
                    Gooball ball = raycastHit2D.transform.GetComponent<Gooball>();
                    if (ball != null && (ball.IsTower == false || ball.data.ball.detachable == true))
                    {
                        ball.isDragged = true;
                        isDragging = true;
                        drag = raycastHit2D.transform.gameObject;
                        return;
                    }
                }
            }
        }
    }

    [SerializeField]public static Dictionary<string, string> ResourcePaths = new Dictionary<string, string>();

    [SerializeField]public static Dictionary<string, AudioClip> audioFiles = new Dictionary<string, AudioClip>();
    [SerializeField]public static Dictionary<string, SpriteData> imageFiles = new Dictionary<string, SpriteData>();
    [SerializeField] public static Dictionary<string, JSONGooball> memoryGooballs = new Dictionary<string, JSONGooball>();

    public static Strand MakeStrand(Gooball b1, Gooball b2, float dampingRatio, float frequency, float strandThickness)
    {
        //check if we already have a strand that is connected to the same gooballs
        foreach (Transform strandTransform in StaticData.strands.transform)
        {
            Strand strandComponent = StaticData.existingStrands[strandTransform.gameObject];
            strandComponent.shouldDropSelf = true;
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
        b1.GetComponent<Gooball>().springs.Add(joint);
            
        joint.connectedBody = b2.gameObject.GetComponent<Rigidbody2D>();
        joint.autoConfigureDistance = false;
        //joint.dampingRatio = b1.dampingRatio;
        //joint.frequency = b1.jointFrequency;
        joint.distance = Mathf.Clamp(joint.distance, b1.strandDistanceRange.x, b1.strandDistanceRange.y) * b1.strandMultiplier;

        //physical
        GameObject child = new GameObject($"Strand ({GenerateRandomID(5)})") {tag = "Strand"};
        child.transform.SetParent(StaticData.strands.transform);


        //visual
        GameObject visual = new GameObject("StrandDisplay");
        visual.transform.SetParent(child.transform);
        SpriteRenderer spriteRenderer = visual.AddComponent<SpriteRenderer>();
        if(b1.strandSprite.sprite2x != null){
            visual.transform.localScale /= 2;
            spriteRenderer.sprite = b1.strandSprite.sprite2x;
        } else
            spriteRenderer.sprite = b1.strandSprite.sprite;
        spriteRenderer.flipY = true;
        //spriteRenderer.sortingLayerName = "Strands"; - this makes strands render in front of gooballs, which is undesireable
        spriteRenderer.enabled = false;

        Strand strand = child.AddComponent<Strand>();
        strand.connectedBall1 = b2.gameObject;
        strand.connectedBall1Class = b2;
        //child.AddComponent<Strand>().connectedBall1 = b2.gameObject;
        strand.connectedBall2 = b1.gameObject;
        strand.connectedBall2Class = b1;
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
            Strand component = StaticData.existingStrands[strand.gameObject];
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

    public void RunEvent(string onclick)
    {
        if(onclick.StartsWith("island")){
            StaticData.levelLoader.LoadLevel(onclick);
        } else if(onclick.StartsWith("pl_")){
            StaticData.levelLoader.LoadLevel(onclick.Substring(3));
        } else if(onclick.StartsWith("blimp")){
            StaticData.levelLoader.LoadLevel("Route99");
        }
    }

    public bool isDetaching = false;
    public GameObject detach = null;
}
