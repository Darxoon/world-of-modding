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
        pipe pyp = new pipe();
        pyp.depth = 0;
        level.level.pipe = pyp;
        level.level.pipe.id = "no_u";
        List<vertex> vertices = new List<vertex>();
        vertex vertex1 = new vertex();
        vertex1.x = 5;
        vertex1.y = 5;
        vertices.Add(vertex1);
        level.level.pipe.Vertex = vertices.ToArray();

        poi poid = new poi();
        poid.pos = new position(5, 5);
        poid.pause = 0;
        poi[] pois = { poid };
        camera cam = new camera();
        cam.aspect = "widescreen";
        cam.poi = pois;
        level.level.camera = cam;

        linearforcefield gravity = new linearforcefield();
        gravity.force = new position(0,-10);
        linearforcefield[] ff = { gravity };
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
