using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
public class JSONLevelLoader : MonoBehaviour
{
    // Start is called before the first frame update
    public bool saveMode = false;
    public bool DebugDraw;
    void Start()
    {
        StaticData.levelLoader = this;
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        if (saveMode)
        {
            #region serialize

            JSONLevel levelL = new JSONLevel();

            Scenelayer[] layers = { new Scenelayer
        {
            image = "IMAGE_BACKGROUND",
            alpha = 1,
        }};

            LevelGeometry[] geoms = { new LevelGeometry
        {
            center = new Position(-3, 0),
            id = "ground",
            size = new Position(10, 1),
            image = new Scenelayer{ 
                image = "IMAGE_BLOCK",
                scaleX = 2,
                scaleY = 2,
            },
            //imagescale = new Position(2,2),
            type = LevelGeometry.Type.Rectangle
        }};


            LevelGeometry[] cgeoms = { new LevelGeometry
        {
            center = new Position(0, 0),
            id = "bottomL",
            size = new Position(10, 1),
            type = LevelGeometry.Type.Rectangle
            },
            new LevelGeometry
        {
            center = new Position(0, 0),
            id = "topL",
            size = new Position(10, 1),
            type = LevelGeometry.Type.Rectangle,
            rotation = 90
            }};


            Compositegeom[] compositegeoms =
            {
                new Compositegeom
                {
                    geometries = cgeoms,
                    name = "hi",
                    position = new Position(1,1)
                }
            };
                
            Ballinstance[] instances =
            {
                new Ballinstance
                {
                    discovered = true,
                    type = "common",
                    id = "1",
                    pos = new Position(5,5)
                },
                new Ballinstance
                {
                    discovered = true,
                    type = "common",
                    id = "1",
                    pos = new Position(5,5)
                },
                new Ballinstance
                {
                    discovered = true,
                    type = "common",
                    id = "1",
                    pos = new Position(5,5)
                },
                new Ballinstance
                {
                    discovered = true,
                    type = "common",
                    id = "1",
                    pos = new Position(5,5)
                }
            };

            levelL.level.BallInstance = instances;
            levelL.scene.compositegeoms = compositegeoms;
            levelL.scene.geometries = geoms;
            levelL.scene.scenelayers = layers;

            levelL.resrc.resources.Add("IMAGE_BACKGROUND", "levels/demoLevel/bg.png");
            levelL.resrc.resources.Add("IMAGE_BLACK", "levels/demoLevel/black.png");

            if (!Directory.Exists(StaticData.levelFolder + "demoLevel/"))
                Directory.CreateDirectory(StaticData.levelFolder + "demoLevel/");
            using (StreamWriter sw = File.CreateText(StaticData.levelFolder + "demoLevel/demoLevel.json"))
                sw.Write(JsonConvert.SerializeObject(levelL, settings));

            Debug.Break();

            

            #endregion
        }
        else
        {


            JSONLevel level = JsonConvert.DeserializeObject<JSONLevel>(File.ReadAllText(StaticData.levelFolder + "demoLevel/demoLevel.json"), settings);

            if (level.DebugDraw)
                DebugDraw = true;

            Camera.main.backgroundColor = level.scene.backgroundcolor.ToUnityColor();

            foreach (var resource in level.resrc.resources)
            {
                StaticData.ResourcePaths.Add(resource.Key, resource.Value);
            }

            foreach (var scenelayer in level.scene.scenelayers)
            {
                GameObject sl = new GameObject(scenelayer.name);
                sl.AddComponent<SceneLayer>().data = scenelayer;
                sl.transform.SetParent(StaticData.sceneLayers.transform);
            }

            foreach (var geometry in level.scene.geometries)
            {
                GameObject geom = new GameObject(geometry.id);
                geom.AddComponent<Geometry>().data = geometry;
                geom.transform.SetParent(StaticData.geometry.transform);
            }

            foreach (var compositegeom in level.scene.compositegeoms)
            {
                GameObject geom = new GameObject(compositegeom.name);
                geom.AddComponent<CompositeGeom>().data = compositegeom;
                geom.transform.SetParent(StaticData.geometry.transform);
            }
            foreach(var gball in level.level.BallInstance)
            {
                
            }
        }
    }
}
