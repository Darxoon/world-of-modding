using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class JSONLevel
{
    public Level level = new Level();

    public Scene scene = new Scene();

    public Resrc resrc = new Resrc();
}



#region level
public class Level
{
    public bool allowskip = true;
    public bool autobounds = false;
    public int ballsrequired = 1;
    public ColorData cursorcolor;
    public bool letterboxed = false;
    public bool retrytime = false;
    public bool strandgeom = false;
    public ColorData textcolor = new ColorData();
    public bool texteffects = true;
    public float timebugprobability = 0.5f;
    public bool visualdebug = false;
    public float zoomoutlimit = 0;
    public CameraData camera = null;
    public Music music = null;
    public Loopsound loopsound = null;
    public Signpost signpost = null;
    public Pipe pipe = null;
    public Ballinstance[] BallInstance = null;
    public LStrand[] Strand = null;
    public Levelexit levelexit = null;
    public Endoncollision endoncollision = null;
    public Fire[] fire = null;
    public Targetheight targetheight = null;
    public string[] tags = null;
}

public class CameraData
{
    public string aspect = "widescreen";
    public Position endpos = new Position();
    public float endzoom = 0;
    public Poi[] poi;
}

public class Position
{
    public Position(float posX = 0, float posY = 0)
    {
        x = posX;
        y = posY;
    }
    public float x;
    public float y;

    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }
    public Vector3 ToVector3(){
        return new Vector3(x, y, 0);
    }
}

public class Poi
{
    public float pause = 0;
    public Position pos = new Position();
    public float traveltime = 0;
    public float zoom = 0; 
}

public class Music
{
    public string id = "";
}

public class Loopsound
{
    public string id = "";
}

public class ColorData
{
    public ColorData(float colorR = 255, float colorG = 255, float colorB = 255)
    {
        r = colorR;
        g = colorG;
        b = colorB;
    }
    public float r = 255;
    public float g = 255;
    public float b = 255;

    public Color ToUnityColor()
    {
        return new Color(r, g, b);
    }
}

public class Signpost
{
    public float alpha;
    public ColorData colorize = new ColorData();
    public float depth;
    public string image = "";
    public string name = "";
    public string particles;
    public float scalex = 0;
    public float scaley = 0;
    public string text = "";
    public float x = 0;
    public float y = 0;
}

public class Pipe
{
    public float depth = 0;
    public string id = "0";
    public string type = "";
    public Vertex[] Vertex;
}

public class Vertex
{
    public float x = 0;
    public float y = 0;
}

public class Ballinstance
{
    public float angle = 0;
    public bool discovered = true;
    public string id = "";
    public string type = "";
    public Position pos = new Position();
}

public class LStrand
{
    public string gb1 = "";
    public string gb2 = "";
}

public class Levelexit
{
    public string filter = "";
    public string id = "";
    public Position pos = new Position();
    public float radius = 0;
}

public class Endoncollision
{
    public float delay = 0;
    public string id1 = "";
    public string id2 = "";
}

public class Fire
{
    public float depth = 0;
    public float particles = 0;
    public float radius = 0;
    public float x = 0;
    public float y = 0;
}

public class Targetheight
{
    public float y = 0;
}

#endregion

#region scene
public class Scene
{
    public ColorData backgroundcolor = new ColorData();
    public float minX;
    public float minY;
    public float maxX;
    public float maxY;
    public ForceFields ForceFields = new ForceFields();
    public Particle[] particles = null;
    public Scenelayer[] scenelayers = null;
    public Buttongroup[] buttongroups = null;
    public Button[] buttons = null;
    public Label[] labels = null;
    public Compositegeom[] compositegeoms = null;
    public LevelGeometry[] geometries = null;
    public Line[] lines = null;
    public Motor[] motors = null;
    public Hinge[] hinges = null;
    public Scene() {
        particles = new Particle[0];
        scenelayers = new Scenelayer[0];
        buttongroups = new Buttongroup[0];
        labels = new Label[0];
        compositegeoms = new Compositegeom[0];
        geometries = new LevelGeometry[0];
        lines = new Line[0];
        motors = new Motor[0];
        hinges = new Hinge[0];
    }
}

public class ForceFields
{
    public LinearForceField[] linearforcefields = null;
    public RadialForceField[] radialforcefields = null;
}

public class LinearForceField
{
    public string id = "";
    public string type = "";
    public Position force = new Position();
    public float dampeningfactor = 0;
    public bool antigrav = true;
    public bool geomonly = false;
    public bool? water = null;
    public Position center = null;
    public float? size = null;
}

public class RadialForceField
{
    public string id = "";
    public string type = "";
    public Position force = new Position();
    public float dampeningfactor = 0;
    public bool antigrav = true;
    public bool geomonly = false;
    public float forceatcenter;
    public float forceatedge;
    public Position center = new Position(0, 0);
    public float radius;
    public bool enabled = true;
}

public class Particle
{
    #region  Level type particle data
    public string effect;
    public enum Type : int
    {
        Ambient,
        PointSource
    }
    public Type type;
    public float depth;
    public Position pos;
    public float pretick;
    #endregion
    #region  Gooball type particle data
    public string id;
    public string[] states;
    public bool overball = false;
    #endregion
}

public class Scenelayer
{
    public string name = "";
    public float depth = 0;
    public Position pos = new Position(0,0);
    public float scaleX = 1;
    public float scaleY = 1;
    public float rotation = 0;
    public float alpha = 1;
    public ColorData colorize = new ColorData(255, 255, 255);
    public string image = "";
    public bool tileX = false;
    public bool tileY = false;
}

public class Buttongroup
{
    public string id = "";
    public Position osx = new Position(); //what is osx i dont know
    public Button[] buttons;
}

public class Button
{
    public string id = "";
    public float depth = 0;
    public Position pos = new Position();
    public float scaleX = 0;
    public float scaleY = 0;
    public float rotation = 0;
    public float alpha = 1;
    public ColorData colorize = new ColorData();
    public string up = "";
    public string over = "";
    public string onclick = "";
    public string onmouseenter = "";
    public string onmouseexit = "";
    public string disabled = "";
}

public class Label
{
    public Position center = new Position();
    public float rotation = 0;
    public float scale = 0;
    public float depth = 0;
    public bool overlay = false;
    public string text = "";
    public string font = "";
}

public class Compositegeom
{
    public string name = "";
    public Position position = new Position();
    public bool dynamic = false;
    public LevelGeometry[] geometries;
    public string image;
    public Position imagepos;
    public float imagerot;
    public Position imagescale;
    public float rotspeed;
}

public class LevelGeometry
{
    public string id = "";
    public string tag = "";
    public string material = "";
    public Position center = new Position();
    public float radius;
    public float rotation;
    public bool dynamic = false;
    public float mass;
    public enum Type
    {
        Circle,
        Rectangle
    }
    public Type type;
    public Position size;
    public float rotspeed;
    public Scenelayer image;
}

public class Line
{
    public string id = "";
    public string tag = "";
    public string material = "";
    public Position anchor = new Position();
    public Position normal = new Position(1, 1);
}

public class Motor
{
    public string body = "";
    public float speed = 0;
    public float maxforce = 0;
}

public class Hinge
{
    public string body1 = "";
    public string body2;
    public Position anchor = new Position();
}
#endregion

#region resources
public class Resrc
{
    public string id = "";
    public Dictionary<string, string> resources = new Dictionary<string, string>();
}

#endregion