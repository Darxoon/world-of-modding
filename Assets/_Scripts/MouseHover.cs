using System.Collections;
using System.Collections.Generic;
using UnityEngine;

void Start(){
	renderer.material.color = Color.white;
}

void OnMouseEnter(){
	renderer.material.color = Color.black;
}

void OnMouseExit() {
	renderer.material.color = Color.white;
}
