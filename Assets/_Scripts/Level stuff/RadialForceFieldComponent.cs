using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialForceFieldComponent : MonoBehaviour
{
    public RadialForceField data;
    private CircleCollider2D area;
    public Vector3 CenterPos {get; private set;}
    // Start is called before the first frame update
    void Start()
    {
        transform.position = data.center.ToVector2();
        area = gameObject.AddComponent<CircleCollider2D>();
        area.isTrigger = true;
        area.radius = data.radius;
        gameObject.SetActive(data.enabled);
        CenterPos = data.center.ToVector2();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
