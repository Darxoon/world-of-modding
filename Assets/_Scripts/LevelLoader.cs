using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Xml;
using System.Globalization;
using System.Linq;


public class LevelLoader : MonoBehaviour
{
    #region Classes
    [Serializable]
    public struct SceneInfo
    {
        public float minX;
        public float minY;
        public float maxX;
        public float maxY;
        public UnityEngine.Color backgroundColor;

        public SceneInfo(string minX, string minY, string maxX, string maxY, string bgColor)
        {
            this.minX = float.Parse(minX);
            this.minY = float.Parse(minY);
            this.maxX = float.Parse(maxX);
            this.maxY = float.Parse(maxY);
            string[] colors = bgColor.Split(',');
            backgroundColor = new UnityEngine.Color(
                float.Parse(colors[0]) / 255,
                float.Parse(colors[1]) / 255,
                float.Parse(colors[2]) / 255);
        }
    }
    [Serializable] 
    public struct SceneLayer
    {
        public float depth;
        public Vector2 pos; 
        public Vector2 scale;
        public float rotation;
        public UnityEngine.Color colorize;
        public string image;
        public string name;

        public SceneLayer(string depth, string x, string y, string scaleX, string scaleY, string rotation, string alpha, string colorize, string image, string name)
        {
            this.depth = float.Parse(depth);
            pos = new Vector2(float.Parse(x), float.Parse(y));
            scale = new Vector2(float.Parse(scaleX), float.Parse(scaleY));
            string[] colors = colorize.Split(',');
            this.colorize = new UnityEngine.Color(
                float.Parse(colors[0]) / 255,
                float.Parse(colors[1]) / 255,
                float.Parse(colors[2]) / 255, 
                float.Parse(alpha));
            this.rotation = float.Parse(rotation);
            this.image = image;
            this.name = name;
        }
    }
    #endregion

    [Header("File reading")]

    public string resDirectory; 
    public string levelDirectory; 
    public string levelToLoad;

    [Header("Scene loading")]

    [SerializeField] private SceneInfo sceneInfo;
    [SerializeField] private SceneLayer[] sceneLayers;

    [SerializeField] private Vector2 fixedScreenSize;

    [SerializeField] private GameObject sceneLayerPrefab;
    [SerializeField] private GameObject ballPrefab; //ideally would cache all balls on startup

    [Header("Positioning fine tuning")]

    [SerializeField] private float scale;
    [SerializeField] private float positiveDistanceScale;
    [SerializeField] private float negativeDistanceScale;

    [Header("References")]

    [SerializeField] private Transform sceneLayerGroup;
    [SerializeField] private Transform ballGroup;
    [SerializeField] private Transform geometryGroup;

    // references that can be guessed automatically 

    private UnityEngine.Camera mainCam;

    // resources 

    private Dictionary<string, Sprite>      imgResources;
    private Dictionary<string, AudioClip>   soundResources;
    private Dictionary<string, string>      textResources;

    private void Start()
    {
        // references 

        mainCam = UnityEngine.Camera.main;

        // load the files
        try
        {
            XmlDocument level = new XmlDocument();
            Debug.Log($"{Path.Combine(resDirectory, "res", levelDirectory, levelToLoad, levelToLoad + ".level")}");
            XmlTextReader levelReader = new XmlTextReader(Path.Combine(resDirectory, "res", levelDirectory, levelToLoad, levelToLoad + ".level"));
            levelReader.Read();
            level.Load(levelReader);

            Debug.LogWarning("loading .level");
            LoadLevel(level);

            XmlDocument resrc = new XmlDocument();
            Debug.Log($"{Path.Combine(resDirectory, "res", levelDirectory, levelToLoad, levelToLoad + ".resrc")}");
            XmlTextReader resrcReader = new XmlTextReader(Path.Combine(resDirectory, "res", levelDirectory, levelToLoad, levelToLoad + ".resrc"));
            resrcReader.Read();
            resrc.Load(resrcReader);

            Debug.LogWarning("loading .resrc");
            LoadResrc(resrc);

            XmlDocument scene = new XmlDocument();
            XmlTextReader sceneReader = new XmlTextReader(Path.Combine(resDirectory, "res", levelDirectory, levelToLoad, levelToLoad + ".scene"));
            sceneReader.Read();
            scene.Load(sceneReader);

            Debug.LogWarning("loading .scene");
            LoadScene(scene);

            //Path.Combine(resDirectory, levelDirectory, levelToLoad, levelToLoad + ".scene
        }
        catch (IOException e)
        {
            Debug.Log("The level couldn't be loaded... ");
            Debug.Log(e.Message);
            return;
        } 
    }

    private void LoadLevel(XmlDocument level)
    {
        // ------ load camera info ------
        XmlNodeList cameras = level.SelectNodes("/level/camera");
        foreach(XmlNode camera in cameras){
            if(camera.Attributes["aspect"].Value == "widescreen"){
                Camera mainCam = UnityEngine.Camera.main;
                mainCam.orthographicSize = float.Parse(camera.Attributes["endzoom"].Value, CultureInfo.InvariantCulture)*1.7f;
                string[] endpos = camera.Attributes["endpos"].Value.Split(",");
                Vector2 pos = new Vector2(float.Parse(endpos[0], CultureInfo.InvariantCulture), float.Parse(endpos[1], CultureInfo.InvariantCulture))
                / 15 * scale / fixedScreenSize;
                mainCam.transform.position = new Vector3(pos.x, pos.y, -100);
                break;
            }
        }
        // ------ load balls -----------
        XmlNodeList balls = level.SelectNodes("/level/BallInstance");
        foreach(XmlNode ball in balls){
            string ballType = ball.Attributes["type"].Value;
            string x = ball.Attributes["x"].Value;
            string y = ball.Attributes["y"].Value;
            string id = ball.Attributes["id"].Value;
            GameObject scnBall = Instantiate(ballPrefab, ballGroup);
            var data = new JSONGooball();
            data.ball.towerMass = 0.7f;
            data.ball.mass = 3;
            data.strand.image = "balls/common/strand.png";
            scnBall.GetComponent<Gooball>().data = data;
            float scaleDiv = 100f;
            scnBall.transform.position = new Vector3(float.Parse(x, CultureInfo.InvariantCulture) / scaleDiv, float.Parse(y, CultureInfo.InvariantCulture) / scaleDiv);
            scnBall.name = id;
        }
        // -------- load strands ----------
        XmlNodeList strands = level.SelectNodes("/level/Strand");
        foreach(XmlNode strand in strands){
            Gooball gb1 = ballGroup.Find(strand.Attributes["gb1"].Value).GetComponent<Gooball>();
            Gooball gb2 = ballGroup.Find(strand.Attributes["gb2"].Value).GetComponent<Gooball>();
            List<Gooball> strs = new List<Gooball>(gb1.initialStrands)
            {
                gb2
            };
            gb1.initialStrands = strs.ToArray();
        }
    }

    private void LoadResrc(XmlDocument resrc)
    {
        // ------ load images ----------

        // get resources from xml
        XmlNodeList imageResources = resrc.SelectNodes("/Resources/Image");
        Dictionary<string, string> resrcFilePaths = new Dictionary<string, string>();

        foreach (XmlNode item in imageResources)
        {
            resrcFilePaths.Add(item.Attributes["id"].Value, item.Attributes["path"].Value);
        }

        // make sprites from files
        imgResources = new Dictionary<string, Sprite>();
        Dictionary<string, Texture2D> textureResources = new Dictionary<string, Texture2D>();
        foreach (KeyValuePair<string, string> item in resrcFilePaths)
        {
            // load texture from byte array
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(File.ReadAllBytes(Path.Combine(resDirectory, item.Value + ".png")));
            // make sprite
            imgResources[item.Key] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(.5f, .5f)); 
            // debug
            Debug.Log(item.Key);
        }
        

        //// load sounds
        //XmlNodeList sndResources = resrc.SelectNodes("/Resources/Sound");
        //resrcFilePaths = new Dictionary<string, string>();

        //foreach (XmlNode item in sndResources)
        //{
        //    resrcFilePaths.Add(item.Attributes["id"].Value, item.Attributes["path"].Value);
        //}

        //soundResources = new Dictionary<string, AudioClip>();

        //foreach (KeyValuePair<string, string> item in resrcFilePaths)
        //{

        //}
    }
    private void CreateBlackBar(Vector2 pivot, Vector3 offset){
        GameObject leftBar = new GameObject("BlackBar");
        SpriteRenderer leftRenderer = leftBar.AddComponent<SpriteRenderer>();
        Texture2D texture = new Texture2D(100, 1000);
        for(int i = 0; i < texture.width; i++)
            for(int j = 0; j < texture.height; j++)
                texture.SetPixel(i, j, new Color(0,0,0));
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        leftBar.transform.position = offset;
        leftBar.transform.localScale = new Vector3(100, 100, 1);
        leftRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), pivot);
        leftRenderer.color = new Color(0,0,0);
        leftRenderer.sortingOrder = 1000;
    }
    private void LoadScene(XmlDocument scene)
    {
        XmlNode sceneAttributes = scene.SelectSingleNode("/scene");

        sceneInfo = new SceneInfo(
            sceneAttributes.Attributes["minx"].Value,
            sceneAttributes.Attributes["miny"].Value,
            sceneAttributes.Attributes["maxx"].Value,
            sceneAttributes.Attributes["maxy"].Value,
            sceneAttributes.Attributes["backgroundcolor"].Value);

        //set up black bars
        float minXOffset = sceneInfo.minX / 12 * scale / fixedScreenSize.x;
        float minYOffset = sceneInfo.minY / 12 * scale / fixedScreenSize.y;
        float maxXOffset = sceneInfo.maxX / 12 * scale / fixedScreenSize.x;
        float maxYOffset = sceneInfo.maxY / 12 * scale / fixedScreenSize.y;
        CreateBlackBar(new Vector2(1f, 0.5f), new Vector3(minXOffset, 0));
        CreateBlackBar(new Vector2(0f, 0.5f), new Vector3(maxXOffset, 0));
        CreateBlackBar(new Vector2(0.5f, 1f), new Vector3(0, minYOffset));
        CreateBlackBar(new Vector2(0.5f, 0f), new Vector3(0, maxYOffset));

        // scene layers
        XmlNodeList sceneLayerNodes = scene.SelectNodes("/scene/SceneLayer");
        List<SceneLayer> sceneLayers = new List<SceneLayer>();
        foreach (XmlNode item in sceneLayerNodes)
        {
            string[] colors = item.Attributes["colorize"].Value.Split(',');
            SceneLayer l = new SceneLayer
            {
                depth = float.Parse(item.Attributes["depth"].Value, CultureInfo.InvariantCulture),
                pos = new Vector2(float.Parse(item.Attributes["x"].Value, CultureInfo.InvariantCulture), float.Parse(item.Attributes["y"].Value, CultureInfo.InvariantCulture)),
                scale = new Vector2(float.Parse(item.Attributes["scalex"].Value, CultureInfo.InvariantCulture), float.Parse(item.Attributes["scaley"].Value, CultureInfo.InvariantCulture)),
                rotation = float.Parse(item.Attributes["rotation"].Value, CultureInfo.InvariantCulture),
                colorize = new UnityEngine.Color(
                    float.Parse(colors[0], CultureInfo.InvariantCulture) / 255,
                    float.Parse(colors[1], CultureInfo.InvariantCulture) / 255,
                    float.Parse(colors[2], CultureInfo.InvariantCulture) / 255,
                    float.Parse(item.Attributes["alpha"].Value, CultureInfo.InvariantCulture)),
                image = item.Attributes["image"].Value,
                name = item.Attributes["name"].Value
            };
            Console.WriteLine("----------------" + l.scale);
            sceneLayers.Add(l);
        }
        this.sceneLayers = sceneLayers.ToArray();

        foreach (SceneLayer item in sceneLayers)
        {
            GameObject sceneLayer = Instantiate(sceneLayerPrefab, sceneLayerGroup);
            // position

            Vector2 worldPosition = item.pos / 15 * scale / fixedScreenSize;
            Vector2 relativeWorldPosition = worldPosition - (Vector2)mainCam.transform.position;

            float distance;

            if (item.depth > 0)
                distance = Mathf.Sqrt(item.depth) * positiveDistanceScale * relativeWorldPosition.magnitude;
            else if (item.depth < 0)
                distance = (Mathf.Sqrt(-item.depth) + -item.depth) * negativeDistanceScale * relativeWorldPosition.magnitude;
            else
                distance = relativeWorldPosition.magnitude;
            Vector2 offsettedRelativeWorldPosition = relativeWorldPosition.normalized * distance;

            //sceneLayer.transform.position = offsettedRelativeWorldPosition + (Vector2)mainCam.transform.position;
            //sceneLayer.transform.position = new Vector3(sceneLayer.transform.position.x, sceneLayer.transform.position.y, Math.Abs(item.depth / 100));
            
            sceneLayer.transform.position = new Vector3(worldPosition.x, worldPosition.y, -item.depth / 100);

            // rotation 

            sceneLayer.transform.rotation = Quaternion.Euler(0f, 0f, item.rotation);

            // scale 
            sceneLayer.transform.localScale = item.scale * scale;
            Debug.Log(item.image);
            sceneLayer.GetComponent<SpriteRenderer>().sprite = imgResources[item.image];

            // name 
            sceneLayer.name = $"SceneLayer '{item.name}'";

            // parallax component 

            sceneLayer.AddComponent(typeof(SceneLayerParallax));
            SceneLayerParallax parallaxComponent = sceneLayer.GetComponent<SceneLayerParallax>();
            parallaxComponent.positiveDistanceScale = positiveDistanceScale;
            parallaxComponent.negativeDistanceScale = negativeDistanceScale;
            parallaxComponent.depth = item.depth;
            parallaxComponent.worldPosition = sceneLayer.transform.position; 

        }

        XmlNodeList circles = scene.SelectNodes("/scene/circle");
        foreach(XmlNode circle in circles){
            string id = circle.Attributes["id"].Value;
            bool staticval = bool.Parse(circle.Attributes["static"].Value);
            string tag = circle.Attributes["tag"].Value;
            string material = circle.Attributes["material"].Value;
            float x = float.Parse(circle.Attributes["x"].Value, CultureInfo.InvariantCulture) / 15 * scale / fixedScreenSize.x;
            float y = float.Parse(circle.Attributes["y"].Value, CultureInfo.InvariantCulture) / 15 * scale / fixedScreenSize.y;
            Debug.Log(y);
            float radius = float.Parse(circle.Attributes["radius"].Value, CultureInfo.InvariantCulture) / 15 * scale / fixedScreenSize.x;
            GameObject obj = new GameObject(id);
            if(staticval){
                CircleCollider2D col = obj.AddComponent<CircleCollider2D>();
                col.radius = radius;
            }
            obj.transform.position = new Vector3(x, y, 0);
            obj.transform.SetParent(geometryGroup);
        }
        XmlNodeList lines = scene.SelectNodes("/scene/line");
        foreach(XmlNode line in lines){
            string id = line.Attributes["id"].Value;
            bool staticval = bool.Parse(line.Attributes["static"].Value);
            try{string tag = line.Attributes["tag"].Value;            } catch{}
            string material = line.Attributes["material"].Value;
            string[] anchor = line.Attributes["anchor"].Value.Split(",");
            float x = float.Parse(anchor[0], CultureInfo.InvariantCulture) / 14 * scale / fixedScreenSize.x;
            float y = float.Parse(anchor[1], CultureInfo.InvariantCulture) / 14 * scale / fixedScreenSize.y;
            string[] normal = line.Attributes["normal"].Value.Split(",");
            float nx = float.Parse(normal[0], CultureInfo.InvariantCulture);
            float ny = float.Parse(normal[1], CultureInfo.InvariantCulture);
            Vector2 normalvec = new Vector2(MathF.Round(nx,1), MathF.Round(ny,1));
            float rotRadians = Vector2.Dot(normalvec.normalized, Vector2.up);
            GameObject obj = new GameObject(id);
            if(normalvec.x > 0)
                obj.transform.eulerAngles = new Vector3(0, 0, (float)(Math.Acos(rotRadians)*180/Math.PI) - 90f);
            else
                obj.transform.eulerAngles = new Vector3(0, 0, (float)(Math.Acos(rotRadians)*180/Math.PI) + 90f);
            Debug.Log($"{rotRadians} - {Math.Acos(rotRadians)} - {Math.Acos(rotRadians)*180/Math.PI}");
            if(staticval){
                BoxCollider2D col = obj.AddComponent<BoxCollider2D>();
                col.offset = new Vector2(-0.5f, 0);
                col.size = new Vector2(1, 10000);
            }
            obj.transform.position = new Vector3(x, y, 0);
            obj.transform.SetParent(geometryGroup);
        }
    }
}