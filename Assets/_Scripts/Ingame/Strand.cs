using System.Collections.Generic;
using UnityEngine;

public class Strand : MonoBehaviour
{
    [Header("Connected gooballs")]
    public GameObject connectedBall1;
    public GameObject connectedBall2;
    public Gooball connectedBall1Class;
    public Gooball connectedBall2Class;
    public bool shouldDropSelf = false;
    public JSONGooball baseJSON;

    [Header("Render stuff")]
    public new SpriteRenderer renderer;
    public GameObject rendererObject;

    [Header("Distance stuff")]  
    public List<Gooball> gooballs = new List<Gooball>();


    private bool spriteEnabled;

    [SerializeField] private float stretchMultiplier = 1.35f;
    public float strandThickness;
    private float spritelen = 0;

    private void Awake()
    {
        StaticData.existingStrands.Add(gameObject, this);
    }
    void Start(){
        rendererObject.transform.localScale = Vector2.one;
        spritelen = renderer.sprite.bounds.max.y - renderer.sprite.bounds.min.y;
    }
    private void Update()
    {
        

        Vector3 ball1Position = connectedBall1.transform.position;
        Vector3 ball2Position = connectedBall2.transform.position;

        // calculate rotation
        Vector2 direction = ball1Position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // apply rotation
        rendererObject.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        // calculate the actual difference between this goo ball and the other gooball
        float distance = Vector3.Distance(ball2Position, ball1Position);
        // apply the scale
        //renderer.drawMode = SpriteDrawMode.Sliced;
        renderer.size = new Vector2(strandThickness * 0.6f / distance, distance);
        //renderer.sprite.textureRect
        //rendererObject.transform.localScale = new Vector3(strandThickness * 0.6f / distance, distance / 3f * stretchMultiplier / 2f, 0);
        rendererObject.transform.localScale = new Vector3(strandThickness * 0.6f / distance, distance/spritelen, 0);

        // calculate and apply center
        Vector3 center = (ball2Position + ball1Position) * 0.5f;
        transform.position = center;

        if(!spriteEnabled)
        {
            spriteEnabled = true;
            renderer.enabled = true;
        }
        //extraMass();

    }
    public Gooball OtherBall(Gooball request)
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
        gooballs.Add(StaticData.existingGooballs[gooball.gameObject]);
    }

    public void ExitStrand(Transform gooball)
    {
        gooballs.Remove(StaticData.existingGooballs[gooball.gameObject]);
    }
}
