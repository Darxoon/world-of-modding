using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONGooball
{
    public Ball ball = new Ball();
    public Part[] parts;
    public Marker marker;
    public Shadow shadow;
    public Particles[] particles;
    public StrandJSON strand;
    public DetachStrand detachstrand;
    public Splat splat;
    public Sound[] sounds;
    public SinVariance[] anims;
}

public enum States
{
    attached,
    climbing,
    detaching,
    dragging,
    falling,
    pipe,
    sleeping,
    standing,
    stuck,
    stuck_attached,
    stuck_detaching,
    tank,
    walking,
    all
}

public enum Events
{
    Attach,
    AttachCloser,
    Bounce,
    CollideDiff,
    CollideGeom,
    CollideSame,
    Death,
    Detached,
    Detaching,
    Detonate,
    Drop,
    Exit,
    Ignite,
    Marker,
    Pickup,
    Snap,
    Suction,
    Throw,
    All
}

#region Ball tag
public class Ball
{
    #region Core attributes
    public string name = "";
    public enum Shape
    {
        Rectangle,
        Circle
    }
    public Shape shape = Shape.Circle;
    public float width;
    public float height;
    public float diameter;
    public float sizeVariation;
    public float mass;
    public int strands;
    public string material = ""; //curently unknown use
    public float towerMass;
    public float dragMass;
    #endregion

    #region Behaviour attributes
    public bool climber;
    public float antigrav;
    public bool isAntiGravUnattached;
    public bool dynamic;
    public bool staticWhenSleeping;
    public float wakeDist;
    public Position jump;
    public bool jumpOnWakeup;
    public bool autoAttach;
    public bool autoDisable;
    public float decay;
    #endregion

    #region Movement attributes
    public float walkSpeed = 0;
    public float climbspeed = 0;
    public Position speedDifference = new Position(1, 1);
    public float walkForce = 500;
    public float thrust = 0;
    #endregion

    #region Player interaction
    public bool draggable;
    public bool detachable;
    public bool hingeDrag;
    public Position fling;
    #endregion

    #region Level interaction
    public bool invulnerable;
    public bool suckable;
    public bool autobounds;
    public bool autoboundsunattached;
    public bool sticky;
    public bool stickyunattached;
    public bool stickyattached;
    #endregion

    #region Other ball interaction
    public bool grumpy;
    public bool collidewithattached;
    public bool collideattached;
    public bool stuckattachment;
    public bool fallingattachment;
    public bool stacking;
    public float maxattachspeed;
    #endregion

    #region Cosmetic attributes
    public ColorData blinkColor;
    public bool hideEyes;
    public StateScale[] statescales;
    public Attenuation attenuationSelect;
    public Attenuation attenuationDeselect;
    public Attenuation attenuationDrag;
    public Attenuation attenuationDrop;
    public bool isBehindStrands;
    public bool distantSounds;
    #endregion

    #region Burn attributes
    public float burntime;
    public float detonateforce;
    public float detonateradius;
    public string explosionparticles;
    #endregion

    #region Popping attributes
    public PopContent[] contains;
    public float popduration;
    public string popparticles;
    public string popsound;
    public Position popdelay;
    #endregion

    public string spawn; //dispenser attribute
}

public class PopContent
{
    public int number;
    public string ballType;
}
public class Attenuation
{
    public float time;
    public float[] scaleFactor;
}

public class StateScale
{
    public States statename;
    public float stateFactor;
}
#endregion

#region Part tag
public class Part
{
    public string name = "";
    public string[] image = { };
    public Position x = new Position(0,0);
    public Position y = new Position(0, 0);
    public int layer;
    public States[] state = { States.all };
    public float scale;
    public float rotate;
    public Stretch stretch;
    public Eye eye;
    public Position xRange;
    public Position yRange;
}

public class Eye
{
    public string pupil = "";
    public int pupilInset;
}
public class Stretch
{
    public float speed;
    public float directionScale;
    public float acrossScale;
}

#endregion
public class Marker
{
    public string drag = "";
    public string detach = "";
    public float rotspeed = 0;
} //tag

public class Shadow
{
    public string image = "";
    public bool additive;
} //tag

public class Particles //tag
{
    public string effect = "";
    public States[] state = { };
    public bool overBall;
}

public class StrandJSON //tag
{
    public enum Type
    {
        Spring,
        Rope,
        Rigid
    }
    public Type type = Type.Rigid;
    public string image = "";
    public string inactiveImage = "";
    public float minLen;
    public float maxLen2;
    public float maxLen1;
    public float shrinkLen;
    public float thickness;
    public Position springConst = new Position(0, 20);
    public bool walkable;
    public float dampFac;
    public float maxForce;
    public float burnSpeed;
    public float igniteDelay;
    public string burntImage;
    public string fireParticles;
}

public class DetachStrand //tag
{
    public string image;
    public float maxLen;
}

public class Splat //tag
{
    public string image;
}

public class Sound
{
    public Events soundEvent;
    public string[] sounds;
} //tag

#region Animation
public class SinAnim
{
    public string part;
    public States[] states = { States.all };
    public enum Type
    {
        scale,
        translate
    }
    public Type type = Type.scale;
    public enum Axis
    {
        x,
        y
    }
    public Axis axis = Axis.x;
    public float freq;
    public float shift;
}

public class SinVariance
{
    public float amp = 1;
    public float freq = 1;
    public float shift = 0;
    
    public SinAnim[] anims = { };

}
#endregion