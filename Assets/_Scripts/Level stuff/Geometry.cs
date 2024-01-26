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
    bool hasInitializedPositions = false;
    List<Vector2> positions = new List<Vector2>();
    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Geometry");
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
            var comp = sl.AddComponent<SceneLayer>();
            comp.data = data.image;
            comp.needsParallax = false;
            sl.transform.SetParent(transform);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localRotation = Quaternion.Euler(0, 0, transform.localRotation.eulerAngles.z + data.rotspeed);

#if DEBUG
        if (StaticData.levelLoader.visualdebug)
        {
            if (data.type == LevelGeometry.Type.Rectangle)
            {
                Vector3 topRight = new Vector3(boxCollider.bounds.extents.x + transform.position.x, boxCollider.bounds.extents.y + transform.position.y, 0);
                Vector3 topLeft = new Vector3(-boxCollider.bounds.extents.x + transform.position.x, boxCollider.bounds.extents.y + transform.position.y, 0);
                Vector3 bottomRight = new Vector3(boxCollider.bounds.extents.x + transform.position.x, -boxCollider.bounds.extents.y + transform.position.y, 0);
                Vector3 bottomLeft = new Vector3(-boxCollider.bounds.extents.x + transform.position.x, -boxCollider.bounds.extents.y + transform.position.y, 0);
                Debug.DrawLine(topRight, topLeft, Color.blue, 0, false); //top
                Debug.DrawLine(topRight, bottomRight, Color.blue, 0, false); //right
                Debug.DrawLine(topLeft, bottomLeft, Color.blue, 0, false); //left
                Debug.DrawLine(bottomLeft, bottomRight, Color.blue, 0, false); //bottom    
            }
            if (data.type == LevelGeometry.Type.Circle)
            {
                positions.Clear();
                float newX;
                float newY;
                float slice = 2 * Mathf.PI / 360;
                for (int i = 0; i < 360; i++)
                {
                    float angle = slice * i;
                    newX = transform.position.x + circleCollider.radius * Mathf.Cos(angle);
                    newY = transform.position.y + circleCollider.radius * Mathf.Sin(angle);
                    positions.Add(new Vector2(newX, newY));
                }

                for (int i = 0; i < positions.Count; i++)
                {
                    if(i != positions.Count-1)
                        Debug.DrawLine(positions[i], positions[i + 1], Color.cyan);
                    else
                        Debug.DrawLine(positions[i], positions[0], Color.cyan);
                }
            }
        }
#endif
    }
}
