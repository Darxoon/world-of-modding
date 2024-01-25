using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcefieldEffector : MonoBehaviour
{
    private List<RadialForceFieldComponent> radffs = new List<RadialForceFieldComponent>();
    public Rigidbody2D rb;
    private Transform tr;
    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
    }

    public void OnTriggerEnter2D(Collider2D collision){
        switch(collision.tag){
            case "RadialForcefield":
                RadialForceFieldComponent radff = collision.GetComponent<RadialForceFieldComponent>();
                radffs.Add(radff);
                break;
        }
    }
    public void OnTriggerExit2D(Collider2D collision){
        switch(collision.tag){
            case "RadialForcefield":
                RadialForceFieldComponent radff = collision.GetComponent<RadialForceFieldComponent>();
                radffs.Remove(radff);
                break;
        }
    }
    public void FixedUpdate(){
        foreach(var radff in radffs){
            float fac = Vector2.Distance(radff.CenterPos, tr.position)/radff.data.radius;
            rb.AddForce((tr.position-radff.CenterPos).normalized * Mathf.Lerp(radff.data.forceatcenter, radff.data.forceatedge, fac)*2f, ForceMode2D.Force);
        }
    }
}
