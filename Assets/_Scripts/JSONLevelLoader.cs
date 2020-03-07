using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
public class JSONLevelLoader : MonoBehaviour
{
    // Start is called before the first frame update
    bool saveMode = false;
    void Start()
    {

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        JSONLevel level = JsonConvert.DeserializeObject<JSONLevel>(File.ReadAllText(StaticData.levelFolder + "demoLevel/demoLevel.json"), settings);

        Camera.main.backgroundColor = level.scene.backgroundcolor.ToUnityColor();

        foreach(var resource in level.resrc.resources)
        {
            StaticData.Resources.Add(resource.Key, resource.Value);
        }

        foreach (var scenelayer in level.scene.scenelayers)
        {
            GameObject sl = new GameObject(scenelayer.name);
            sl.AddComponent<SceneLayer>().data = scenelayer;
            sl.transform.SetParent(StaticData.sceneLayers.transform);
        }

        if (saveMode)
        {
            #region serialize

            JSONLevel levelL = new JSONLevel();

            Scenelayer[] layers = { new Scenelayer
        {
            image = "IMAGE_BACKGROUND",
            alpha = 1,
        }};

            Geometry[] geoms = { new Geometry
        {
            center = new Position(0, 0),
            id = "ground",
            size = new Position(1, 1),
            image = "IMAGE_BLACK",
            imagescale = new Position(2,2)
        }};
            levelL.scene.geometries = geoms;
            levelL.scene.scenelayers = layers;

            levelL.resrc.resources.Add("IMAGE_BACKGROUND", "levels/demoLevel/bg.png");
            levelL.resrc.resources.Add("IMAGE_BLACK", "levels/demoLevel/black.png");

            if (!Directory.Exists(StaticData.levelFolder + "demoLevel/"))
                Directory.CreateDirectory(StaticData.levelFolder + "demoLevel/");
            using (StreamWriter sw = File.CreateText(StaticData.levelFolder + "demoLevel/demoLevel.json"))
                sw.Write(JsonConvert.SerializeObject(levelL, settings));


            #endregion
        }
    }
}
