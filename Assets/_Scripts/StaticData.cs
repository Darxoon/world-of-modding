using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.Networking;

public static class StaticData
{
    
    public static GameObject balls = null;
    public static GameObject sceneLayers = null;
    public static GameObject geometry = null;
    public static GameObject strands = null;
    public static GameObject forcefields = null;
    public static GameObject ui = null;
    public static GameObject pipe = null;

    public static Dictionary<GameObject, Strand> existingStrands = new Dictionary<GameObject, Strand>();
    public static Dictionary<GameObject, Gooball> existingGooballs = new Dictionary<GameObject, Gooball>();
        
    public static GameManager gameManager = null;
    public static string resFolder = Application.dataPath + "/res/";
    public static string levelFolder = resFolder + "levels/";
    public static string ballsFolder = resFolder + "balls/";
    public static string imagesFolder = resFolder + "images/";
    public static JSONLevelLoader levelLoader;



    public static void PopulateAllResources()
    {
        foreach(var Path in GameManager.ResourcePaths)
        {
            if (Path.Value.EndsWith(".ogg") && !GameManager.audioFiles.ContainsKey(Path.Key))
            {
                string fullPath = "file:///" + resFolder + Path.Value;
                AudioClip audio = null;
                getAudioClip(fullPath, audio);
                GameManager.audioFiles.Add(Path.Key, audio);
            }
            else if(!GameManager.imageFiles.ContainsKey(Path.Key))
            {
                string fullpath = resFolder + Path.Value;
                if(File.Exists(fullpath.Substring(0, fullpath.Length-4) + "@2x.png")){
                    fullpath = fullpath.Substring(0, fullpath.Length-4) + "@2x.png";
                }
                if(!File.Exists(fullpath)){
                    Debug.LogWarning($"Texture at {fullpath} for ID {Path.Key} was not found. Skipping loading...");
                    continue;
                }
                byte[] imageData = File.ReadAllBytes(fullpath);
                Texture2D tex = new Texture2D(2,2);
                tex.LoadImage(imageData);
                Vector2 texsize = new Vector2(tex.width, tex.height);
                Sprite sprite = Sprite.Create(tex, new Rect(Vector2.zero, texsize), new Vector2(0.5f, 0.5f));
                if(!fullpath.EndsWith("@2x.png"))
                    GameManager.imageFiles.Add(Path.Key, new SpriteData() { sprite = sprite });
                else
                    GameManager.imageFiles.Add(Path.Key, new SpriteData() { sprite2x = sprite });
            }
        }
    }

    static IEnumerator getAudioClip(string url, AudioClip clip)
    {
        using(UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.OGGVORBIS))
        {
            yield return www.Send();
            if (www.isNetworkError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                clip = DownloadHandlerAudioClip.GetContent(www);
            }
        }
    }

    public static JSONGooball RetrieveGooballDataFromType(string type)
    {
        if(Directory.Exists(ballsFolder + type))
        {
            if(File.Exists(ballsFolder + type + "/" + type + ".json")){
                string json = File.ReadAllText(ballsFolder + type + "/" + type + ".json");
                return JsonConvert.DeserializeObject<JSONGooball>(json);
            } else if (File.Exists(ballsFolder + type + "/balls.xml")){
                //Legacy data
                return ResourceConverter.ConvertXMLBallToJSON(ballsFolder + type + "/balls.xml", 
                ballsFolder + type + "/resources.xml");
            } else if (File.Exists(ballsFolder + type + "/balls.xml.bin")){
                //Encrypted legacy data
                return ResourceConverter.ConvertEncryptedXMLBallToJSON(ballsFolder + type + "/balls.xml.bin",
                ballsFolder + type + "/resources.xml.bin");
            }
        }
        return null;
    }

}
    