using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanController : MonoBehaviour
{
    public bool isAllowedToMove = false;
    public List<string> matches = new List<string>();
    public float maxX;
    public float minX;
    public float maxY;
    public float minY;

    Vector3 corner1 = Vector3.zero;
    Vector3 corner2 = Vector3.zero;
    Vector3 corner3 = Vector3.zero;
    Vector3 corner4 = Vector3.zero;

    public Vector3[] frustumcorners = new Vector3[4];
    
    public void MyOnMouseEnter()
    {
        //Debug.Log("this thing wont move");
        isAllowedToMove = false;
    }
    private void MyOnMouseExit()
    {
        //Debug.Log("this thing can move");
        //isAllowedToMove = true; //supposed to be true
    }

    private Transform parent;
    private Camera cam;

    private void Start()
    {
        parent = transform.parent;
        cam = parent.GetComponent<Camera>();
    }

    public void Update()
    {

        if (isAllowedToMove == true)
        {

            //Debug.Log("attempting movement");
            Vector3 pos = Input.mousePosition;
            pos.z = cam.nearClipPlane;
            pos.z = parent.position.z;
            pos = cam.ScreenToWorldPoint(pos);
            pos.z = -3600;
            //Debug.Log("Screen to world point is: " + pos);

            var deltaTime = 1.0f * Time.deltaTime;
            deltaTime *= Vector3.Distance(parent.position, pos);

            /*Vector3 buffer = Vector3.MoveTowards(parent.position, pos, deltaTime);
            if (buffer.x <= maxX)
                parent.position = Vector3.MoveTowards(parent.position, pos, deltaTime);
            if (buffer.x > maxX)
                parent.position = new Vector3(maxX, buffer.y, buffer.z);*/

            Camera.main.CalculateFrustumCorners(new Rect(0, 0, 1, 1), 3600, Camera.MonoOrStereoscopicEye.Mono, frustumcorners); 

            parent.position = new Vector3(
                Mathf.Clamp(Vector3.MoveTowards(parent.position, pos, deltaTime).x, minX, maxX),
                Mathf.Clamp(Vector3.MoveTowards(parent.position, pos, deltaTime).y, minY, maxY),
                Vector3.MoveTowards(parent.position, pos, deltaTime).z
                );

            //Vector3.MoveTowards(parent.position, pos, deltaTime)

            Transform bcTransform = this.transform.parent;
            Vector3 worldPosition = bcTransform.TransformPoint(0, 0, 0);

            BoxCollider2D box = parent.transform.GetChild(1).GetComponent<BoxCollider2D>();
            Vector2 size = new Vector2(box.size.x, box.size.y);
            corner1 = new Vector2(-size.x, -size.y);
            corner2 = new Vector2(-size.x, size.y);
            corner3 = new Vector2(size.x, -size.y);
            corner4 = new Vector2(size.x, size.y);
            corner1 = worldPosition + corner1;
            corner2 = worldPosition + corner2;
            corner3 = worldPosition + corner3;
            corner4 = worldPosition + corner4;
            //Debug.Log(corner1 + " " + corner2 + " " + corner3 + " " + corner4);

            float distancetosideedge = bcTransform.position.x - size.x;
            Debug.Log(distancetosideedge);

            var frustumHeight = 2.0f * 3600 * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
            var frustumWidth = frustumHeight * Camera.main.aspect;
            frustumWidth = frustumWidth / 2;
            frustumHeight = frustumHeight / 2;
            Debug.Log(frustumHeight + " " + frustumWidth);
        }
        Camera.main.CalculateFrustumCorners(new Rect(0, 0, 1, 1), 3600, Camera.MonoOrStereoscopicEye.Mono, frustumcorners);

        for (int i = 0; i < 4; i++)
        {
            var worldSpaceCorner = Camera.main.transform.TransformVector(frustumcorners[0]);
            Debug.DrawRay(Camera.main.transform.position, worldSpaceCorner, Color.blue);
        }
        for (int i = 0; i < 4; i++)
        {
            var worldSpaceCorner = Camera.main.transform.TransformVector(frustumcorners[1]);
            Debug.DrawRay(Camera.main.transform.position, worldSpaceCorner, Color.green);
        }
        for (int i = 0; i < 4; i++)
        {
            var worldSpaceCorner = Camera.main.transform.TransformVector(frustumcorners[2]);
            Debug.DrawRay(Camera.main.transform.position, worldSpaceCorner, Color.yellow);
        }
        for (int i = 0; i < 4; i++)
        {
            var worldSpaceCorner = Camera.main.transform.TransformVector(frustumcorners[3]);
            Debug.DrawRay(Camera.main.transform.position, worldSpaceCorner, Color.magenta);
        }

        RaycastHit2D[] hits;

        hits = Physics2D.GetRayIntersectionAll(Camera.main.ScreenPointToRay(Input.mousePosition), Mathf.Infinity);
        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit2D hit = hits[i];
                Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);
                if (hit.transform == transform)
                {
                    matches.Add(i.ToString());
                }
            }
        }
        if (matches.Count != 0)
            MyOnMouseEnter();
        else MyOnMouseExit();
        matches.Clear();

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), Mathf.Infinity);
            if (hit.collider != null)
            {
                Debug.Log(hit.collider.transform.name);
            }
        }
    }
}
