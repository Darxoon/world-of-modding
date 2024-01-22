using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using UnityEngine;

public class ResourceConverter
{
    public static JSONLevel ConvertXMLLevelToJSON(string scenefile, string resrcfile, string levelfile){
        JSONLevel lvl = new JSONLevel();
        lvl.level = CreateJSONLevelDataFromXML(levelfile);
        lvl.scene = CreateJSONLevelSceneDataFromXML(scenefile);
        lvl.resrc = CreateJSONLevelResrcDataFromXML(resrcfile);
        return lvl;
    }
    private static float DivFac = 100f;
    private static Resrc CreateJSONLevelResrcDataFromXML(string resrcfile)
    {
        Resrc resrc = new Resrc();
        XmlDocument xml = new XmlDocument();
        XmlTextReader resrcReader = new XmlTextReader(resrcfile);
        resrcReader.Read();
        xml.Load(resrcReader);

        XmlNode root = xml.SelectSingleNode("/Resources");
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
        return resrc;
    }

    private static Level CreateJSONLevelDataFromXML(string levelfile)
    {
        Level level = new Level();
        XmlDocument xml = new XmlDocument();
        XmlTextReader levelReader = new XmlTextReader(levelfile);
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
        foreach(XmlNode camera in cameras){
            if(camera.Attributes["aspect"].Value == "widescreen"){
                CameraData data = new CameraData();
                try{
                    float[] pos = camera.Attributes["endpos"].Value.Split(",").Select(s => float.Parse(s, CultureInfo.InvariantCulture)).ToArray();
                    data.endpos = new(pos[0] / DivFac, pos[1] / DivFac);
                } catch{}
                try{data.endzoom = float.Parse(camera.Attributes["endzoom"].Value, CultureInfo.InvariantCulture) * 2f;} catch{}
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
                    try{poi.zoom = float.Parse(pois[i].Attributes["zoom"].Value, CultureInfo.InvariantCulture) * 2f;} catch{}
                    data.poi[i] = poi;
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
                try{level.pipe.Vertex[i].x = float.Parse(vertices[i].Attributes["x"].Value, CultureInfo.InvariantCulture);} catch{}
                try{level.pipe.Vertex[i].y = float.Parse(vertices[i].Attributes["y"].Value, CultureInfo.InvariantCulture);} catch{}
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
        return level;
    }

    private static Scene CreateJSONLevelSceneDataFromXML(string sceneFile){
        Scene scene = new Scene();
        XmlDocument xml = new XmlDocument();
        XmlTextReader sceneReader = new XmlTextReader(sceneFile);
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
            optionalrootvardict.Add(optionalvar, float.Parse(root[optionalvar].Value, CultureInfo.InvariantCulture));
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
        //TODO: IMPLEMENT RADIALFORCEFIELD CONVERSION

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
        //TODO: IMPLEMENT BUTTONGROUPS
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
                float x = float.Parse(circle.Attributes["x"].Value) / DivFac;
                float y = float.Parse(circle.Attributes["y"].Value) / DivFac;
                geom.center = new(x, y);
            } catch{}
            try{geom.radius = float.Parse(circle.Attributes["radius"].Value) / DivFac;} catch{}
            //TODO: IMPLEMENT REST OF PROPERTIES
            levelGeometries.Add(geom);
        }
        //TODO: IMPLEMENT RECTANGLES
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
        return scene;
    }
}
