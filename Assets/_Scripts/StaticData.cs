using System.Collections.Generic;
using UnityEngine;

public static class StaticData
{
    
    public static GameObject balls = null;
    public static GameObject sceneLayers = null;
    public static GameObject geometry = null;
    public static GameObject strands = null;

    public static Dictionary<GameObject, Strand> existingStrands = new Dictionary<GameObject, Strand>();
    public static Dictionary<GameObject, Gooball> existingGooballs = new Dictionary<GameObject, Gooball>();
        
    public static GameManager gameManager = null;

}
