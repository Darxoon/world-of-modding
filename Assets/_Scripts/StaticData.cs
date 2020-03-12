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
                byte[] imageData = File.ReadAllBytes(fullpath);
                Texture2D tex = new Texture2D(2,2);
                tex.LoadImage(imageData);
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                GameManager.imageFiles.Add(Path.Key, sprite);
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
            string json = File.ReadAllText(ballsFolder + type + "/" + type + ".json");
            return JsonConvert.DeserializeObject<JSONGooball>(json);
        }
        return null;
    }

}
    