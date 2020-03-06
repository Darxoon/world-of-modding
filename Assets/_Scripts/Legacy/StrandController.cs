using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrandController : MonoBehaviour
{
    public List<GameObject> gooballs = new List<GameObject>();
    public List<float> lenghts = new List<float>();
    public bool isActive = false;
    public int StRands = 0;
    public float dampFac = 0;
    public float springConst = 0;
    public Texture2D texture;
    public byte[] image;
    public Sprite spriteE;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<GooBallController>() && collision.GetComponent<GooBallController>().isTower)
            gooballs.Add(collision.gameObject);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<GooBallController>() && collision.GetComponent<GooBallController>().isTower)
            gooballs.Remove(collision.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            //creating the lenght list
            gooballs.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position).CompareTo(Vector3.Distance(transform.position, b.transform.position)));

        }
    }
    public void AttatchStrand()
    {
        if (gooballs.Count != 0)
        {
            for (int i = 0; i < StRands; i++)
            {
                //transform.parent.gameObject.AddComponent<SpringJoint2D>();

                //int y = i;

                Rigidbody2D RigidBall = gooballs[i].GetComponent<Rigidbody2D>();
                SpringJoint2D Spring = transform.parent.gameObject.AddComponent<SpringJoint2D>();
                Spring.autoConfigureDistance = false;
                Spring.dampingRatio = dampFac;
                Spring.frequency = springConst;
                Spring.connectedBody = RigidBall;
                Spring.distance = Vector3.Distance(transform.parent.position, gooballs[i].transform.position);

                texture.LoadImage(image);
                Sprite SpritE = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                
                GameObject Strand = new GameObject();
                Strand.transform.SetParent(transform);
                Strand.AddComponent<SpriteRenderer>().sprite = spriteE;
                Strand.GetComponent<SpriteRenderer>().flipY = true;
                Strand.transform.localPosition = Vector3.zero;
                Strand.AddComponent<sTransformController>().connectedBall = gooballs[i];
                Strand.transform.GetComponentInParent<GooBallController>().isTower = true;
                Strand.transform.GetComponentInParent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                //Debug.Log("Created a spring! i: " + i + " rigidbody2d: " + RigidBall + " spring joint: " + Spring);
            }
        }
        //else Debug.Log("no gooballs to attatch to!");

    }
    public void AttatchStrandFromLevel(GameObject gb2)
    {

                

                Rigidbody2D RigidBall = gb2.GetComponent<Rigidbody2D>();
                SpringJoint2D Spring = transform.parent.gameObject.AddComponent<SpringJoint2D>();
                Spring.autoConfigureDistance = false;
                Spring.dampingRatio = dampFac;
                Spring.frequency = springConst;
                Spring.connectedBody = RigidBall;
                Spring.distance = Vector3.Distance(transform.parent.position, gb2.transform.position);

                texture.LoadImage(image);
                Sprite SpritE = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 1f));

                GameObject Strand = new GameObject();
                Strand.transform.SetParent(transform);
                Strand.AddComponent<SpriteRenderer>().sprite = spriteE;
                Strand.GetComponent<SpriteRenderer>().flipY = true;
                Strand.transform.localPosition = Vector3.zero;
                Strand.AddComponent<sTransformController>().connectedBall = gb2;
                Strand.transform.GetComponentInParent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

    }
}
