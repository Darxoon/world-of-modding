using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class JSONLevel
{
    public bool allowskip = true;
    public bool autobounds = false;
    public int ballsrequired = 1;
    public Color cursorcolor;
    public bool letterboxed = false;
    public bool retrytime = false;
    public bool strandgeom = false;
    public Color textcolor = new Color(255, 255, 255);
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
    public Vector2 endpos = Vector2.zero;
    public float endzoom = 0;
    public poi[] poi;
}

[Serializable]
public class poi
{
    public float pause = 0;
    public Vector2 pos = Vector2.one;
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
public class signpost
{
    public float alpha;
    public Color colorize = new Color(255, 255, 255);
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
    public Vector2 pos = Vector2.zero;
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