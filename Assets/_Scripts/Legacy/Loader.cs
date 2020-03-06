using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//xml reading imports
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

public class Loader : MonoBehaviour {

    /*[XmlAttribute("ballsrequired")]
    public int ballsrequired;

    [XmlAttribute("letterboxed")]
    public bool letterboxed;

    [XmlAttribute("visualdebug")]
    public bool visualdebug;

    [XmlAttribute("autobounds")]
    public bool autobounds;

    // Use this for initialization*/

    public GameObject thing;
    public Sprite aaa;
    public GameObject button;
    public Transform CanvasT;
    public GameObject camera;
    //public string[] ImageID = new string[] { };
    //public string[] ImageLoc = new string[] { };

    public List<string> ImageID = new List<string>();
    public List<string> ImageLoc = new List<string>();
    public List<string> BallImageID = new List<string>();
    public List<string> BallImageLoc = new List<string>();
    public List<GameObject> gooballs = new List<GameObject>();

    public string PathToLevel = @"";

    IEnumerator Wait(GameObject gb1, GameObject gb2)
    {
        yield return new WaitUntil(() => gb1.GetComponentInChildren<StrandController>() != null);
        gb1.GetComponentInChildren<StrandController>().AttatchStrandFromLevel(gb2);
        gb2.GetComponent<GooBallController>().isTower = true;
        gb1.GetComponent<GooBallController>().isTower = true;
    }

    public string LoadImageID(string id)
    {
        string location = "none";

        for (int i = 0; i <= ImageID.Count; i++) {
            if (id == ImageID[i])
            {
                location = ImageLoc[i];
                return location;
            }


        }

        return location;
    }
    /*
    IEnumerator WaitOneFrame()
    {
        yield return null;
    }
    */

    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }
    public bool isMapWorldView = true;
    public bool isInChapter = false;

    public float GetFieldOfView(float orthoSize, float distanceFromOrigin)
    {
        // orthoSize
        float a = orthoSize;
        // distanceFromOrigin
        float b = Mathf.Abs(distanceFromOrigin);

        float fieldOfView = Mathf.Atan(a / b) * Mathf.Rad2Deg * 2f;
        return fieldOfView;
    }


    public void LoadSelectedLevel (string path)
    {
        PathToLevel = path;
        RestoreValues.path = path;

        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        LoadResrc(path);
        LoadLevel(path);
        LoadScene(path);
    }

    public void LoadNewLevel(string path, bool chapter)
    {
        if (chapter == false)
        {
            if (isInChapter)
                RestoreValues.chaptherPath = PathToLevel;
            PathToLevel = path;
            RestoreValues.path = path;


            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            LoadResrc(path);
            LoadLevel(path);
            LoadScene(path);
        }
        if(chapter == true)
        {
            PathToLevel = RestoreValues.chaptherPath;
            path = RestoreValues.chaptherPath;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            isInChapter = true;
            LoadResrc(path);
            LoadLevel(path);
            LoadScene(path);
        }
    }

    void Start () {
        PathToLevel = RestoreValues.path;
        isMapWorldView = RestoreValues.isMapWorldView;
        isInChapter = RestoreValues.isInChapter;
        LoadSelectedLevel(PathToLevel);
    }

	void Update () {


    }

    void LoadLevel(string pathToFile)
    {
        //level characteristics

        XmlDocument scene = new XmlDocument();
        scene.Load(pathToFile + ".scene.xml");

        XmlNode sceneAttributes = scene.SelectSingleNode("/scene");
        float minX = float.Parse(sceneAttributes.Attributes["minx"].Value);
        float minY = float.Parse(sceneAttributes.Attributes["miny"].Value);
        float maxX = float.Parse(sceneAttributes.Attributes["maxx"].Value);
        float maxy = float.Parse(sceneAttributes.Attributes["maxy"].Value);


        Debug.Log("------level characteristics------");
        XmlDocument level = new XmlDocument();
        level.Load(pathToFile+".level.xml");

        string ballsrequired = level.SelectSingleNode("/level/@ballsrequired").Value;
        Debug.Log(ballsrequired);

        string letterboxed = level.SelectSingleNode("/level/@letterboxed").Value;
        Debug.Log(letterboxed);

        string visualdebug = level.SelectSingleNode("/level/@visualdebug").Value;
        Debug.Log(visualdebug);

        string autobounds = level.SelectSingleNode("/level/@autobounds").Value;
        Debug.Log(autobounds);

        string textcolor = level.SelectSingleNode("/level/@textcolor").Value;
        Debug.Log(textcolor);
        if (level.SelectSingleNode("/level/@texteffects") != null)
        {
            string texteffects = level.SelectSingleNode("/level/@texteffects").Value;
            Debug.Log(texteffects);
        }
        string timebugprobability = level.SelectSingleNode("/level/@timebugprobability").Value;
        Debug.Log(timebugprobability);

        string strandgeom = level.SelectSingleNode("/level/@strandgeom").Value;
        Debug.Log(strandgeom);

        string allowskip = level.SelectSingleNode("/level/@allowskip").Value;
        Debug.Log(allowskip);
        Debug.Log("------level characteristics------");

        //camera1 characteristics (normal)

        Debug.Log("------camera characteristics (normal)------");
        string camera_aspect = level.SelectSingleNode("/level/camera/@aspect").Value;
        Debug.Log(camera_aspect);

        string camera_pos = level.SelectSingleNode("/level/camera/poi/@pos").Value;
        Debug.Log(camera_pos);

        string camera_traveltime = level.SelectSingleNode("/level/camera/poi/@traveltime").Value;
        Debug.Log(camera_traveltime);

        string camera_pause = level.SelectSingleNode("/level/camera/poi/@pause").Value;
        Debug.Log(camera_pause);

        string camera_zoom = level.SelectSingleNode("/level/camera/poi/@zoom").Value;
        Debug.Log(camera_zoom);
        Debug.Log("------camera characteristics (normal)------");

        //camera1 characteristics (normal)

        Debug.Log("------camera characteristics (widescreen)------");
        string camera1_aspect = level.SelectSingleNode("/level/camera1/@aspect").Value;
        Debug.Log(camera1_aspect);

        string camera1_pos = level.SelectSingleNode("/level/camera1/poi/@pos").Value;
        Debug.Log(camera1_pos);

        string camera1_traveltime = level.SelectSingleNode("/level/camera1/poi/@traveltime").Value;
        Debug.Log(camera1_traveltime);

        string camera1_pause = level.SelectSingleNode("/level/camera1/poi/@pause").Value;
        Debug.Log(camera1_pause);

        string camera1_zoom = level.SelectSingleNode("/level/camera1/poi/@zoom").Value;
        float zoom = float.Parse(level.SelectSingleNode("/level/camera1/poi/@zoom").Value);
        Debug.Log(camera1_zoom);

        camera = new GameObject();
        camera.name = "Camera";
        zoom = zoom * 0.09f;
        camera.tag = "MainCamera";
        GameObject scrollDetector = new GameObject();
        scrollDetector.transform.SetParent(camera.transform);
        scrollDetector.name = "scroll detector";
        scrollDetector.transform.position = new Vector3(0, 0, 3601);
        scrollDetector.AddComponent<BoxCollider2D>().isTrigger = true;
        scrollDetector.transform.localScale = new Vector3(8.237139f, 3.66913f, 2.875494f);
        scrollDetector.AddComponent<PanController>();
        scrollDetector.GetComponent<PanController>().maxX = maxX / 100;
        scrollDetector.GetComponent<PanController>().maxY = maxy / 100;
        scrollDetector.GetComponent<PanController>().minX = minX / 100;
        scrollDetector.GetComponent<PanController>().minY = minY / 100;
        GameObject cameraFrustum = new GameObject();
        cameraFrustum.AddComponent<BoxCollider2D>().isTrigger = true;
        cameraFrustum.GetComponent<BoxCollider2D>().size = new Vector2(10.05f, 5.65f);
        //cameraFrustum.transform.localScale = new Vector3(10.00229f, 5.616897f, 3.448575f);
        cameraFrustum.name = "camera frustum";
        cameraFrustum.transform.parent = camera.transform;
        cameraFrustum.transform.position = new Vector3(0, 0, 3601);
        //camera.AddComponent<Camera>().orthographicSize = zoom;
        //camera.GetComponent<Camera>().orthographic = true;
        //camera.GetComponent<Camera>().pixelRect = new Rect(0, 0, , 1);

        //get the x and y values for the camera
        string[] pos = camera1_pos.Split(',');
        float Cx = float.Parse(pos[0]);
        float Cy = float.Parse(pos[1]);

        //normalize the location
        Cx = Cx / 100;
        Cy = Cy / 100;
        Cx = Cx - 0.25f;
        camera.transform.Translate(Cx, Cy, -3600);
        camera.AddComponent<Camera>().nearClipPlane = 0.01f;
        camera.GetComponent<Camera>().farClipPlane = 9999999f;
        camera.GetComponent<Camera>().depth = -1;
        camera.GetComponent<Camera>().cullingMask = (1 << 0) | (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4) | (1 << 5) | (1 << 6) | (1 << 7);
        //camera.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;\
        camera.GetComponent<Camera>().fieldOfView = zoom;





        /*
        GameObject PCam1 = new GameObject();
        PCam1.AddComponent<Camera>();
        Camera PCamN = PCam1.GetComponent<Camera>();
        PCamN.clearFlags = CameraClearFlags.Depth;
        PCamN.cullingMask = (0 << 0) | (0 << 1) | (0 << 2) | (0 << 3) | (0 << 4) | (0 << 5) | (0 << 6) | (0 << 7) | (1 << 8);
        PCam1.transform.parent = camera.transform;
        PCam1.transform.localPosition = new Vector3();
        PCamN.fieldOfView = GetFieldOfView(zoom, -10);
        PCamN.farClipPlane = 9999999f;
        PCamN.nearClipPlane = 0;

        // distanceFromOrigin
        float b = Mathf.Abs(camera.transform.position.z);

        //change clipping planes based on main camera z-position
        PCamN.farClipPlane = b;
        PCamN.nearClipPlane = camera.GetComponent<Camera>().nearClipPlane;

        */ //to the fam who is looking at this source code right now:
        //i tried to use perspective cameras to get the parallax effect, but it was not the best solution, since things also got their size changed aswell, which is unwanted behaviour


        Debug.Log("------camera characteristics (widescreen------");




        //skipping a lotta stuff because i dont feel like making a level that contains every single god damn element

        //music
        XmlNodeList musicList = level.GetElementsByTagName("music");
        for (int i = 0; i < musicList.Count; i++)
        {
            string idM = musicList[i].Attributes["id"].Value;
            Debug.Log("found music with id of " + idM);
        }

        //gooballs

        XmlNodeList ballList = level.GetElementsByTagName("BallInstance");
        for (int i = 0; i < ballList.Count; i++)
        {
            string type = ballList[i].Attributes["type"].Value;
            float x = float.Parse(ballList[i].Attributes["x"].Value);
            float y = float.Parse(ballList[i].Attributes["y"].Value);
            string id = ballList[i].Attributes["id"].Value;
            Debug.Log("found gooball with type " + type + " at " + x + " " + y);

            GenerateBall(type, x, y, id);

        }

        //arms

        XmlNodeList arms = level.GetElementsByTagName("Strand");
        for (int i=0; i< arms.Count; i++)
        {
            string gooballID1 = arms[i].Attributes["gb1"].Value;
            string gooballID2 = arms[i].Attributes["gb2"].Value;
            GameObject gb1 = null;
            GameObject gb2 = null;

            for (int y = 0; i < gooballs.Count; y++) {



                string requiredID1 = "ID: " + gooballID1.ToString();
                string requiredID2 = "ID: " + gooballID2.ToString();

                if (gooballs[y].name.Contains(requiredID1)) { 
                    gb1 = gooballs[y];
                }
                if (gooballs[y].name.Contains(requiredID2))
                {
                    gb2 = gooballs[y];
                }


                if (gb1 != null && gb2 != null)
                    break;
            }

            if (gb1 == null) Debug.Log("gb1 was null");
            else Debug.Log(gb1.name);
            if (gb2 == null) Debug.Log("gb2 was null");
            else Debug.Log(gb2.name);

            //if (gb1.GetComponentInChildren<StrandController>() != null) Debug.Log("Yay");
            if (gb1.transform.Find(gb1.name + "/Strand Range") != null) Debug.Log("Yay");
            else Debug.Log("Nay");
            StartCoroutine(Wait(gb1, gb2));
            //gb1.GetComponentInChildren<StrandController>().AttatchStrandFromLevel(gb2);

            //thing.AttatchStrandFromLevel(gb2);
            //gb1.transform.Find("Strand Range").GetComponent<StrandController>().AttatchStrandFromLevel(gb2);
        }
        if (level.SelectNodes("/level/pipe").Count != 0)
        {
            GameObject current = null;
            GameObject last = null;
            XmlNodeList pipes = level.SelectNodes("/level/pipe");
            GameObject pipe = new GameObject();
            pipe.name = "pipe";
            for (int z = 0; z < pipes.Item(0).ChildNodes.Count; z++)
            {
                GameObject vertex = new GameObject();
                if (z == 0)
                {
                    GameObject levelexit = new GameObject();
                    levelexit.name = "levelexit";
                    levelexit.transform.position = new Vector3(float.Parse(pipes.Item(0).ChildNodes[z].Attributes["x"].Value) / 100, float.Parse(pipes.Item(0).ChildNodes[z].Attributes["y"].Value) / 100);
                    levelexit.AddComponent<CircleCollider2D>().isTrigger = true;
                    levelexit.GetComponent<CircleCollider2D>().radius = float.Parse(level.SelectSingleNode("/level/levelexit/@radius").Value) / 100;
                    string positionS = level.SelectSingleNode("/level/levelexit/@pos").Value;
                    string[] position = positionS.Split(',');
                    float x = float.Parse(position[0]) / 100;
                    float y = float.Parse(position[1]) / 100;
                    string id = level.SelectSingleNode("/level/levelexit/@id").Value;
                    levelexit.name = id;
                    levelexit.transform.position = new Vector3(x, y, 0);
                    levelexit.AddComponent<levelExitController>();
                    levelexit.GetComponent<levelExitController>().pipeCap = vertex;
                    levelexit.GetComponent<levelExitController>().required = int.Parse(level.SelectSingleNode("/level/@ballsrequired").Value);
                    levelexit.GetComponent<levelExitController>().loader = this;
                }
                
                vertex.name = "vertex";
                vertex.transform.position = new Vector3(float.Parse(pipes.Item(0).ChildNodes[z].Attributes["x"].Value) / 100, float.Parse(pipes.Item(0).ChildNodes[z].Attributes["y"].Value) / 100);
                vertex.transform.SetParent(pipe.transform);
                if (z != 0)
                {
                    last = current;
                    current = vertex;
                }
                if (z == 0)
                {
                    current = vertex;
                }

                current.AddComponent<pipeParts>().last = last;
                current.GetComponent<pipeParts>().order = z;
            }
        }

    }

    void LoadResrc(string pathToFile)
    {
        XmlDocument resrc = new XmlDocument();
        resrc.Load(pathToFile+".resrc.xml");

        XmlNodeList resrcList = resrc.GetElementsByTagName("Image");
        for (int i = 0; i < resrcList.Count; i++)
        {
            string id = resrcList[i].Attributes["id"].Value;
            string path = resrcList[i].Attributes["path"].Value;
            Debug.Log("found resource with id " + id + " at /" + path);
            ImageID.Add(id);
            ImageLoc.Add(path + ".png");
        }

        XmlNodeList resrcSoundList = resrc.GetElementsByTagName("Sound");
        for (int i = 0; i < resrcSoundList.Count; i++)
        {
            string id = resrcSoundList[i].Attributes["id"].Value;
            string path = resrcSoundList[i].Attributes["path"].Value;
            Debug.Log("found sound resource with id " + id + " at /" + path);
        }
    }

    void LoadScene(string pathToFile)
    {
        XmlDocument scene = new XmlDocument();
        scene.Load(pathToFile+".scene.xml");

        //force fields
        XmlNodeList radialffList = scene.GetElementsByTagName("radialforcefield");
        for (int i = 0; i < radialffList.Count; i++)
        {
            string id = radialffList[i].Attributes["id"].Value;
            string type = radialffList[i].Attributes["type"].Value;
            string center = radialffList[i].Attributes["center"].Value;
            float radius = float.Parse(radialffList[i].Attributes["radius"].Value);
            float forceatcenter = float.Parse(radialffList[i].Attributes["forceatcenter"].Value);
            float forceatedge = float.Parse(radialffList[i].Attributes["forceatedge"].Value);
            string dampeningfactor = radialffList[i].Attributes["dampeningfactor"].Value;
            string antigrav = radialffList[i].Attributes["antigrav"].Value;
            string geomonly = radialffList[i].Attributes["geomonly"].Value;
            string enabled1 = radialffList[i].Attributes["enabled"].Value;
            Debug.Log("found a radial forcefield with id of " + id + " with type of " + type + " with its center at " + center + " and radius being" + radius + "; It's force at center is " + forceatcenter + " and force at edge is " + forceatedge + " its dampening factor is " + dampeningfactor + " antigravity is set to " + antigrav + " geomonly is set to(if true will affect gooballs too, if false, wont) " + geomonly + " and its enabled state is " + enabled1);
            GameObject RFF = new GameObject();
            RFF.AddComponent<RadialForceField>();
            RFF.AddComponent<Rigidbody2D>().gravityScale = 0;
            RFF.AddComponent<CircleCollider2D>().isTrigger = true;
            //RFF.AddComponent<PointEffector2D>().forceMagnitude = forceatcenter * 10000;
            //RFF.GetComponent<PointEffector2D>().forceMode = EffectorForceMode2D.InverseSquared;
            //RFF.GetComponent<PointEffector2D>().forceSource = EffectorSelection2D.Rigidbody;
            RFF.GetComponent<RadialForceField>().ForceAtCenter = forceatcenter * -1;
            RFF.GetComponent<RadialForceField>().ForceAtEdge = forceatedge * -1;
            RFF.name = id;
            string[] coords = center.Split(',');
            float x = float.Parse(coords[0]);
            float y = float.Parse(coords[1]);
            x = x / 100;
            y = y / 100;
            radius = radius / 100;
            //RFF.GetComponent<PointEffector2D>().distanceScale = radius;
            RFF.GetComponent<CircleCollider2D>().radius = radius;
            RFF.GetComponent<CircleCollider2D>().usedByEffector = true;
            RFF.GetComponent<RadialForceField>().Radius = radius;
            RFF.transform.position = new Vector3(x, y, 0);
        }

        XmlNodeList linearffList = scene.GetElementsByTagName("linearforcefield");
        for (int i = 0; i < linearffList.Count; i++)
        {
            string type = linearffList[i].Attributes["type"].Value;
            string force = linearffList[i].Attributes["force"].Value;
            string[] forceValues = force.Split(',');
            float forceX = float.Parse(forceValues[0]); forceX = forceX;
            float forceY = float.Parse(forceValues[1]); forceY = forceY;
            string dampeningfactor = linearffList[i].Attributes["dampeningfactor"].Value;
            string antigrav = linearffList[i].Attributes["antigrav"].Value;
            string geomonly = linearffList[i].Attributes["geomonly"].Value;
            Debug.Log("found a linear forcefield with type " + type + "; its force direction is " + force + "; its dampening factor is " + dampeningfactor + "; its antigrav state is set to " + antigrav + " and its geomonly thing is set to " + geomonly) ;
            GameObject LFF = new GameObject();
            LFF.transform.position = new Vector3(0, 0, 1);
            LFF.AddComponent<LinearForceField>().forceX = forceX;
            LFF.GetComponent<LinearForceField>().forceY = forceY;
            LFF.GetComponent<LinearForceField>().antigrav = bool.Parse(antigrav);
            LFF.AddComponent<BoxCollider2D>().isTrigger = true;
            LFF.GetComponent<BoxCollider2D>().size = new Vector2(100, 100);

        }

        XmlNodeList particleList = scene.GetElementsByTagName("particles");
        for (int i = 0; i < particleList.Count; i++)
        {
            string effect = particleList[i].Attributes["effect"].Value;
            string depth = particleList[i].Attributes["depth"].Value;
            string pos = "none";
            if (particleList[i].Attributes["pos"] != null)
            {
                pos = particleList[i].Attributes["pos"].Value;
            }
            Debug.Log("found a particle emitter with its effect of "+effect+"; its depth is "+depth+"; and its position is "+pos);
        }


        //scenelayers

        XmlNodeList scenelayers = scene.GetElementsByTagName("SceneLayer");
        for (int i = 0; i < scenelayers.Count; i++)
        {
            

            string id = "none";
            if (scenelayers[i].Attributes["id"] != null)
            {
                id = scenelayers[i].Attributes["id"].Value;
            }
            string name = "none";
            if (scenelayers[i].Attributes["name"] != null)
            {
                name = scenelayers[i].Attributes["name"].Value;
            }

            
            float depth = float.Parse(scenelayers[i].Attributes["depth"].Value);
            float x = float.Parse(scenelayers[i].Attributes["x"].Value);
            float y = float.Parse(scenelayers[i].Attributes["y"].Value);

            //try to normalize the values so they ain't so damn far
            x = x / 100; // old was 250
            y = y / 100; //old was 250

            float scalex = float.Parse(scenelayers[i].Attributes["scalex"].Value);
            float scaley = float.Parse(scenelayers[i].Attributes["scaley"].Value);
            float rotation = float.Parse(scenelayers[i].Attributes["rotation"].Value);
            string alpha = scenelayers[i].Attributes["alpha"].Value;
            string colorize = scenelayers[i].Attributes["colorize"].Value;
            string image = scenelayers[i].Attributes["image"].Value;
            string anim = "none";
            if (scenelayers[i].Attributes["anim"] != null)
            {
                anim = scenelayers[i].Attributes["anim"].Value;
            }
            string animspeed = "none";
            if (scenelayers[i].Attributes["animspeed"] != null)
            {
                animspeed = scenelayers[i].Attributes["animspeed"].Value;
            }
            Debug.Log("SL's id is "+id+" name is "+name+" depth is "+depth+" its x coords are "+x+" it's y coords are "+y+" its x scale is "+scalex+" its y scale is "+scaley+" its rotation is "+rotation+" its transparency is "+alpha+" colorize values are "+colorize+" and its image id is "+image);

            //generate the thing

            //colorize the thing
            string[] tint = colorize.Split(',');
            float r = float.Parse(tint[0]);
            float g = float.Parse(tint[1]);
            float b = float.Parse(tint[2]);

            GameObject SceneLayer = new GameObject();
            Texture2D TextureE = new Texture2D(2, 2);
            byte[] imageData = File.ReadAllBytes(LoadImageID(image));
            TextureE.LoadImage(imageData);
            Sprite Sprite = Sprite.Create(TextureE, new Rect(0.0f, 0.0f, TextureE.width, TextureE.height), new Vector2(0.5f, 0.5f));
            SceneLayer.AddComponent<SpriteRenderer>().sprite = Sprite;
            Color Colorize;
            Colorize = new Color(r, g, b);
            SceneLayer.GetComponent<SpriteRenderer>().color = Colorize;
            
            SceneLayer.name = name;

            int order = (int)depth;
            order = order * -1;
            int depth1 = (int)depth;
            SceneLayer.GetComponent<SpriteRenderer>().sortingOrder = depth1;

            //scalex = scalex / 2.5f;
            //scaley = scaley / 2.5f;

            SceneLayer.transform.Translate(x, y, order);
            SceneLayer.transform.rotation = Quaternion.Euler(0, 0, rotation);
            SceneLayer.transform.localScale = new Vector3(scalex, scaley, 0);
            Debug.Log(name + " " + rotation + " current: "+ SceneLayer.transform.rotation.ToString());

        }

        //buttons

        XmlNodeList buttons = scene.GetElementsByTagName("button");
        for (int i = 0; i < buttons.Count; i++)
        {
            string id = buttons[i].Attributes["id"].Value;
            string depth = buttons[i].Attributes["depth"].Value;
            float x = float.Parse(buttons[i].Attributes["x"].Value);
            float y = float.Parse(buttons[i].Attributes["y"].Value);
            float scalex = float.Parse(buttons[i].Attributes["scalex"].Value);
            float scaley = float.Parse(buttons[i].Attributes["scaley"].Value);
            float rotation = float.Parse(buttons[i].Attributes["rotation"].Value);
            string alpha = buttons[i].Attributes["alpha"].Value;
            string colorize = buttons[i].Attributes["colorize"].Value;
            string up = buttons[i].Attributes["up"].Value;
            string over = buttons[i].Attributes["over"].Value;
            string onMouseEnter = buttons[i].Attributes["onmouseenter"].Value;
            string onMouseExit = buttons[i].Attributes["onmouseexit"].Value;
            string onClick = buttons[i].Attributes["onclick"].Value;
            //string disabled = buttons[i].Attributes["disabled"].Value;
            Debug.Log("id: "+id+", depth: "+depth+", x: "+x+", y: "+y+", scalex: "+scalex+", scaley: "+scaley+", rotation: "+rotation+", alpha: "+alpha+", colorize: "+colorize);


            //GameObject buttonThing;
            GameObject buttonThing = Instantiate(this.button, CanvasT);
            CanvasT.GetComponent<Canvas>().worldCamera = camera.GetComponent<Camera>();
            buttonThing.name = id;
            y = y / 100;
            x = x / 100;

            scalex = scalex / 100;
            scaley = scaley / 100;
            

            buttonThing.transform.Translate(x, y, -2);
            buttonThing.transform.rotation = Quaternion.Euler(0, 0, rotation);
            buttonThing.transform.localScale = new Vector3(scalex, scaley, 0);

            GameObject upG;
            GameObject overG;
            upG = buttonThing.transform.GetChild(0).gameObject;
            overG = buttonThing.transform.GetChild(1).gameObject;

            //sets texture for the up button
            Texture2D TextureE = new Texture2D(2, 2);
            byte[] imageData = File.ReadAllBytes("E:/Unity/Projects/World of Goo 2U 2/"+LoadImageID(up));
            TextureE.LoadImage(imageData);
            Sprite Sprite = Sprite.Create(TextureE, new Rect(0.0f, 0.0f, TextureE.width, TextureE.height), new Vector2(0.5f, 0.5f));
            upG.GetComponent<Image>().sprite = Sprite;
            upG.GetComponent<RectTransform>().sizeDelta = new Vector2(TextureE.width, TextureE.height);

            //sets texture for the over button
            
            Texture2D TextureE2 = new Texture2D(2, 2);
            byte[] imageData2 = File.ReadAllBytes("E:/Unity/Projects/World of Goo 2U 2/"+LoadImageID(over));
            Debug.Log(LoadImageID(over));
            TextureE2.LoadImage(imageData2);
            Sprite Sprite2 = Sprite.Create(TextureE2, new Rect(0.0f, 0.0f, TextureE2.width, TextureE2.height), new Vector2(0.5f, 0.5f));
            overG.GetComponent<Image>().sprite = Sprite2;
            overG.GetComponent<RectTransform>().sizeDelta = new Vector2(TextureE.width, TextureE.height);

            if(isMapWorldView = true)
            {
                //WILL ADD MORE COMMMANDS LATER, http://goofans.com/developers/world-of-goo-level-editor/reference-guide/commands ------------------------------------------------------------------------------------------
                string command = Regex.Replace(onClick, @"\d", "");
                Debug.Log("started checking for commands");
                if (command == "island")
                {
                    int islandNumber = int.Parse(Regex.Match(onClick, @"\d+").Value);
                    //LoadChapter(islandNumber);
                    overG.GetComponent<Button>().onClick.AddListener(() => LoadChapter(islandNumber));
                    Debug.Log("found an island button!");


                }
                if (command == "quit")
                {
                    //Application.Quit(); //this doesnt do anythign in the editor, add it back before publishing ----------------------------------------------------------------------------------------------------------------------
                     
                    overG.GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().name));//this relaods the scene, only use in the editor
                }

                
            }
            if (isInChapter = true)
            {
                
                if (onClick.StartsWith("pl_"))
                {
                    string level = onClick.Replace("pl_", string.Empty);
                    overG.GetComponent<Button>().onClick.AddListener(() => LoadNewLevel(@"E:\Unity\Projects\World of Goo 2U 2\res\levels\" + level + @"\" + level, false));
                }
            }

        }

        //labels

        XmlNodeList labels = scene.GetElementsByTagName("label");
        for (int i = 0; i < labels.Count; i++)
        {
            string id = labels[i].Attributes["id"].Value;
            string depth = labels[i].Attributes["depth"].Value;
            string x = labels[i].Attributes["x"].Value;
            string y = labels[i].Attributes["y"].Value;
            string align = labels[i].Attributes["align"].Value;
            string rotation = labels[i].Attributes["rotation"].Value;
            string scale = labels[i].Attributes["scale"].Value;
            string overlay = labels[i].Attributes["overlay"].Value;
            string screenspace= labels[i].Attributes["screenspace"].Value;
            string font= labels[i].Attributes["font"].Value;
            string text = labels[i].Attributes["text"].Value;
            Debug.Log("");
        }

        //static geometry

        
        //lines
        XmlNodeList lines = scene.GetElementsByTagName("line");
        for (int i = 0; i < lines.Count; i++)
        {
            string id = lines[i].Attributes["id"].Value;
            string static1 = lines[i].Attributes["static"].Value;
            string tag = "none";
            if (lines[i].Attributes["tag"] != null)
            {
                tag = lines[i].Attributes["tag"].Value;
            }
            string material = lines[i].Attributes["material"].Value;
            string anchor = lines[i].Attributes["anchor"].Value;

            string[] anchorparts = anchor.Split(',');
            float anchor1 = float.Parse(anchorparts[0]) / 100;
            float anchor2 = float.Parse(anchorparts[1]) /100;

            string normal = lines[i].Attributes["normal"].Value;

            string[] normalparts = anchor.Split(',');
            float normal1 = float.Parse(normalparts[0]);
            float normal2 = float.Parse(normalparts[1]);
            Debug.Log("");

            //here comes the instantiate part :D
            GameObject thing = new GameObject();
            //thing = Instantiate(thing, new Vector3(0, 0, 0), Quaternion.identity);
            thing.AddComponent<BoxCollider2D>().size = new Vector2(9999,0.01f);
            thing.tag = tag;
            //sets the material of the thing VVV
            //circle.GetComponent<CircleCollider2D>().sharedMaterial = 
            //
             
            thing.transform.Translate(anchor1, anchor2, 0);


            float linerot = 0;
            float one = normal1;
            float two = normal2;
            Debug.Log(one);
            Debug.Log(two);
            if (normal2 != 0)
            {
                linerot = 180 * Mathf.Atan(normal2 / normal1) / Mathf.PI;
            }

            if (one <= 0) { linerot = 270; }
            else { linerot = 90; }

            thing.name = id;
            //thing.transform.rotation = new Quaternion(0, 0, linerot, thing.transform.rotation.w);
            Vector3 localpos = new Vector3(0, one, two) + thing.transform.position;
            //localpos.y = 0;

            Quaternion rotation = Quaternion.LookRotation(localpos);
            rotation.z = rotation.y;
            thing.transform.rotation = rotation;

            //thing.transform.LookAt(localpos);
        }
        




        // composite geometry
        XmlNodeList compGeom = scene.GetElementsByTagName("compositegeom");
        for (int i = 0; i < compGeom.Count; i++)
        {

            GameObject Parent = null;
            if (GameObject.Find(i.ToString()) != null)
            {
                Parent = GameObject.Find(i.ToString());
            }
            else
            {
                Parent = new GameObject();
            }

            Parent.name = compGeom[i].SelectNodes("/scene/compositegeom").Item(i).Attributes["id"].Value;

            Parent.transform.position = new Vector3(float.Parse(compGeom[i].SelectNodes("/scene/compositegeom").Item(i).Attributes["x"].Value) / 100, float.Parse(compGeom[i].SelectNodes("/scene/compositegeom").Item(i).Attributes["y"].Value) / 100, 0);
            Parent.transform.rotation = Quaternion.Euler(0, 0, float.Parse(compGeom[i].SelectNodes("/scene/compositegeom").Item(i).Attributes["rotation"].Value));
            //to add static tag thing
            //to add tag things
            //to add material things

            //Debug.Log(compGeom[i].SelectNodes("/scene/compositegeom/rectangle/@x").Item(0).Value);
            Debug.Log(compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Count);
            for (int SS = 0; SS < compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Count; SS++)
            {







                if (compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Item(SS).Name == "rectangle")
                {
                    string id = compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Item(SS).Attributes["id"].Value;
                    Debug.Log(compGeom[i].ParentNode.Name);


                    string static1 = "none";
                    string tag = "none";
                    if (compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Item(SS).Attributes["tag"] != null)
                    {
                        tag = compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Item(SS).Attributes["tag"].Value;
                    }

                    if (compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Item(SS).Attributes["static"] != null)
                    {
                        static1 = compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Item(SS).Attributes["static"].Value;
                    }

                    string material = "none";

                    if (compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Item(SS).Attributes["material"] != null)
                    {
                        material = compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Item(SS).Attributes["material"].Value;
                    }


                    float x = float.Parse(compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Item(SS).Attributes["x"].Value);
                    float y = float.Parse(compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Item(SS).Attributes["y"].Value);
                    float width = float.Parse(compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Item(SS).Attributes["width"].Value);
                    float height = float.Parse(compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Item(SS).Attributes["height"].Value);
                    float rotation = float.Parse(compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Item(SS).Attributes["rotation"].Value);
                    //Debug.Log("");

                    //here comes the instantiate part :D

                    //defines parent's settings


                    GameObject thing = new GameObject();
                    thing.transform.SetParent(Parent.transform);
                    Parent.name = i.ToString();
                    //thing = Instantiate(thing, new Vector3(0, 0, 0), Quaternion.identity);
                    width = width / 100;
                    height = height / 100;

                    thing.AddComponent<BoxCollider2D>().size = new Vector2(width, height);
                    thing.tag = tag;
                    //sets the material of the thing VVV
                    //circle.GetComponent<CircleCollider2D>().sharedMaterial = 
                    //

                    y = y / 100;
                    x = x / 100;


                    thing.transform.localPosition = new Vector3(x, y, 0);
                    thing.name = id;

                }

                if (compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Item(SS).Name == "circle")
                {
                    string id = compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Item(SS).Attributes["id"].Value;
                    string static1 = "none";

                    if (compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Item(SS).Attributes["static"] != null)
                    {
                        static1 = compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Item(SS).Attributes["static"].Value;
                    }

                    string tag = "none";
                    if (compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Item(SS).Attributes["tag"] != null)
                    {
                        tag = compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Item(SS).Attributes["tag"].Value;
                    }
                    string material = "none";
                    if (compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Item(SS).Attributes["tag"] != null)
                    {
                        material = compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Item(SS).Attributes["material"].Value;
                    }


                    float x = float.Parse(compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Item(SS).Attributes["y"].Value);
                    float y = float.Parse(compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Item(SS).Attributes["x"].Value);
                    float radius = float.Parse(compGeom[i].SelectNodes("/scene/compositegeom").Item(i).ChildNodes.Item(SS).Attributes["radius"].Value);

                    //here comes the instantiate part :D



                    GameObject thing2 = new GameObject();
                    thing2.transform.SetParent(Parent.transform);
                    //thing = Instantiate(thing, new Vector3(0,0,0), Quaternion.identity);
                    radius = radius / 100;
                    thing2.AddComponent<CircleCollider2D>().radius = radius;
                    //thing.tag = tag;
                    //sets the material of the thing VVV
                    //circle.GetComponent<CircleCollider2D>().sharedMaterial = 
                    //

                    y = y / 100;
                    x = x / 100;


                    thing2.transform.localPosition = new Vector3(y, x, 0);
                    thing2.name = id;
                }

            }
            // compGeom[i].SelectNodes

        }



        // normal geometry

        //rectangles
        XmlNodeList normalGeom = scene.SelectNodes("/scene/rectangle");
        for (int i = 0; i < normalGeom.Count; i++)
        {
            
            //if (normalGeom.Item(i).Name == "rectangle")
            if (i == i)
            {
                string id = normalGeom.Item(i).Attributes["id"].Value;


                string static1 = "none";
                string tag = "none";
                if (normalGeom.Item(i).Attributes["tag"] != null)
                {
                    tag = normalGeom.Item(i).Attributes["tag"].Value;
                }

                if (normalGeom.Item(i).Attributes["static"] != null)
                {
                    static1 = normalGeom.Item(i).Attributes["static"].Value;
                }

                string material = "none";

                if (normalGeom.Item(i).Attributes["material"] != null)
                {
                    material = normalGeom.Item(i).Attributes["material"].Value;
                }


                float x = float.Parse(normalGeom.Item(i).Attributes["x"].Value);
                float y = float.Parse(normalGeom.Item(i).Attributes["y"].Value);
                float width = float.Parse(normalGeom.Item(i).Attributes["width"].Value);
                float height = float.Parse(normalGeom.Item(i).Attributes["height"].Value);
                float rotation = float.Parse(normalGeom.Item(i).Attributes["rotation"].Value);
                //Debug.Log("");

                //here comes the instantiate part :D

                //defines parent's settings


                GameObject thing = new GameObject();
                //thing = Instantiate(thing, new Vector3(0, 0, 0), Quaternion.identity);
                width = width / 100;
                height = height / 100;

                thing.AddComponent<BoxCollider2D>().size = new Vector2(width, height);
                thing.tag = tag;
                //sets the material of the thing VVV
                //circle.GetComponent<CircleCollider2D>().sharedMaterial = 
                //

                y = y / 100;
                x = x / 100;


                thing.transform.localPosition = new Vector3(x, y, 0);
                thing.name = id;
            }

            
        }

        //circles
        XmlNodeList normalGeomC = scene.SelectNodes("/scene/circle");
        for (int i = 0; i < normalGeomC.Count; i++)
        {
            if (i == i)
            {
                string id = normalGeomC.Item(i).Attributes["id"].Value;
                string static1 = "none";

                if (normalGeomC.Item(i).Attributes["static"] != null)
                {
                    static1 = normalGeomC.Item(i).Attributes["static"].Value;
                }

                string tag = "none";
                if (normalGeomC.Item(i).Attributes["tag"] != null)
                {
                    tag = normalGeomC.Item(i).Attributes["tag"].Value;
                }
                string material = "none";
                if (normalGeomC.Item(i).Attributes["material"] != null)
                {
                    material = normalGeomC.Item(i).Attributes["material"].Value;
                }

                float x = float.Parse(normalGeomC.Item(i).Attributes["y"].Value);
                float y = float.Parse(normalGeomC.Item(i).Attributes["x"].Value);
                float radius = float.Parse(normalGeomC.Item(i).Attributes["radius"].Value);

                //here comes the instantiate part :D



                GameObject thing2 = new GameObject();
                //thing = Instantiate(thing, new Vector3(0,0,0), Quaternion.identity);
                radius = radius / 100;
                thing2.AddComponent<CircleCollider2D>().radius = radius;
                //thing.tag = tag;
                //sets the material of the thing VVV
                //circle.GetComponent<CircleCollider2D>().sharedMaterial = 
                //

                y = y / 100;
                x = x / 100;


                thing2.transform.localPosition = new Vector3(y, x, 0);
                thing2.name = id;
            }
        }


    }

    public void LoadChapter(int chapter)
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name); //reloads the scene, so i can put the chapter stuff
        XmlDocument scene = new XmlDocument(); //creates a new "document" in memory to read from
        scene.Load(@"E:\Unity\Projects\World of Goo 2U 2\res\islands\island"+chapter+".xml.xml"); //put stuff to read from taht document
        string map = scene.SelectSingleNode("/island").Attributes["map"].Value; // first line of island1.xml.xml: <island name="The Goo Filled Hills" map="island1" icon="IMAGE_GLOBAL_ISLAND_1_ICON">   reads the attribute `map`, and stores it as a string
        PathToLevel = @"E:\Unity\Projects\World of Goo 2U 2\res\levels\island" + chapter + @"\island" + chapter; //sets PathToLevel string to the path to the chapter level file(where u seelect the levels)
        LoadSelectedLevel(PathToLevel); //loads the selected level(in this case, the chapter)
        RestoreValues.path = PathToLevel; //saves the current path to level, so it doesnt overwrite it on load with the Main Menu level
        isInChapter = true; // just some extra memory optimization(i think it optimizes the game)
        RestoreValues.isInChapter = isInChapter; // ssaving previous value
        isMapWorldView = false; // optimization, as above
        RestoreValues.isMapWorldView = isMapWorldView; //saving previous value, as above
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // reloads scene
    }

    public void GenerateBall(string type, float x, float y, string id)
    {
        #region gooball resources
        XmlDocument gooballResrc = new XmlDocument();
        gooballResrc.Load(@"E:\Unity\Projects\World of Goo 2U 2\res\balls\" + type+@"\"+"resources.xml.xml");

        XmlNodeList resrcList = gooballResrc.GetElementsByTagName("Image");
        for (int i = 0; i < resrcList.Count; i++)
        {
            string BIid = resrcList[i].Attributes["id"].Value;
            string path = resrcList[i].Attributes["path"].Value;
            string pathPrefix = gooballResrc.SelectSingleNode("/ResourceManifest/Resources/SetDefaults/@path").Value;
            string idPrefix = gooballResrc.SelectSingleNode("/ResourceManifest/Resources/SetDefaults/@idprefix").Value;
            BallImageID.Add(idPrefix + BIid);
            BallImageLoc.Add(@"E:\Unity\Projects\World of Goo 2U 2\" + pathPrefix + path + ".png");
        }

        //GENERIC CRAP GOD DAMNIT

        BallImageID.Add("IMAGE_BALL_GENERIC_PUPIL1");
        BallImageLoc.Add(@"E:\Unity\Projects\World of Goo 2U 2\res\balls\_generic\pupil1.png");

        BallImageID.Add("IMAGE_BALL_GENERIC_EYE_GLASS_1");
        BallImageLoc.Add(@"E:\Unity\Projects\World of Goo 2U 2\res\balls\_generic\eye_glass_1.png");

        BallImageID.Add("IMAGE_BALL_GENERIC_EYE_GLASS_2");
        BallImageLoc.Add(@"E:\Unity\Projects\World of Goo 2U 2\res\balls\_generic\eye_glass_2.png");

        BallImageID.Add("IMAGE_BALL_GENERIC_EYE_GLASS_3");
        BallImageLoc.Add(@"E:\Unity\Projects\World of Goo 2U 2\res\balls\_generic\eye_glass_3.png");

        BallImageID.Add("IMAGE_BALL_GENERIC_EYE_FEM_L1");
        BallImageLoc.Add(@"E:\Unity\Projects\World of Goo 2U 2\res\balls\_generic\eye_fem_l1.png");

        BallImageID.Add("IMAGE_BALL_GENERIC_EYE_FEM_L2");
        BallImageLoc.Add(@"E:\Unity\Projects\World of Goo 2U 2\res\balls\_generic\eye_fem_l2.png");

        BallImageID.Add("IMAGE_BALL_GENERIC_EYE_FEM_R1");
        BallImageLoc.Add(@"E:\Unity\Projects\World of Goo 2U 2\res\balls\_generic\eye_fem_r1.png");

        BallImageID.Add("IMAGE_BALL_GENERIC_EYE_FEM_R2");
        BallImageLoc.Add(@"E:\Unity\Projects\World of Goo 2U 2\res\balls\_generic\eye_fem_r2.png");

        BallImageID.Add("IMAGE_BALL_GENERIC_SHADOW1");
        BallImageLoc.Add(@"E:\Unity\Projects\World of Goo 2U 2\res\balls\_generic\shadowCircle59.png");

        BallImageID.Add("IMAGE_BALL_GENERIC_SHADOW0");
        BallImageLoc.Add(@"E:\Unity\Projects\World of Goo 2U 2\res\balls\_generic\shadowCircle49.png");

        BallImageID.Add("IMAGE_BALL_GENERIC_HILITE1");
        BallImageLoc.Add(@"E:\Unity\Projects\World of Goo 2U 2\res\balls\_generic\hilite16.png");

        BallImageID.Add("IMAGE_BALL_GENERIC_HILITE2");
        BallImageLoc.Add(@"E:\Unity\Projects\World of Goo 2U 2\res\balls\_generic\hilite8.png");

        BallImageID.Add("IMAGE_BALL_GENERIC_ARM_INACTIVE");
        BallImageLoc.Add(@"E:\Unity\Projects\World of Goo 2U 2\res\balls\_generic\spring_goo_inactive.png");



        #endregion

        #region gooball
        XmlDocument gooball = new XmlDocument();
        gooball.Load(@"E:\Unity\Projects\World of Goo 2U 2\res\balls\" + type + @"\" + "balls.xml.xml");

        //XmlNodeList gData = gooball.single
        XmlNode ballStats = gooball.SelectSingleNode("/ball");
        string name = ballStats.Attributes["name"].Value;
        string shapeE = ballStats.Attributes["shape"].Value;
        string[] shapeProps = shapeE.Split(',');
        string shape = shapeProps[0];
        float diameter = 0;
        float variation = 0;
        if (shape == "circle")
        {
             diameter = float.Parse(shapeProps[1]);
            if (shapeProps.Length > 3 && shapeProps[2] != null)
            {
                variation = float.Parse(shapeProps[2]);
            }
        }
        float width = 0f;
        float height = 0f;
        int strands = 0;
        strands = int.Parse(ballStats.Attributes["strands"].Value);
        if (shape == "rectangle")
        {
            width = float.Parse(shapeProps[1]);
            height = float.Parse(shapeProps[2]);
            if (shapeProps.Length > 4 && shapeProps[3] != null)
            {
                variation = float.Parse(shapeProps[3]);
            }
        }

        float mass = float.Parse(ballStats.Attributes["mass"].Value);
        float towermass = float.Parse(ballStats.Attributes["towermass"].Value); // when a ball is climbing on a structure its mass is diffirent
        
        //insert marker stuff here

        //end inserting marker stuff here

        //insert particle stuff here

        //strand stuff here

        //detachstrand stuff aswell

        //splat(whatever that means lol)

        //gooball body
        XmlNodeList ballBody = gooball.SelectNodes("/ball/part");

        GameObject GooBall = new GameObject();
        GooBall.name = type + " ID: " + id;
        x = x / 100;
        y = y / 100;
        GooBall.transform.position = new Vector3(x, y, -0.1f);
        GooBall.AddComponent<Rigidbody2D>().gravityScale = 0;
        GooBall.GetComponent<Rigidbody2D>().mass = mass;

        diameter = diameter / 2;
        diameter = diameter / 100;
        if (shape == "circle")
            GooBall.AddComponent<CircleCollider2D>().radius = diameter;
        GooBall.AddComponent<GooBallController>();
        for (int i = 0; i < ballBody.Count; i++)
        {
            string ballName = ballBody.Item(i).Attributes["name"].Value;
            string X = ballBody.Item(i).Attributes["x"].Value;
            string Y = ballBody.Item(i).Attributes["y"].Value;
            float ballX = 0;
            float ballY = 0;
            if (X.Contains(","))
            {
                //Random rnd = new Random();
                string[] xCoord = X.Split(',');
                ballX = Random.Range(float.Parse(xCoord[0]), float.Parse(xCoord[1]));
            }

            if (Y.Contains(","))
            {
                //Random rnd = new Random();
                string[] xCoord = X.Split(',');
                ballY = Random.Range(float.Parse(xCoord[0]), float.Parse(xCoord[1]));
            }
            bool draggable = true;
            if (gooball.SelectSingleNode("/ball/@draggable") != null)
                draggable = bool.Parse(gooball.SelectSingleNode("/ball/@draggable").Value);
            //ballY = float.Parse(Y);
            float scale = float.Parse(ballBody.Item(i).Attributes["scale"].Value);
            string image = ballBody.Item(i).Attributes["image"].Value;
            //add state support when gooballs are actually finished lol
            string rotate = "false";
            if (ballBody.Item(i).Attributes["rotate"] != null)
                rotate = ballBody.Item(i).Attributes["rotate"].Value;
            //make this work later VVV
            string stretch = "none";
            if (ballBody.Item(i).Attributes["stretch"] != null)
            {
                string strech = ballBody.Item(i).Attributes["stretch"].Value;
                string[] stretchParams = strech.Split(',');
                float stretchSpeed = float.Parse(stretchParams[0]);
                float stretchDirectionScale = float.Parse(stretchParams[1]);
                float stretchAcrossScale = float.Parse(stretchParams[2]);
            }
            //make this work later ^^^
            GameObject gooballPart = new GameObject();
            string eye = "none";
            string pupil = "none";
            string pupilInset = "none";
            if (ballBody.Item(i).Attributes["eye"] != null)
            {
                eye = ballBody.Item(i).Attributes["eye"].Value;
                pupil = ballBody.Item(i).Attributes["pupil"].Value;
                pupilInset = ballBody.Item(i).Attributes["pupilinset"].Value;
                GameObject pupilObj = new GameObject();
                byte[] imageData2 = File.ReadAllBytes(LoadBallImageID(pupil));
                Texture2D TextureEE = new Texture2D(2, 2);
                TextureEE.LoadImage(imageData2);
                Sprite SpriteE = Sprite.Create(TextureEE, new Rect(0.0f, 0.0f, TextureEE.width, TextureEE.height), new Vector2(0.5f, 0.5f));
                pupilObj.AddComponent<SpriteRenderer>().sprite = SpriteE;
                pupilObj.transform.SetParent(gooballPart.transform);
                pupilObj.name = "pupil";
                pupilObj.transform.localPosition = new Vector3(0, 0, -1);
            }
            
            gooballPart.name = ballName;
            gooballPart.transform.SetParent(GooBall.transform);
            Texture2D TextureE = new Texture2D(2, 2);
            if (image.Contains(","))
            {
                string[] images = image.Split(',');
                image = images[Random.Range(0, images.Length - 1)];
            }
            byte[] imageData = File.ReadAllBytes(LoadBallImageID(image));
            TextureE.LoadImage(imageData);
            Sprite SpritE = Sprite.Create(TextureE, new Rect(0.0f, 0.0f, TextureE.width, TextureE.height), new Vector2(0.5f, 0.5f));
            gooballPart.AddComponent<SpriteRenderer>().sprite = SpritE;
            gooballPart.transform.localScale = new Vector3(gooballPart.transform.localScale.x * scale, gooballPart.transform.localScale.y * scale, gooballPart.transform.localScale.z * scale);
            int Z = i * -1;
            gooballPart.transform.localPosition = new Vector3(ballX / 100, ballY / 100, Z);
            GooBall.GetComponent<GooBallController>().Strands = strands;

        }
        #endregion gooball

        #region strand
        XmlNodeList strandParts = gooball.SelectNodes("/ball/strand");
        for (int i = 0; i < strandParts.Count; i++)
        {
            string sType = strandParts.Item(i).Attributes["type"].Value;
            string sImage = strandParts.Item(i).Attributes["image"].Value;
            string sInImage = strandParts.Item(i).Attributes["inactiveimage"].Value;
            string sSpringConstMin = strandParts.Item(i).Attributes["springconstmin"].Value; //Defines the spring constant of the strand. The original balls used values around (6-10). Low values(1 to 5) produce weak and wobbly strands.High Values(11 - 20) produce very strong solid strands.Values above 20 cause the "Shaking Bug" to occur, even with only a few balls attached. The operation of these values is quite complex and can produce "unexpected" behaviour.The original balls used the same value for both min and max, it is highly recommended that you do the same. 
            string sSpringConstMax = strandParts.Item(i).Attributes["springconstmax"].Value; //Defines the spring constant of the strand. The original balls used values around (6-10). Low values(1 to 5) produce weak and wobbly strands.High Values(11 - 20) produce very strong solid strands.Values above 20 cause the "Shaking Bug" to occur, even with only a few balls attached. The operation of these values is quite complex and can produce "unexpected" behaviour.The original balls used the same value for both min and max, it is highly recommended that you do the same. 
            string sDampFac = strandParts.Item(i).Attributes["dampfac"].Value; // Dampening Factor that applies to the strand's length.Set to low values(< 0.1) strands will continue to wobble for a long time after they are attached or hit by a flying ball.Set to high values(> 0.7) this wooble is reduce very quickly.Most original balls used values around 0.9 Balloons used 0.002
            float sMaxLen2 = float.Parse(strandParts.Item(i).Attributes["maxlen2"].Value);
            float sMaxLen1 = float.Parse(strandParts.Item(i).Attributes["maxlen1"].Value);
            float sMinLen = float.Parse(strandParts.Item(i).Attributes["minlen"].Value);
            sMaxLen2 = sMaxLen2 / 100; sMaxLen1 = sMaxLen1 / 100; sMinLen = sMinLen / 100;

            Texture2D TextureE = new Texture2D(2, 2);
            byte[] imageData = File.ReadAllBytes(LoadBallImageID(sImage));
            TextureE.LoadImage(imageData);
            //Sprite SpritE = Sprite.Create(TextureE, new Rect(0.0f, 0.0f, TextureE.width, TextureE.height), new Vector2(0.5f, 1f)); // OLD METHOD

            Sprite SpritE = Sprite.Create(TextureE, new Rect(0.0f, 0.0f, TextureE.width, TextureE.height), new Vector2(0.5f, 0.5f)); // NEW METHOD

            GooBall.GetComponent<GooBallController>().texture = TextureE;
            GooBall.GetComponent<GooBallController>().image = imageData;
            GooBall.GetComponent<GooBallController>().spriteE = SpritE;

            GooBall.GetComponent<GooBallController>().maxLen2 = sMaxLen2;
            GooBall.GetComponent<GooBallController>().maxLen1 = sMaxLen1;
            GooBall.GetComponent<GooBallController>().minLen = sMinLen;
            GooBall.GetComponent<GooBallController>().sImage = sImage;

            GooBall.GetComponent<GooBallController>().sInactiveImage = sInImage;
            

        }
        float dampFac = float.Parse(gooball.SelectSingleNode("/ball/strand").Attributes["dampfac"].Value);
        float springConstMin = float.Parse(gooball.SelectSingleNode("/ball/strand").Attributes["springconstmin"].Value);
        GooBall.GetComponent<GooBallController>().dampFac = dampFac;
        GooBall.GetComponent<GooBallController>().springConst = springConstMin;
        gooballs.Add(GooBall);
        //GooBall.layer = LayerMask.NameToLayer("Strand");
        #endregion

        #region gooball creation


        #endregion
    }

    public string LoadBallImageID(string id)
    {
        string location = "none";

        for (int i = 0; i <= BallImageID.Count; i++)
        {
            if (id == BallImageID[i])
            {
                location = BallImageLoc[i];
                return location;
            }


        }

        return location;
    }

}
