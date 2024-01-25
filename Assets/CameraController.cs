using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public int MoveThreshold = 100;
    public bool DebugDraw = false;
    Camera cam;
    private float left, right, top, bottom;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        //0,0 is bottom left
        UpdateCameraEdges();
        Vector3 screen = new Vector3(Screen.currentResolution.width, Screen.currentResolution.height);
        Vector3 mousepos = Input.mousePosition;
        if(mousepos.x < MoveThreshold || mousepos.x > screen.x-MoveThreshold ||
        mousepos.y < MoveThreshold || mousepos.y > screen.y-MoveThreshold){
            Vector3 dir = (mousepos-screen/2).normalized;
            if(left < GameManager.instance.currentLevel.scene.minX && dir.x < 0){
                dir.x = 0;
            }
            if(right > GameManager.instance.currentLevel.scene.maxX && dir.x > 0){
                dir.x = 0;
            }
            if(top > GameManager.instance.currentLevel.scene.maxY && dir.y > 0){
                dir.y = 0;
            }
            if(bottom < GameManager.instance.currentLevel.scene.minY && dir.y < 0){
                dir.y = 0;
            }
            Debug.Log(dir);
            transform.position += dir * Time.deltaTime;
        }
    }
    void UpdateCameraEdges(){
        Resolution res = Screen.currentResolution;
        left = cam.ScreenToWorldPoint(new Vector3(0,res.height/2,0)).x;
        right = cam.ScreenToWorldPoint(new Vector3(res.width,res.height/2,0)).x;
        top = cam.ScreenToWorldPoint(new Vector3(res.width/2,res.height,0)).y;
        bottom = cam.ScreenToWorldPoint(new Vector3(res.width/2,0,0)).y;
    }
}
