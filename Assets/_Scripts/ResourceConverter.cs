using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using UnityEngine;

public class ResourceConverter
{
    public static string CreateDecryptedFile(string infile){
        byte[] KEY = {0x0D, 0x06, 0x07, 0x07, 0x0C, 0x01, 0x08, 0x05,
            0x06, 0x09, 0x09, 0x04, 0x06, 0x0D, 0x03, 0x0F,
            0x03, 0x06, 0x0E, 0x01, 0x0E, 0x02, 0x07, 0x0B};
        byte[] IV = {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
        Aes aes = Aes.Create();
        aes.Key = KEY;
        aes.IV = IV;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.Zeros;
        var decryptor = aes.CreateDecryptor();
        string plaintext = "";
        using(var inStream = new BinaryReader(File.Open(infile, FileMode.Open))){
            using(var csDecrypt = new CryptoStream(inStream.BaseStream, decryptor, CryptoStreamMode.Read)){
                using(var sr = new StreamReader(csDecrypt)){
                    while(!sr.EndOfStream){
                        int val = sr.Read();
                        plaintext += (char)val;
                    }
                }
            }
        }
        string finalstr = Encoding.UTF8.GetString(new byte[] {
            0xEF, 0xBF, 0xBD
        });
        for(int i = plaintext.Length-1-64; i < plaintext.Length; i++){
            var substr = plaintext.Substring(i, 1);
            if(substr == finalstr)
                return plaintext.Substring(0, i);
        }
        return plaintext;
    }
    public static JSONLevel ConvertEncryptedXMLLevelToJSON(string scenefile, string resrcfile, string levelfile){
        JSONLevel lvl = new JSONLevel();
        string level = CreateDecryptedFile(levelfile);
        string scene = CreateDecryptedFile(scenefile);
        string resrc = CreateDecryptedFile(resrcfile);
        Debug.Log("Decrypted level files.");
        Debug.Log("level.xml.bin:");
        Debug.Log(level);
        Debug.Log("scene.xml.bin:");
        Debug.Log(scene);
        Debug.Log("resrc.xml.bin:");
        Debug.Log(resrc);
        lvl.level = CreateJSONLevelDataFromXMLStream(new MemoryStream(Encoding.UTF8.GetBytes(level)));
        lvl.scene = CreateJSONLevelSceneDataFromXMLStream(new MemoryStream(Encoding.UTF8.GetBytes(scene)));
        lvl.resrc = CreateJSONLevelResrcDataFromXMLStream(new MemoryStream(Encoding.UTF8.GetBytes(resrc)));
        return lvl;
    }
    public static JSONLevel ConvertXMLLevelToJSON(string scenefile, string resrcfile, string levelfile){
        JSONLevel lvl = new JSONLevel();
        lvl.level = CreateJSONLevelDataFromXMLStream(new StreamReader(levelfile).BaseStream);
        lvl.scene = CreateJSONLevelSceneDataFromXMLStream(new StreamReader(scenefile).BaseStream);
        lvl.resrc = CreateJSONLevelResrcDataFromXMLStream(new StreamReader(resrcfile).BaseStream);
        return lvl;
    }
    
    private static float DivFac = 100f;
    private static Resrc CreateJSONLevelResrcDataFromXMLStream(Stream inStream)
    {
        Resrc resrc = new Resrc();
        XmlDocument xml = new XmlDocument();
        XmlTextReader resrcReader = new XmlTextReader(inStream);
        resrcReader.Read();
        xml.Load(resrcReader);

        XmlNode root = xml.SelectSingleNode("/Resources");
        if(root == null)
            root = xml.SelectSingleNode("/ResourceManifest/Resources");
        Dictionary<string, string> resources = new Dictionary<string, string>();
        XmlNodeList images = root.SelectNodes("Image");
        foreach(XmlNode image in images){
            try{
                string path = image.Attributes["path"].Value + ".png";
                if(path.StartsWith("res/"))
                    path = path.Substring(4);
                resources.Add(image.Attributes["id"].Value, path);
            } catch{}
        }
        XmlNodeList sounds = root.SelectNodes("Sound");
        foreach(XmlNode sound in sounds){
            try{
                string path = sound.Attributes["path"].Value + ".ogg";
                if(path.StartsWith("res/"))
                    path = path.Substring(4);
                resources.Add(sound.Attributes["id"].Value, path);
            } catch{}
        }
        resrc.resources = resources;
        inStream.Dispose();
        resrcReader.Dispose();
        return resrc;
    }

    private static Level CreateJSONLevelDataFromXMLStream(Stream inStream)
    {
        Level level = new Level();
        XmlDocument xml = new XmlDocument();
        XmlTextReader levelReader = new XmlTextReader(inStream);
        levelReader.Read();
        xml.Load(levelReader);

        XmlNode root = xml.SelectSingleNode("/level");
        try{level.ballsrequired = int.Parse(root.Attributes["ballsrequired"].Value);} catch{}
        try{level.letterboxed = bool.Parse(root.Attributes["letterboxed"].Value);} catch{}
        try{level.visualdebug = bool.Parse(root.Attributes["visualdebug"].Value);} catch{}
        try{level.autobounds = bool.Parse(root.Attributes["autobounds"].Value);} catch{}
        try{
            float[] colors = root.Attributes["textcolor"].Value.Split(",").Select(s => float.Parse(s, CultureInfo.InvariantCulture)).ToArray();
            level.textcolor = new(colors[0], colors[1], colors[2]);
        } catch{}
        try{level.timebugprobability = float.Parse(root.Attributes["timebugprobability"].Value, CultureInfo.InvariantCulture);} catch{}
        try{level.strandgeom = bool.Parse(root.Attributes["strandgeom"].Value);} catch{}
        try{level.allowskip = bool.Parse(root.Attributes["allowskip"].Value);} catch{}
        XmlNodeList cameras = root.SelectNodes("camera");
        float zoomMult = 3f;
        foreach(XmlNode camera in cameras){
            if(camera.Attributes["aspect"].Value == "widescreen"){
                CameraData data = new CameraData();
                bool hasEndPos = true;
                try{
                    float[] pos = camera.Attributes["endpos"].Value.Split(",").Select(s => float.Parse(s, CultureInfo.InvariantCulture)).ToArray();
                    data.endpos = new(pos[0] / DivFac, pos[1] / DivFac);
                } catch{data.endpos = new(0,0); hasEndPos = false;}
                try{data.endzoom = float.Parse(camera.Attributes["endzoom"].Value, CultureInfo.InvariantCulture) * zoomMult;} catch{data.endzoom = zoomMult;}
                XmlNodeList pois = camera.SelectNodes("poi");
                data.poi = new Poi[pois.Count];
                for(int i = 0; i < pois.Count; i++){
                    Poi poi = new Poi();
                    try{
                        float[] pos = pois[i].Attributes["pos"].Value.Split(",").Select(s => float.Parse(s, CultureInfo.InvariantCulture)).ToArray();
                        poi.pos = new(pos[0] / DivFac, pos[1] / DivFac);
                    } catch{}
                    try{poi.traveltime = float.Parse(pois[i].Attributes["traveltime"].Value, CultureInfo.InvariantCulture);} catch{}
                    try{poi.pause = float.Parse(pois[i].Attributes["pause"].Value, CultureInfo.InvariantCulture);} catch{}
                    try{poi.zoom = float.Parse(pois[i].Attributes["zoom"].Value, CultureInfo.InvariantCulture) * zoomMult;} catch{}
                    data.poi[i] = poi;
                    if(i == pois.Count-1)
                        data.endpos = poi.pos;
                }
                level.camera = data;
            }
        }
        try{level.music = new Music() { id = root.SelectSingleNode("music").Attributes["id"].Value}; } catch{}
        //TODO: FIRE
        //TODO: SIGNPOSTS
        try{
            XmlNode pipe = root.SelectSingleNode("pipe");
            level.pipe = new();
            level.pipe.id = pipe.Attributes["id"].Value;
            level.pipe.depth = float.Parse(pipe.Attributes["depth"].Value);
            XmlNodeList vertices = pipe.SelectNodes("vertex");
            level.pipe.Vertex = new Vertex[vertices.Count];
            for(int i = 0; i < vertices.Count; i++){
                level.pipe.Vertex[i] = new();
                try{level.pipe.Vertex[i].x = float.Parse(vertices[i].Attributes["x"].Value, CultureInfo.InvariantCulture) / DivFac;} catch{}
                try{level.pipe.Vertex[i].y = float.Parse(vertices[i].Attributes["y"].Value, CultureInfo.InvariantCulture) / DivFac;} catch{}
            }
        } catch{}

        XmlNodeList instances = root.SelectNodes("BallInstance");
        level.BallInstance = new Ballinstance[instances.Count];
        for(int i = 0; i < instances.Count; i++){
            Ballinstance ballinstance = new Ballinstance();
            try{ballinstance.type = instances[i].Attributes["type"].Value;} catch{}
            try{ballinstance.id = instances[i].Attributes["id"].Value;} catch{}
            try{ballinstance.angle = float.Parse(instances[i].Attributes["angle"].Value, CultureInfo.InvariantCulture);} catch{}
            try{
                float x = float.Parse(instances[i].Attributes["x"].Value, CultureInfo.InvariantCulture);
                float y = float.Parse(instances[i].Attributes["y"].Value, CultureInfo.InvariantCulture);
                ballinstance.pos = new(x / DivFac, y / DivFac);
            } catch{}
            try{ballinstance.discovered = bool.Parse(instances[i].Attributes["discovered"].Value);} catch{}
            level.BallInstance[i] = ballinstance;
        }
        
        XmlNodeList strands = root.SelectNodes("Strand");
        level.Strand = new LStrand[strands.Count];
        for(int i = 0; i < strands.Count; i++){
            LStrand strand = new LStrand();
            try{strand.gb1 = strands[i].Attributes["gb1"].Value;} catch{}
            try{strand.gb2 = strands[i].Attributes["gb2"].Value;} catch{}
            level.Strand[i] = strand;
        }

        try{
            XmlNode exit = root.SelectSingleNode("levelexit");
            level.levelexit = new Levelexit();
            try{level.levelexit.filter = exit.Attributes["filter"].Value;} catch{}
            try{level.levelexit.id = exit.Attributes["id"].Value;} catch{}
            try{
                float[] pos = exit.Attributes["pos"].Value.Split(",").Select(s => float.Parse(s, CultureInfo.InvariantCulture)).ToArray();
                level.levelexit.pos = new(pos[0], pos[1]);
            } catch{}
            try{level.levelexit.radius = float.Parse(exit.Attributes["radius"].Value, CultureInfo.InvariantCulture);} catch{}
        } catch{}
        //TODO: the rest of the stuff
        inStream.Dispose();
        levelReader.Dispose();
        return level;
    }

    private static Scene CreateJSONLevelSceneDataFromXMLStream(Stream inStream){
        Scene scene = new Scene();
        XmlDocument xml = new XmlDocument();
        XmlTextReader sceneReader = new XmlTextReader(inStream);
        sceneReader.Read();
        xml.Load(sceneReader);

        XmlNode root = xml.SelectSingleNode("/scene");
        string[] backgroundrgb = root.Attributes["backgroundcolor"].Value.Split(",");
        ColorData colorData = new(
            int.Parse(backgroundrgb[0]), 
            int.Parse(backgroundrgb[1]), 
            int.Parse(backgroundrgb[2]));
        scene.backgroundcolor = colorData;
        string[] optionalrootvars = {"minx", "miny", "maxx", "maxy"};
        Dictionary<string, float> optionalrootvardict = new Dictionary<string, float>();
        foreach(var optionalvar in optionalrootvars)
        try{
            optionalrootvardict.Add(optionalvar, float.Parse(root.Attributes[optionalvar].Value, CultureInfo.InvariantCulture)/DivFac);
        } catch {}
        scene.minX = optionalrootvardict.GetValueOrDefault("minx");
        scene.minY = optionalrootvardict.GetValueOrDefault("miny");
        scene.maxX = optionalrootvardict.GetValueOrDefault("maxx");
        scene.maxY = optionalrootvardict.GetValueOrDefault("maxy");

        XmlNodeList linearforcefields = root.SelectNodes("linearforcefield");
        scene.ForceFields.linearforcefields = new LinearForceField[linearforcefields.Count];
        for(int i = 0; i < linearforcefields.Count; i++){
            XmlNode forcefield = linearforcefields[i];
            LinearForceField field = new LinearForceField();
            try{field.type = forcefield.Attributes["type"].Value; } catch {}
            try{field.id = forcefield.Attributes["id"].Value; } catch {}
            try{
            string[] force = forcefield.Attributes["force"].Value.Split(",");
            field.force = new Position(
                float.Parse(force[0], CultureInfo.InvariantCulture), 
                float.Parse(force[1], CultureInfo.InvariantCulture)); } catch {}
            try{field.dampeningfactor = float.Parse(forcefield.Attributes["dampeningfactor"].Value, CultureInfo.InvariantCulture);} catch{}
            try{field.antigrav = bool.Parse(forcefield.Attributes["antigrav"].Value);} catch{}
            try{field.geomonly = bool.Parse(forcefield.Attributes["geomonly"].Value);} catch{}
            //TODO: IMPLEMENT THE REST OF THE LINEAR FORCEFIELD FEATURE CONVERSIONS
            scene.ForceFields.linearforcefields[i] = field;
        }
        XmlNodeList radialforcefields = root.SelectNodes("radialforcefield");
        scene.ForceFields.radialforcefields = new RadialForceField[radialforcefields.Count];
        for(int i = 0; i < radialforcefields.Count; i++){
            XmlNode forcefield = radialforcefields[i];
            RadialForceField field = new RadialForceField();
            try{field.type = forcefield.Attributes["type"].Value;} catch{}
            try{field.id = forcefield.Attributes["id"].Value;} catch{}
            try{
                float[] pos = forcefield.Attributes["center"].Value.Split(",").Select(s => float.Parse(s, CultureInfo.InvariantCulture)).ToArray();
                field.center = new(pos[0]/DivFac, pos[1]/DivFac);
            } catch{}
            try{field.radius = float.Parse(forcefield.Attributes["radius"].Value, CultureInfo.InvariantCulture)/DivFac;} catch{}
            try{field.forceatcenter = float.Parse(forcefield.Attributes["forceatcenter"].Value, CultureInfo.InvariantCulture);} catch{}
            try{field.forceatedge = float.Parse(forcefield.Attributes["forceatedge"].Value, CultureInfo.InvariantCulture);} catch{}
            try{field.dampeningfactor = float.Parse(forcefield.Attributes["dampeningfactor"].Value, CultureInfo.InvariantCulture);} catch{}
            try{field.antigrav = bool.Parse(forcefield.Attributes["antigrav"].Value);} catch{}
            try{field.geomonly = bool.Parse(forcefield.Attributes["geomonly"].Value);} catch{}
            try{field.enabled = bool.Parse(forcefield.Attributes["enabled"].Value);} catch{}
            scene.ForceFields.radialforcefields[i] = field;
        }
        XmlNodeList particles = root.SelectNodes("particles");
        scene.particles = new Particle[particles.Count];
        for (int i = 0; i < particles.Count; i++)
        {
            Particle p = new Particle();
            XmlNode particle = particles[i];
            try {p.effect = particle.Attributes["effect"].Value; } catch { }
            try {p.depth = float.Parse(particle.Attributes["depth"].Value, CultureInfo.InvariantCulture);} catch {}
            try {p.pretick = float.Parse(particle.Attributes["pretick"].Value, CultureInfo.InvariantCulture);} catch {}
            //TODO: IMPLEMENT THE REST OF THE PARTICLE FEATURE CONVERSIONS
            scene.particles[i] = p;
        }

        XmlNodeList scenelayers = root.SelectNodes("SceneLayer");
        scene.scenelayers = new Scenelayer[scenelayers.Count];
        for(int i = 0; i < scenelayers.Count; i++){
            XmlNode layer = scenelayers[i];
            Scenelayer scenelayer = new Scenelayer();
            try{scenelayer.name = layer.Attributes["name"].Value;} catch{};
            try{scenelayer.depth = float.Parse(layer.Attributes["depth"].Value, CultureInfo.InvariantCulture);} catch{}
            try{
                float x = float.Parse(layer.Attributes["x"].Value, CultureInfo.InvariantCulture) / DivFac;
                float y = float.Parse(layer.Attributes["y"].Value, CultureInfo.InvariantCulture) / DivFac;
                scenelayer.pos = new(x, y);
            } catch{}
            try{scenelayer.scaleX = float.Parse(layer.Attributes["scalex"].Value,CultureInfo.InvariantCulture);} catch{}
            try{scenelayer.scaleY = float.Parse(layer.Attributes["scaley"].Value,CultureInfo.InvariantCulture);} catch{}
            try{scenelayer.tileX = bool.Parse(layer.Attributes["tilex"].Value);} catch{}
            try{scenelayer.tileY = bool.Parse(layer.Attributes["tiley"].Value);} catch{}
            try{scenelayer.rotation = float.Parse(layer.Attributes["rotation"].Value,CultureInfo.InvariantCulture);} catch{}
            try{scenelayer.alpha = float.Parse(layer.Attributes["alpha"].Value,CultureInfo.InvariantCulture);} catch{}
            try{
                float[] colors = layer.Attributes["colorize"].Value.Split(",").Select(s => float.Parse(s, CultureInfo.InvariantCulture)).ToArray();
                scenelayer.colorize = new(colors[0], colors[1], colors[2]);
            } catch{}
            try{scenelayer.image = layer.Attributes["image"].Value;} catch{};
            //TODO: implement tileX and tileY?
            scene.scenelayers[i] = scenelayer;
        }
        XmlNodeList buttongroups = root.SelectNodes("buttongroup");
        scene.buttongroups = new Buttongroup[buttongroups.Count];
        for(int i = 0; i < buttongroups.Count; i++){
            XmlNode group = buttongroups[i];
            Buttongroup buttongroup = new Buttongroup{ buttons = ReadButtons(group)};
            scene.buttongroups[i] = buttongroup;
        }
        scene.buttons = ReadButtons(root);

        //TODO: IMPLEMENT LABELS
        List<LevelGeometry> levelGeometries = new List<LevelGeometry>();
        XmlNodeList circles = root.SelectNodes("circle");
        foreach(XmlNode circle in circles){
            LevelGeometry geom = new LevelGeometry();
            geom.type = LevelGeometry.Type.Circle;
            try{geom.id = circle.Attributes["id"].Value;} catch{}
            try{geom.tag = circle.Attributes["tag"].Value;} catch{}
            try{geom.material = circle.Attributes["material"].Value;} catch{}
            try{geom.dynamic = !bool.Parse(circle.Attributes["static"].Value);} catch{}
            try{
                float x = float.Parse(circle.Attributes["x"].Value, CultureInfo.InvariantCulture) / DivFac;
                float y = float.Parse(circle.Attributes["y"].Value, CultureInfo.InvariantCulture) / DivFac;
                geom.center = new(x, y);
            } catch{}
            try{geom.radius = float.Parse(circle.Attributes["radius"].Value, CultureInfo.InvariantCulture) / DivFac;} catch{}
            //TODO: IMPLEMENT REST OF PROPERTIES
            levelGeometries.Add(geom);
        }

        XmlNodeList rectangles = root.SelectNodes("rectangle");
        foreach(XmlNode rectangle in rectangles){
            LevelGeometry geom = new LevelGeometry();
            geom.type = LevelGeometry.Type.Rectangle;
            try{geom.id = rectangle.Attributes["id"].Value;} catch{}
            try{geom.tag = rectangle.Attributes["tag"].Value;} catch{}
            try{geom.material = rectangle.Attributes["material"].Value;} catch{}
            try{geom.dynamic = !bool.Parse(rectangle.Attributes["static"].Value);} catch{}
            try{
                float x = float.Parse(rectangle.Attributes["x"].Value, CultureInfo.InvariantCulture) / DivFac;
                float y = float.Parse(rectangle.Attributes["y"].Value, CultureInfo.InvariantCulture) / DivFac;
                geom.center = new(x, y);
            } catch{}
            try{
                float width = float.Parse(rectangle.Attributes["width"].Value, CultureInfo.InvariantCulture) / DivFac;
                float height = float.Parse(rectangle.Attributes["height"].Value, CultureInfo.InvariantCulture) / DivFac;
                geom.size = new(width, height);
            } catch{}
            // this is in radians for some reason
            try{geom.rotation = float.Parse(rectangle.Attributes["rotation"].Value, CultureInfo.InvariantCulture) * 180 / Mathf.PI;} catch{}
            //TODO: IMPLEMENT REST OF PROPERTIES
            levelGeometries.Add(geom);
        }
        //TODO: IMPLEMENT COMPOSITEGEOMS
        scene.compositegeoms = new Compositegeom[0];
        scene.geometries = levelGeometries.ToArray();
        
        XmlNodeList lines = root.SelectNodes("line");
        scene.lines = new Line[lines.Count];
        for(int i = 0; i < lines.Count; i++){
            Line line = new Line();
            try{line.id = lines[i].Attributes["id"].Value;} catch{}
            try{line.tag = lines[i].Attributes["tag"].Value;} catch{}
            try{line.material = lines[i].Attributes["material"].Value;} catch{}
            try{
                float[] pos = lines[i].Attributes["anchor"].Value.Split(",").Select(s => float.Parse(s, CultureInfo.InvariantCulture)).ToArray();
                line.anchor = new(pos[0] / DivFac, pos[1] / DivFac);
            } catch{}
            try{
                float[] pos = lines[i].Attributes["normal"].Value.Split(",").Select(s => float.Parse(s, CultureInfo.InvariantCulture)).ToArray();
                line.normal = new(pos[0], pos[1]);
            } catch{}
            scene.lines[i] = line;
        }
        inStream.Dispose();
        sceneReader.Dispose();
        return scene;
    }

    public static Button[] ReadButtons(XmlNode baseNode){
        XmlNodeList buttons = baseNode.SelectNodes("button");
        Button[] btns = new Button[buttons.Count];
        for(int j = 0; j < buttons.Count; j++){
            XmlNode node = buttons[j];
            Button button = new Button();
            try{button.id = node.Attributes["id"].Value;} catch{}
            try{button.up = node.Attributes["up"].Value;} catch{}
            try{button.over = node.Attributes["over"].Value;} catch{}
            try{button.onmouseenter = node.Attributes["onmouseenter"].Value;} catch{}
            try{button.onmouseexit = node.Attributes["onmouseexit"].Value;} catch{}
            try{button.disabled = node.Attributes["disabled"].Value;} catch{}
            try{button.onclick = node.Attributes["onclick"].Value;} catch{}
            try{
                float x = float.Parse(node.Attributes["x"].Value, CultureInfo.InvariantCulture) / DivFac;
                float y = float.Parse(node.Attributes["y"].Value, CultureInfo.InvariantCulture) / DivFac;
                button.pos = new(x, y);
            } catch{}
            try{button.scaleX = float.Parse(node.Attributes["scalex"].Value, CultureInfo.InvariantCulture);} catch{}
            try{button.scaleY = float.Parse(node.Attributes["scaley"].Value, CultureInfo.InvariantCulture);} catch{}
            try{button.rotation = float.Parse(node.Attributes["rotation"].Value, CultureInfo.InvariantCulture);} catch{}
            try{button.depth = float.Parse(node.Attributes["depth"].Value, CultureInfo.InvariantCulture);} catch{}
            try{
                float[] pos = node.Attributes["colorize"].Value.Split(",").Select(s => float.Parse(s, CultureInfo.InvariantCulture)).ToArray();
                button.colorize = new(pos[0], pos[1], pos[2]);
            } catch{}
            btns[j] = button;
        }
        return btns;
    }
    public static JSONGooball ConvertXMLBallToJSON(string ballfile, string resrcfile)
    {
        JSONGooball ball = ConvertXMLBallDataFromXMLStream(new StreamReader(ballfile).BaseStream);
        ball.resrc = ConvertXMLBallResourcesFromXMLStream(new StreamReader(resrcfile).BaseStream);
        return ball;
    }
    public static JSONGooball ConvertEncryptedXMLBallToJSON(string ballfile, string resrcfile)
    {
        string ballxml = CreateDecryptedFile(ballfile);
        string resrcxml = CreateDecryptedFile(resrcfile);
        Debug.Log("Decrypted gooball files.");
        Debug.Log("balls.xml.bin:");
        Debug.Log(ballxml);
        Debug.Log("resources.xml.bin:");
        Debug.Log(resrcxml);
        JSONGooball ball = ConvertXMLBallDataFromXMLStream(new MemoryStream(Encoding.UTF8.GetBytes(ballxml)));
        ball.resrc = ConvertXMLBallResourcesFromXMLStream(new MemoryStream(Encoding.UTF8.GetBytes(resrcxml)));
        return ball;
    }
    private static Resrc ConvertXMLBallResourcesFromXMLStream(Stream inStream)
    {
        Resrc resrc = new Resrc();
        XmlDocument xml = new XmlDocument();
        XmlTextReader resrcReader = new XmlTextReader(inStream);
        resrcReader.Read();
        xml.Load(resrcReader);
        XmlNode root = xml.SelectSingleNode("/Resources");
        if(root == null)
            root = xml.SelectSingleNode("/ResourceManifest/Resources");

        try{resrc.id = root.Attributes["id"].Value;} catch{}
        string idprefix = "";
        string pathprefix = "";
        foreach(XmlNode node in root.ChildNodes){
            switch(node.Name){
                case "SetDefaults":
                    try{idprefix = node.Attributes["idprefix"].Value;} catch{}
                    try{pathprefix = node.Attributes["path"].Value;} catch{}
                    break;
                case "Image":{
                    try{
                    string id = idprefix + node.Attributes["id"].Value;
                    string path = pathprefix + node.Attributes["path"].Value + ".png";
                    if(path.StartsWith("res/"))
                        path = path.Substring(4);
                    resrc.resources.Add(id, path);} catch{}
                }
                    break;
                case "Sound":{
                    try{
                    string id = idprefix + node.Attributes["id"].Value;
                    string path = pathprefix + node.Attributes["path"].Value + ".ogg";
                    if(path.StartsWith("res/"))
                        path = path.Substring(4);
                    resrc.resources.Add(id, path);} catch{}
                }
                    break;
            }
        }

        return resrc;
    }
    private static JSONGooball ConvertXMLBallDataFromXMLStream(Stream inStream){
        JSONGooball jsonball = new JSONGooball();
        XmlDocument xml = new XmlDocument();
        XmlTextReader ballReader = new XmlTextReader(inStream);
        ballReader.Read();
        xml.Load(ballReader);

        XmlNode root = xml.SelectSingleNode("/ball");

        try{jsonball.ball.name = root.Attributes["name"].Value;} catch{}
        try{
            string[] shapestr = root.Attributes["shape"].Value.Split(",");
            if(shapestr[0].ToLower() == "rectangle"){
                jsonball.ball.shape = Ball.Shape.Rectangle;
                jsonball.ball.width = float.Parse(shapestr[1], CultureInfo.InvariantCulture);
                jsonball.ball.height = float.Parse(shapestr[2], CultureInfo.InvariantCulture);
                try{jsonball.ball.sizeVariation = float.Parse(shapestr[3], CultureInfo.InvariantCulture);} catch{}
            } else {
                jsonball.ball.radius = float.Parse(shapestr[1]);
                try{jsonball.ball.sizeVariation = float.Parse(shapestr[2], CultureInfo.InvariantCulture);} catch{}
            }
        } catch{}
        try{jsonball.ball.material = root.Attributes["material"].Value;} catch{}
        try{jsonball.ball.mass = float.Parse(root.Attributes["mass"].Value, CultureInfo.InvariantCulture);} catch{}
        try{jsonball.ball.towerMass = float.Parse(root.Attributes["towermass"].Value, CultureInfo.InvariantCulture);} catch{}
        try{jsonball.ball.strands = int.Parse(root.Attributes["strands"].Value, CultureInfo.InvariantCulture);} catch{}
        try{jsonball.ball.walkSpeed = float.Parse(root.Attributes["walkspeed"].Value, CultureInfo.InvariantCulture);} catch{}
        try{jsonball.ball.climbspeed = float.Parse(root.Attributes["climbspeed"].Value, CultureInfo.InvariantCulture);} catch{}
        try{jsonball.ball.speedvariance = float.Parse(root.Attributes["speedvariance"].Value, CultureInfo.InvariantCulture);} catch{}
        try{jsonball.ball.detachable = bool.Parse(root.Attributes["detachable"].Value);} catch{}
        try{
            float[] jmp = root.Attributes["jump"].Value.Split(",").Select(s => float.Parse(s, CultureInfo.InvariantCulture)).ToArray();
            jsonball.ball.jump = new(jmp[0], jmp[1]);
        } catch{}
        
        XmlNode marker = root.SelectSingleNode("marker");
        try{jsonball.marker.drag = marker.Attributes["drag"].Value;} catch{}
        try{jsonball.marker.detach = marker.Attributes["detach"].Value;} catch{}
        try{jsonball.marker.rotspeed = float.Parse(marker.Attributes["rotspeed"].Value, CultureInfo.InvariantCulture);} catch{}


        XmlNodeList particles = root.SelectNodes("particles");
        jsonball.particles = new Particles[particles.Count];
        for(int i = 0; i < particles.Count; i++){
            XmlNode node = particles[i];
            Particles particle = new Particles();
            try{particle.effect = node.Attributes["id"].Value;} catch{}
            //try{particle.states = node.Attributes["states"].Value.Split(",");} catch{}
            try{particle.overBall = bool.Parse(node.Attributes["overball"].Value);} catch{}
            jsonball.particles[i] = particle;
            //TODO: finish implementing rest of particles
        }

        XmlNode strandnode = root.SelectSingleNode("strand");
        try{
            string type = strandnode.Attributes["type"].Value.ToLower();
            switch(type){
                case "spring":
                    jsonball.strand.type = StrandJSON.Type.Spring;
                    break;
                case "rope":
                    jsonball.strand.type = StrandJSON.Type.Rope;
                    break;
                case "rigid":
                    jsonball.strand.type = StrandJSON.Type.Rigid;
                    break;
            }
        } catch{}
        try{jsonball.strand.image = strandnode.Attributes["image"].Value;} catch{}
        try{jsonball.strand.inactiveImage = strandnode.Attributes["inactiveimage"].Value;} catch{}
        try{jsonball.strand.springConst = new(
            float.Parse(strandnode.Attributes["springconstmin"].Value, CultureInfo.InvariantCulture),
            float.Parse(strandnode.Attributes["springconstmax"].Value, CultureInfo.InvariantCulture));
        } catch{}
        try{jsonball.strand.dampFac = float.Parse(strandnode.Attributes["dampfac"].Value, CultureInfo.InvariantCulture);} catch{}
        try{jsonball.strand.minLen = float.Parse(strandnode.Attributes["minlen"].Value, CultureInfo.InvariantCulture)/150;} catch{}
        try{jsonball.strand.maxLen1 = float.Parse(strandnode.Attributes["maxlen1"].Value, CultureInfo.InvariantCulture)/150;} catch{}
        try{jsonball.strand.maxLen2 = float.Parse(strandnode.Attributes["maxlen2"].Value, CultureInfo.InvariantCulture)/150;} catch{}
        try{jsonball.strand.shrinkLen = float.Parse(strandnode.Attributes["shrinklen"].Value, CultureInfo.InvariantCulture)/150;} catch{}
        try{jsonball.strand.maxForce = float.Parse(strandnode.Attributes["maxforce"].Value, CultureInfo.InvariantCulture);} catch{}
        //TODO: Implement the reading of the strand values that are left

        XmlNode detachstrand = root.SelectSingleNode("detachstrand");
        try{jsonball.detachstrand.image = detachstrand.Attributes["image"].Value;} catch{}
        try{jsonball.detachstrand.maxLen = float.Parse(detachstrand.Attributes["maxlen"].Value, CultureInfo.InvariantCulture);} catch{}

        XmlNode splat = root.SelectSingleNode("splat");
        try{jsonball.splat.images = splat.Attributes["image"].Value.Split(",");} catch{}

        XmlNodeList parts = root.SelectNodes("part");
        jsonball.parts = new Part[parts.Count];
        for(int i = 0; i < parts.Count; i++){
            XmlNode part = parts[i];
            Part jsonpart = new Part();
            try{jsonpart.name = part.Attributes["name"].Value;} catch{}
            try{jsonpart.layer = int.Parse(part.Attributes["layer"].Value);} catch{}
            try{
                float[] x = part.Attributes["x"].Value.Split(",").Select(x => float.Parse(x, CultureInfo.InvariantCulture)).ToArray();
                jsonpart.x = new(x[0], x.Length > 1 ? x[1] : x[0]);
            } catch{}
            try{
                float[] y = part.Attributes["y"].Value.Split(",").Select(x => float.Parse(x, CultureInfo.InvariantCulture)).ToArray();
                jsonpart.y = new(y[0], y.Length > 1 ? y[1] : y[0]);
            } catch{}
            try{jsonpart.image = part.Attributes["image"].Value.Split(",");} catch{}
            try{jsonpart.rotate = bool.Parse(part.Attributes["rotate"].Value);} catch{}
            try{
                float scale = float.Parse(part.Attributes["scale"].Value, CultureInfo.InvariantCulture);
                jsonpart.scale = new(scale, scale);
            } catch{}
            try{
                float[] stretchvals = part.Attributes["stretch"].Value.Split(",").Select(x => float.Parse(x, CultureInfo.InvariantCulture)).ToArray();
                Stretch stretch = new Stretch
                {
                    speed = stretchvals[0],
                    directionScale = stretchvals[1],
                    acrossScale = stretchvals[2]
                };
                jsonpart.stretch = stretch;
            } catch{}
            //TODO: Finish the rest of part reading
            jsonball.parts[i] = jsonpart;
        }

        //TODO: Finish the reading XML, sinvariance and such
        inStream.Dispose();
        ballReader.Dispose();
        return jsonball;
    }
}
