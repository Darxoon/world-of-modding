using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLayerParallax : MonoBehaviour
{
    public float positiveDistanceScale; 
    public float negativeDistanceScale;
    GameManager gm;
    public float depth;
    
    public Vector2 worldPosition;

    private UnityEngine.Camera mainCam;

    private void Start()
    {
        mainCam = UnityEngine.Camera.main;
    }
    private void Awake(){
        gm = GameManager.instance;
    }

    private void Update()
    {
        Vector2 relativeWorldPosition = worldPosition - (Vector2)mainCam.transform.position;

        float distance;

        if (depth > 0)
            distance = 1000/depth*gm.positiveDistanceScale; //Mathf.Sqrt(depth) * gm.positiveDistanceScale * relativeWorldPosition.magnitude;
        else if (depth < 0)
            distance = 1000/depth*gm.negativeDistanceScale;  //(Mathf.Sqrt(-depth) + -depth) * gm.negativeDistanceScale * relativeWorldPosition.magnitude;
        else
            distance = 0;
        Vector2 offsettedRelativeWorldPosition = relativeWorldPosition.normalized * distance;

        //transform.position = offsettedRelativeWorldPosition + (Vector2)mainCam.transform.position;
        //transform.position = new Vector3(transform.position.x, transform.position.y, -depth / 100);
        Vector3 removedOffset = offsettedRelativeWorldPosition + worldPosition;
        transform.position = new Vector3(removedOffset.x, removedOffset.y, -depth/100);
        //Debug.Log($"{transform.position}, {worldPosition}");
    }
}
