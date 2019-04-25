using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    [SerializeField] Vector3 normalScale;
    [SerializeField] float hoveringMultiplicator;
    [SerializeField] float interpolationCoefficient;

    //private void Update()
    //{
    //    transform.localScale = Vector3.Lerp(transform.localScale, normalScale * hoveringMultiplicator, interpolationCoefficient);
    //    transform.localScale = Vector3.Lerp(transform.localScale, normalScale, interpolationCoefficient);
    //}

    private void OnGUI()
    {
        //Rect buttonRect = new Rect(button.x + window.x, button.y + window.y + 1, button.width, button.height); 

        //if (buttonRect.Contains(Event.current.mousePosition))
        //{
        //}
    }
}
