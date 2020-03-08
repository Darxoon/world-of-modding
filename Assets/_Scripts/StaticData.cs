using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
public static class StaticData
{
    
    public static GameObject balls = null;
    public static GameObject sceneLayers = null;
    public static GameObject geometry = null;
    public static GameObject strands = null;
    public static GameManager gameManager = null;
    public static string resFolder = Application.dataPath + "/res/";
    public static string levelFolder = resFolder + "levels/";
    public static string ballsFolder = resFolder + "balls/";
    public static string imagesFolder = resFolder + "images/";
    public static JSONLevelLoader levelLoader;

    public static Dictionary<string, string> ResourcePaths = new Dictionary<string, string>();

    public static Dictionary<string, AudioClip> audioFiles = new Dictionary<string, AudioClip>();
    public static Dictionary<string, Sprite> imageFiles = new Dictionary<string, Sprite>();

    public static void PopulateAllResources()
    {
        foreach(var Path in ResourcePaths)
        {
            if (Path.Value.EndsWith(".ogg"))
            {

            }
        }
    }
    public static Sprite RetrieveImage(string textureID)
    {
        string RelativePath;
        ResourcePaths.TryGetValue(textureID, out RelativePath);
        if (RelativePath != null)
        {
            string fullPath = resFolder + RelativePath;
            byte[] imageData = File.ReadAllBytes(fullPath);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imageData);
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }
        else
            return null;
    }

    public static JSONGooball RetrieveGooballDataFromID(string id)
    {
        if(Directory.Exists(ballsFolder + id))
        {
            string json = File.ReadAllText(ballsFolder + id + "/" + id + ".json");
            return JsonConvert.DeserializeObject<JSONGooball>(json);
        }
        return null;
    }

}
    