using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geometry : MonoBehaviour
{
    // Start is called before the first frame update

    public LevelGeometry data;
    Rigidbody2D dynamic;
    BoxCollider2D boxCollider;
    CircleCollider2D circleCollider;
    void Start()
    {
        transform.localPosition = data.center.ToVector2();
        if (data.dynamic)
        {
            dynamic = gameObject.AddComponent<Rigidbody2D>();
            dynamic.mass = data.mass;
        }
        if(data.type == LevelGeometry.Type.Rectangle)
        {
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
            boxCollider.size = data.size.ToVector2();
            boxCollider.transform.rotation = Quaternion.Euler(0, 0, data.rotation);
        }
        else if(data.type == LevelGeometry.Type.Circle)
        {
            circleCollider = gameObject.AddComponent<CircleCollider2D>();
            circleCollider.radius = data.radius;
        }
        if(data.image != null)
        {
            GameObject sl = new GameObject(data.image.name);
            sl.AddComponent<SceneLayer>().data = data.image;
            sl.transform.SetParent(transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + data.rotspeed);

#if DEBUG
        if (StaticData.levelLoader.DebugDraw)
        {
            if (data.type == LevelGeometry.Type.Rectangle)
            {
                Vector3 topRight = new Vector3(boxCollider.bounds.extents.x + transform.position.x, boxCollider.bounds.extents.y + transform.position.y, 0);
                Vector3 topLeft = new Vector3(-boxCollider.bounds.extents.x + transform.position.x, boxCollider.bounds.extents.y + transform.position.y, 0);
                Vector3 bottomRight = new Vector3(boxCollider.bounds.extents.x + transform.position.x, -boxCollider.bounds.extents.y + transform.position.y, 0);
                Vector3 bottomLeft = new Vector3(-boxCollider.bounds.extents.x + transform.position.x, -boxCollider.bounds.extents.y + transform.position.y, 0);
                Debug.DrawLine(topRight, topLeft, Color.green, 0, false); //top
                Debug.DrawLine(topRight, bottomRight, Color.green, 0, false); //right
                Debug.DrawLine(topLeft, bottomLeft, Color.green, 0, false); //left
                Debug.DrawLine(bottomLeft, bottomRight, Color.green, 0, false); //bottom    
            }
        }
#endif

    }
}
