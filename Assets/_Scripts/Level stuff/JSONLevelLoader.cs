using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Threading.Tasks;

public class JSONLevelLoader : MonoBehaviour
{
    // Start is called before the first frame update
    public enum SaveMode
    {
        Level,Gooball,None
    }
    public SaveMode saveMode = SaveMode.None;
    public bool DebugDraw;
    void Start()
    {
        StaticData.levelLoader = this;
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        if (saveMode == SaveMode.Level)
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
                image = "IMAGE_BLACK",
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
                    pos = new Position(4,5)
                },
                new Ballinstance
                {
                    discovered = true,
                    type = "common",
                    id = "2",
                    pos = new Position(5,5)
                },
                new Ballinstance
                {
                    discovered = true,
                    type = "common",
                    id = "3",
                    pos = new Position(6,5)
                },
                new Ballinstance
                {
                    discovered = true,
                    type = "common",
                    id = "4",
                    pos = new Position(7,5)
                }
            };

            levelL.level.BallInstance = instances;
            levelL.scene.compositegeoms = compositegeoms;
            levelL.scene.geometries = geoms;
            levelL.scene.scenelayers = layers;
            levelL.level.Strand = new LStrand[]
            {
                new LStrand
                {
                    gb1 = "1",
                    gb2 = "2"
                }
            };
            
            levelL.resrc.resources.Add("IMAGE_BACKGROUND", "levels/demoLevel/bg.png");
            levelL.resrc.resources.Add("IMAGE_BLACK", "levels/demoLevel/black.png");

            if (!Directory.Exists(StaticData.levelFolder + "demoLevel/"))
                Directory.CreateDirectory(StaticData.levelFolder + "demoLevel/");
            using (StreamWriter sw = File.CreateText(StaticData.levelFolder + "demoLevel/demoLevel.json"))
                sw.Write(JsonConvert.SerializeObject(levelL, settings));

            Debug.Break();

            

            #endregion
        }
        else if(saveMode == SaveMode.Gooball)
        {
            JSONGooball gooball = new JSONGooball();
            gooball.ball.name = "common";
            gooball.ball.radius = 2.2f;
            gooball.ball.walkSpeed = 1;
            gooball.ball.mass = 3.23f;
            gooball.ball.strands = 2;
            gooball.ball.climber = true;
            gooball.ball.dynamic = true;
            gooball.ball.draggable = true;
            gooball.ball.towerMass = 3;
            gooball.resrc.resources.Add("body", "balls/common/body.png");
            gooball.resrc.resources.Add("strand", "balls/common/strand.png");
            Part[] pts =
            {
                new Part
                {
                    image = new string[]{"body"},
                    name = "body",
                }
            };
            gooball.parts = pts;
            gooball.strand = new StrandJSON
            {
                image = "strand",
            };
            if (!Directory.Exists(StaticData.ballsFolder + "common/"))
                Directory.CreateDirectory(StaticData.ballsFolder+ "common/");
            using (StreamWriter sw = File.CreateText(StaticData.ballsFolder + "common/common.json"))
                sw.Write(JsonConvert.SerializeObject(gooball, settings));
            Debug.Break();
        }
        else if(saveMode == SaveMode.None)
        {


            JSONLevel level = JsonConvert.DeserializeObject<JSONLevel>(File.ReadAllText(StaticData.levelFolder + "demoLevel/demoLevel.json"), settings);

            if (level.DebugDraw)
                DebugDraw = true;

            Camera.main.backgroundColor = level.scene.backgroundcolor.ToUnityColor();

            foreach (var resource in level.resrc.resources)
            {
                GameManager.ResourcePaths.Add(resource.Key, resource.Value);
                
            }
            foreach (var gball in level.level.BallInstance)
            {
                if (!GameManager.memoryGooballs.ContainsKey(gball.type))
                {
                    JSONGooball ball = StaticData.RetrieveGooballDataFromType(gball.type);
                    GameManager.memoryGooballs.Add(gball.type, ball);
                    foreach (var path in ball.resrc.resources)
                        if (!GameManager.ResourcePaths.ContainsKey(path.Key))
                            GameManager.ResourcePaths.Add(path.Key, path.Value);
                }
            }
            StaticData.PopulateAllResources();
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
            foreach (var gball in level.level.BallInstance)
            {
                JSONGooball ball = null;
                GameManager.memoryGooballs.TryGetValue(gball.type, out ball);
                GameObject gooball = new GameObject(gball.id);

                gooball.transform.SetParent(StaticData.balls.transform);
                //gooball.transform.position   = new Vector3(gball.pos.x, gball.pos.y, 0);
                //gooball.AddComponent<Rigidbody2D>();
                //gooball.AddComponent<Walk>();
                var g = gooball.AddComponent<Gooball>();
                g.data = ball;
                g.gooballPosition = new Vector3(gball.pos.x, gball.pos.y, 0);
                gooball.layer = LayerMask.NameToLayer("Detached Balls");
            }

            foreach(LStrand strand in level.level.Strand)
            {
                StartCoroutine(MakeStrand(strand));
            }

        }
    }

    IEnumerator MakeStrand(LStrand strand)
    {
        Gooball ball1 = StaticData.balls.transform.Find(strand.gb1).GetComponent<Gooball>();
        Gooball ball2 = StaticData.balls.transform.Find(strand.gb2).GetComponent<Gooball>();
        while (!ball1.finishedLoading)
        {
            yield return null;
        }
        while (!ball2.finishedLoading)
        {
            yield return null;
        }
        ball1.MakeStrand(ball2);
        ball1.SetTowered();
        ball2.SetTowered();
    }
}
