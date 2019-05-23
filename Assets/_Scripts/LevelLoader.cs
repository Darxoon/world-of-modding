using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Xml;


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
        public Color backgroundColor;

        public SceneInfo(string minX, string minY, string maxX, string maxY, string bgColor)
        {
            this.minX = float.Parse(minX);
            this.minY = float.Parse(minY);
            this.maxX = float.Parse(maxX);
            this.maxY = float.Parse(maxY);
            string[] colors = bgColor.Split(',');
            backgroundColor = new Color(
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
        public Color colorize;
        public string image; 

        public SceneLayer(string depth, string x, string y, string scaleX, string scaleY, string rotation, string alpha, string colorize, string image)
        {
            this.depth = float.Parse(depth);
            pos = new Vector2(float.Parse(x), float.Parse(y));
            scale = new Vector2(float.Parse(scaleX), float.Parse(scaleY));
            string[] colors = colorize.Split(',');
            this.colorize = new Color(
                float.Parse(colors[0]) / 255,
                float.Parse(colors[1]) / 255,
                float.Parse(colors[2]) / 255, 
                float.Parse(alpha));
            this.rotation = float.Parse(rotation);
            this.image = image;
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

    [SerializeField] private GameObject sceneLayerPrefab;

    [Header("References")]

    [SerializeField] private Transform sceneLayerGroup;

    // references that can be guessed automatically 

    private Camera mainCam;

    // resources 

    private Dictionary<string, Sprite>   imgResources;
    private Dictionary<string, AudioClip>   soundResources;
    private Dictionary<string, string>      textResources;

    private void Start()
    {
        // references 

        mainCam = Camera.main;

        // set camera's aspect ratio 

        mainCam.aspect = 16f / 9f;

        // load the files
        try
        {


            XmlDocument resrc = new XmlDocument();
            XmlTextReader resrcReader = new XmlTextReader(Path.Combine(resDirectory, "res", levelDirectory, levelToLoad, levelToLoad + ".resrc"));
            resrcReader.Read();
            resrc.Load(resrcReader);

            Debug.LogWarning("loading resrc");
            LoadResrc(resrc);

            XmlDocument scene = new XmlDocument();
            XmlTextReader levelReader = new XmlTextReader(Path.Combine(resDirectory, "res", levelDirectory, levelToLoad, levelToLoad + ".scene"));
            levelReader.Read();
            scene.Load(levelReader);

            Debug.LogWarning("loading scene");
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
                new Vector2(texture.width / 2, texture.height / 2)); 
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

    private void LoadScene(XmlDocument scene)
    {
        XmlNode sceneAttributes = scene.SelectSingleNode("/scene");

        sceneInfo = new SceneInfo(
            sceneAttributes.Attributes["minx"].Value,
            sceneAttributes.Attributes["miny"].Value,
            sceneAttributes.Attributes["maxx"].Value,
            sceneAttributes.Attributes["maxy"].Value,
            sceneAttributes.Attributes["backgroundcolor"].Value);

        // scene layers
        XmlNodeList sceneLayerNodes = scene.SelectNodes("/scene/SceneLayer");
        List<SceneLayer> sceneLayers = new List<SceneLayer>();
        foreach (XmlNode item in sceneLayerNodes)
        {
            sceneLayers.Add(new SceneLayer(
                item.Attributes["depth"].Value,
                item.Attributes["x"].Value,
                item.Attributes["y"].Value,
                item.Attributes["scalex"].Value,
                item.Attributes["scaley"].Value,
                item.Attributes["rotation"].Value,
                item.Attributes["alpha"].Value,
                item.Attributes["colorize"].Value,
                item.Attributes["image"].Value));
        }
        this.sceneLayers = sceneLayers.ToArray();

        foreach (SceneLayer item in sceneLayers)
        {
            GameObject sceneLayer = Instantiate(sceneLayerPrefab, sceneLayerGroup);
            Debug.Log(item.image);
            sceneLayer.GetComponent<SpriteRenderer>().sprite = imgResources[item.image];
        }

    }
}
