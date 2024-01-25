using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class FunThings : MonoBehaviour
{
    // Start is called before the first frame update
    List<string> filter = new List<string>{ "Distant", "BlockHead", "RectHead", "IconWindowSquare", "IconWindowRect" };
    void Start()
    {
        int count = 0;
        foreach(var dir in Directory.GetDirectories(StaticData.levelFolder)){
            string[] parts = dir.Split('/');
            string levelName = parts[parts.Length-1];
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            JSONLevel level = null;
            if(File.Exists(StaticData.levelFolder + $"{levelName}/{levelName}.json"))
                level = JsonConvert.DeserializeObject<JSONLevel>(File.ReadAllText(StaticData.levelFolder + $"{levelName}/{levelName}.json"), settings);
            else if(
                File.Exists(StaticData.levelFolder + $"{levelName}/{levelName}.level") &&
                File.Exists(StaticData.levelFolder + $"{levelName}/{levelName}.scene") &&
                File.Exists(StaticData.levelFolder + $"{levelName}/{levelName}.resrc")
            )
                level = ResourceConverter.ConvertXMLLevelToJSON(
                    StaticData.levelFolder + $"{levelName}/{levelName}.scene",
                    StaticData.levelFolder + $"{levelName}/{levelName}.resrc",
                    StaticData.levelFolder + $"{levelName}/{levelName}.level"
                );
            else if(
                File.Exists(StaticData.levelFolder + $"{levelName}/{levelName}.level.bin") &&
                File.Exists(StaticData.levelFolder + $"{levelName}/{levelName}.scene.bin") &&
                File.Exists(StaticData.levelFolder + $"{levelName}/{levelName}.resrc.bin")
            )
                    level = ResourceConverter.ConvertEncryptedXMLLevelToJSON(
                    StaticData.levelFolder + $"{levelName}/{levelName}.scene.bin",
                    StaticData.levelFolder + $"{levelName}/{levelName}.resrc.bin",
                    StaticData.levelFolder + $"{levelName}/{levelName}.level.bin"
                );
            else
            {
                Debug.LogError($"Level {levelName} does not exist.");
                return;
            }
            foreach(var inst in level.level.BallInstance){
                if(!filter.Contains(inst.type))
                    count++;
            }
        }
        Debug.Log(count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
