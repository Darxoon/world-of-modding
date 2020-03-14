using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLayerParallax : MonoBehaviour
{
    public float positiveDistanceScale; 
    public float negativeDistanceScale;

    public float depth;
    
    public Vector2 worldPosition;

    private UnityEngine.Camera mainCam;

    private void Start()
    {
        mainCam = UnityEngine.Camera.main;
    }

    private void Update()
    {
        Vector2 relativeWorldPosition = worldPosition - (Vector2)mainCam.transform.position;

        float distance;

        if (depth > 0)
            distance = Mathf.Sqrt(depth) * positiveDistanceScale * relativeWorldPosition.magnitude;
        else if (depth < 0)
            distance = (Mathf.Sqrt(-depth) + -depth) * negativeDistanceScale * relativeWorldPosition.magnitude;
        else
            distance = relativeWorldPosition.magnitude;
        Vector2 offsettedRelativeWorldPosition = relativeWorldPosition.normalized * distance;

        transform.position = offsettedRelativeWorldPosition + (Vector2)mainCam.transform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y, -depth / 10);

    }
}
