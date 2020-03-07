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
        Pipe pyp = new Pipe();
        pyp.depth = 0;
        level.level.pipe = pyp;
        level.level.pipe.id = "no_u";
        List<Vertex> vertices = new List<Vertex>();
        Vertex vertex1 = new Vertex();
        vertex1.x = 5;
        vertex1.y = 5;
        vertices.Add(vertex1);
        level.level.pipe.Vertex = vertices.ToArray();

        Poi poid = new Poi();
        poid.pos = new Position(5, 5);
        poid.pause = 0;
        Poi[] pois = { poid };
        Camera cam = new Camera();
        cam.aspect = "widescreen";
        cam.poi = pois;
        level.level.camera = cam;

        LinearForceField gravity = new LinearForceField();
        gravity.force = new Position(0,-10);
        LinearForceField[] ff = { gravity };
        level.scene.ForceFields.linearforcefields = ff;

        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.NullValueHandling = NullValueHandling.Ignore;

        /*
        string json = JsonConvert.SerializeObject(level, Formatting.Indented, settings);
        using (StreamWriter sw = File.CreateText("E:/text.json"))
        {
            sw.Write(json);
        }
        */

        string json = File.ReadAllText("E:/text.json");

        JSONLevel deserialized = JsonConvert.DeserializeObject<JSONLevel>(json);
        Debug.Log(deserialized.level);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
