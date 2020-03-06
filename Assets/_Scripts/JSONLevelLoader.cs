using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

public class JSONLevelLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        JSONLevel level = new JSONLevel();
        pipe pyp = new pipe();
        pyp.depth = 0;
        level.pipe = pyp;
        level.pipe.id = "no_u";
        List<vertex> vertices = new List<vertex>();
        vertex vertex1 = new vertex();
        vertex1.x = 5;
        vertex1.y = 5;
        vertices.Add(vertex1);
        level.pipe.Vertex = vertices.ToArray();

        poi poid = new poi();
        poid.pos = new Vector2(5, 5);
        poid.pause = 0;
        poi[] pois = { poid };
        camera cam = new camera();
        cam.aspect = "widescreen";
        cam.poi = pois;
        level.camera = cam;

        string json = JsonUtility.ToJson(level, true);

        using (StreamWriter sw = File.CreateText("E:/text.json"))
        {
            sw.Write(json);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
