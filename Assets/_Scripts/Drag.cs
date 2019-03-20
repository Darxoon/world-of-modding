using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
    [SerializeField] private int ballY;

    [Header("Debugging and Reference")]

    [SerializeField] private bool isDragged = false;
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private AnimationCurve curve;

    #region update isDragged
    private void OnMouseDown() 
    {
        isDragged = true;
    }
    private void OnMouseUp() 
    {
        isDragged = false;
    }
    #endregion#


    private void FixedUpdate()
    {
        if(isDragged)
        {
            // calculate mouse pos
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = new Vector3(mousePos.x, mousePos.y, ballY);
            // get difference between mouse and ball 
            Vector3 difference = mousePos - transform.position;
            // apply that vector
            rigid.velocity += new Vector2(curve.Evaluate(difference.x), curve.Evaluate(difference.y)) * 2;
            
        }
    }

    

}
