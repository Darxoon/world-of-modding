using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorScript : MonoBehaviour
{
    private JSONLevel CL; //Current level JSON
    private string CLName; //Current level name
    private string CLDescription; //Current level description

    private Transform NLD; //New level dialog
    private Transform NL; //New level
    private Transform Cheks;

    private TMP_InputField LevelName;
    private TMP_InputField LevelDescription;

    void Start()
    {
        NLD = this.transform.Find("New level dialog");
        print(NLD);

        NL = NLD.transform.Find("New level");
        print(NL);

        LevelName = NL.transform.Find("Level name").GetComponent<TMP_InputField>();
        print(LevelName);

        LevelDescription = NL.transform.Find("Description").GetComponent<TMP_InputField>();
        print(LevelDescription);

        Cheks = NL.transform.Find("Cheks");
        print(Cheks);

        NL.transform.Find("Warning").gameObject.SetActive(false);
    }

    void Update()
    {
        
    }

    public void openWin() // open a new level window and restart
    {
        NLD.gameObject.SetActive(true);
        NL.transform.Find("Warning").gameObject.SetActive(false);

        LevelName.text = null;
        LevelDescription.text = null;
    }

    public void closeWin()
    {
        NLD.gameObject.SetActive(false);
    }

    public void NewLevel()
    {
        CLName = LevelName.text;
        print(CLName);

        CLDescription = LevelDescription.text;
        print(CLDescription);

        if (CLName != string.Empty) {
            CL = new JSONLevel(); //init new level

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            CL.level.allowskip = Cheks.transform.Find("Allow skip").GetComponent<Toggle>().isOn;
            CL.level.autobounds = Cheks.transform.Find("Auto bounds").GetComponent<Toggle>().isOn;
            CL.level.retrytime = Cheks.transform.Find("Retry time").GetComponent<Toggle>().isOn;
            CL.level.letterboxed = Cheks.transform.Find("Letter boxed").GetComponent<Toggle>().isOn;
            CL.level.texteffects = Cheks.transform.Find("Text effects").GetComponent<Toggle>().isOn;
            CL.level.visualdebug = Cheks.transform.Find("Debug").GetComponent<Toggle>().isOn;

            if (!Directory.Exists(StaticData.levelFolder + CLName + "/"))
                Directory.CreateDirectory(StaticData.levelFolder + CLName + "/");

            using (StreamWriter sw = File.CreateText(StaticData.levelFolder + CLName + "/" + CLName + ".json"))
                sw.Write(JsonConvert.SerializeObject(CL, Formatting.Indented, settings));

            closeWin();
        }
        else
        {
            NL.transform.Find("Warning").gameObject.SetActive(true);
        }
    }
}
