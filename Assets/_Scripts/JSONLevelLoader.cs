using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
public class JSONLevelLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        JSONLevel level = new JSONLevel();
        Pipe pyp = new Pipe
        {
            depth = 0
        };
        level.level.pipe = pyp;
        level.level.pipe.id = "no_u";
        List<Vertex> vertices = new List<Vertex>();
        Vertex vertex1 = new Vertex
        {
            x = 5,
            y = 5
        };
        vertices.Add(vertex1);
        level.level.pipe.Vertex = vertices.ToArray();

        Poi poid = new Poi
        {
            pos = new Position(5, 5),
            pause = 0
        };
        Poi[] pois = { poid };
        Camera cam = new Camera
        {
            aspect = "widescreen",
            poi = pois
        };
        level.level.camera = cam;

        LinearForceField gravity = new LinearForceField
        {
            force = new Position(0, -10)
        };

        LinearForceField[] ff = { gravity };
        level.scene.ForceFields.linearforcefields = ff;
        /*
        _ = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        */




    }
}
