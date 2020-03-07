using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class JSONLevel
{
    public level level = new level();

    public scene scene = new scene();
}



#region level
public class level
{
    public bool allowskip = true;
    public bool autobounds = false;
    public int ballsrequired = 1;
    public color cursorcolor;
    public bool letterboxed = false;
    public bool retrytime = false;
    public bool strandgeom = false;
    public color textcolor = new color();
    public bool texteffects = true;
    public float timebugprobability = 0.5f;
    public bool visualdebug = false;
    public float zoomoutlimit = 0;
    public camera camera = null;
    public music music = null;
    public loopsound loopsound = null;
    public signpost signpost = null;
    public pipe pipe = null;
    public ballinstance[] BallInstance = null;
    public strand[] Strand = null;
    public levelexit levelexit = null;
    public endoncollision endoncollision = null;
    public fire[] fire = null;
    public targetheight targetheight = null;
}

[Serializable]
public class camera
{
    public string aspect = "widescreen";
    public position endpos = new position();
    public float endzoom = 0;
    public poi[] poi;
}

[Serializable]
public class position
{
    public position(float posX = 0, float posY = 0)
    {
        x = posX;
        y = posY;
    }
    public float x;
    public float y;
}

[Serializable]
public class poi
{
    public float pause = 0;
    public position pos = new position();
    public float traveltime = 0;
    public float zoom = 0; 
}

[Serializable]
public class music
{
    public string id = "";
}

[Serializable]
public class loopsound
{
    public string id = "";
}

[Serializable]
public class color
{
    public color(float colorR = 255, float colorG = 255, float colorB = 255)
    {
        r = colorR;
        g = colorG;
        b = colorB;
    }
    public float r = 255;
    public float g = 255;
    public float b = 255;
}

[Serializable]
public class signpost
{
    public float alpha;
    public color colorize = new color();
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

[Serializable]
public class pipe
{
    public float depth = 0;
    public string id = "0";
    public string type = "";
    public vertex[] Vertex;
}

[Serializable]
public class vertex
{
    public float x = 0;
    public float y = 0;
}

[Serializable]
public class ballinstance
{
    public float angle = 0;
    public bool discovered = true;
    public string id = "";
    public string type = "";
    public float x = 0;
    public float y = 0;
}

[Serializable]
public class strand
{
    public string gb1 = "";
    public string gb2 = "";
}

[Serializable]
public class levelexit
{
    public string filter = "";
    public string id = "";
    public position pos = new position();
    public float radius = 0;
}

[Serializable]
public class endoncollision
{
    public float delay = 0;
    public string id1 = "";
    public string id2 = "";
}

[Serializable]
public class fire
{
    public float depth = 0;
    public float particles = 0;
    public float radius = 0;
    public float x = 0;
    public float y = 0;
}

[Serializable]
public class targetheight
{
    public float y = 0;
}

#endregion

#region scene
public class scene
{
    public color backgroundcolor = new color();
    public float minX;
    public float minY;
    public float maxX;
    public float maxY;
    public ForceFields ForceFields = new ForceFields();
    public particle[] particles;
    public scenelayer[] scenelayers;
}

public class ForceFields
{
    public linearforcefield[] linearforcefields = null;
    public radialforcefield[] radialforcefields = null;
}
public class linearforcefield
{
    public string type = "";
    public position force = new position();
    public float dampeningfactor = 0;
    public bool antigrav = true;
    public bool geomonly = false;
    public bool? water = null;
    public position center = null;
    public float? size = null;
}

public class radialforcefield
{
    public string type = "";
    public position force = new position();
    public float dampeningfactor = 0;
    public bool antigrav = true;
    //public bool geomonly = false;
    public float forceatcenter;
    public float forceatedge;
    public position center = new position(0, 0);
}

public class particle
{
    public enum type : int
    {
        Ambient,
        PointSource
    }
    public float depth;
    public position pos;
    public float pretick;
}

public class scenelayer
{
    public string name = "";
    public float depth = 0;
    public position pos = new position(0,0);
    public float scaleX = 1;
    public float scaleY = 1;
    public float rotation = 0;
    public float alpha = 1;
    public color colorize = new color(255, 255, 255);
    public string image = "";
    public bool tileX = false;
    public bool tileY = false;
}
#endregion