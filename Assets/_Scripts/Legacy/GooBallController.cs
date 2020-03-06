using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GooBallController : MonoBehaviour
{
    string[] attenuationselect = "0.2,1,1.25".Split(',');
    string[] attenuationdeselect = "0.2,1.25,0.8,1".Split(',');
    string[] attenuationdrag = "0.2,1.75,1".Split(',');
    string[] attenuationdrop = "0.2,1,1".Split(',');
    bool draggable = true;
    public bool dragging;
    private Vector2 mousePosition;
    public float moveSpeed = 0.1f;
    public float maxLen2 = 0;
    public float maxLen1 = 0;
    public float minLen = 0;
    public string sImage = "";
    public string sInactiveImage = "";
    public bool isTower = false;
    public int Strands = 0;
    public float originalSX;
    public float originalSY;
    public GameObject Strandrange;
    public List<string> matches = new List<string>();
    public float dampFac = 0;
    public float springConst = 0;
    [SerializeField] private GameObject drag;
    [SerializeField] private LayerMask layerMask = (1 << 0) | (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4) | (1 << 5) | (1 << 6) | (1 << 7) | (1 << 8) | (0 << 9);

    [SerializeField] private GooBallController[] drags;

    public Texture2D texture;
    public byte[] image;
    public Sprite spriteE;




    public void passOnTextures(GameObject stnd)
    {
        StrandController strand = stnd.GetComponent<StrandController>();
        strand.texture = texture;
        strand.image = image;
        strand.spriteE = spriteE;
    }

    void Start()
    {
        GameObject StrandRange = new GameObject();
        Strandrange = StrandRange;
        StrandRange.name = "Strand Range";
        StrandRange.transform.SetParent(gameObject.transform);
        StrandRange.transform.localPosition = new Vector3(0, 0, 0);
        StrandRange.AddComponent<CircleCollider2D>().radius = maxLen2;
        StrandRange.GetComponent<CircleCollider2D>().isTrigger = true;
        StrandRange.AddComponent<StrandController>();
        originalSX = transform.localScale.x;
        originalSY = transform.localScale.y;
        StrandRange.layer = LayerMask.NameToLayer("Strand");
        drags = FindObjectsOfType<GooBallController>();
        StrandRange.GetComponent<StrandController>().StRands = Strands;
        StrandRange.GetComponent<StrandController>().dampFac = dampFac;
        StrandRange.GetComponent<StrandController>().springConst = springConst;
        StrandRange.transform.localPosition = new Vector3(0, 0, 1);
        passOnTextures(StrandRange);
    }

    private void Update()
    {
        foreach (var x in drags)
        {
            if (x != this)
                if (x.dragging)
                    return;
        }    
        if (Input.GetMouseButton(0))
        {
            var hits = Physics2D.GetRayIntersectionAll(Camera.main.ScreenPointToRay(Input.mousePosition), Mathf.Infinity, layerMask);
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    RaycastHit2D hit = hits[i];
                    Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);


                    if (hit.transform == transform)
                    {
                        dragging = true;
                        drag = hit.transform.gameObject;
                    }
                }
            }
        }

        if (drag != null && dragging && isTower == false)
        {
            //Vector2 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));

            this.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            mousePosition = Input.mousePosition;
            Vector2 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 3600f));
            //transform.position = Vector2.Lerp(transform.position, mousePosition, 0.1f);
            transform.position = new Vector3(point.x, point.y, 0);
            //Debug.Log("am dragging");
            Strandrange.GetComponent<StrandController>().isActive = true;


            if (Input.GetMouseButtonUp(0))
            {
                if (drag != null) { drag.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None; }
                dragging = false;
                drag = null;
                Debug.Log("stopped dragging");
                //this.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                if (isTower == false)
                {
                    Strandrange.GetComponent<StrandController>().AttatchStrand();

                }
            }
        }

        
    }
}
