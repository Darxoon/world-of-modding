using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MouseHover : MonoBehaviour
{
    Renderer render;

    void Start()
    {
        render = GetComponent<Renderer>();
        render.material.color = Color.white;
    }

    void OnMouseEnter()
    {
        render.material.color = Color.black;
    }

    void OnMouseExit()
    {
        render.material.color = Color.white;
    }
}