using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositeGeom : MonoBehaviour
{
    public Compositegeom data;
    void Start()
    {
        foreach(var geometry in data.geometries)
        {
            GameObject geom = new GameObject(geometry.id);
            geom.AddComponent<Geometry>().data = geometry;
            geom.transform.SetParent(transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.Euler(0, 0, transform.localRotation.eulerAngles.z + data.rotspeed);
    }
}
