using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHitbox : MonoBehaviour
{
    public bool hovering = false;

    private void OnMouseEnter()
    {
        hovering = true;
        ButtonPop.instance.Play();
    }

    private void OnMouseExit()
    {
        hovering = false;
    }


}
