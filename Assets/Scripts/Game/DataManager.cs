using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private string path;

    private void Awake()
    {
        path = Application.dataPath + "/Data/";
    }

    public void SaveGameData(GameData gameData)
    {
        string savedJson = JsonUtility.ToJson(gameData, true);
        string dataPath = Path.Combine(path, "GameData.json");
        File.WriteAllText(dataPath, savedJson);

        Debug.Log("Finish Saving GameData");
    }

    public void SaveRunData(RunData runData)
    {
        string savedJson = JsonUtility.ToJson(runData, true);
        string dataPath = Path.Combine(path, "RunData.json");
        File.WriteAllText(dataPath, savedJson);

        Debug.Log("Finish Saving RunData");
    }


    public GameData LoadGameData()
    {
        string dataPath = Path.Combine(path, "GameData.json");
        if (!File.Exists(dataPath))
        {
            Debug.Log("There doesn't exist Game Data");
            //return null;
        }
        
        //string loadedJson = File.ReadAllText(dataPath);
        //GameData gameData = JsonUtility.FromJson<GameData>(loadedJson);

        // ---------------------------------
        // TEST

        GameData gameData = new GameData();
        gameData.Initialize();

        // ---------------------------------

        return gameData;
    }

    public RunData LoadRunData(GameData gameData)
    {
        string dataPath = Path.Combine(path, "RunData.json");
        if (!File.Exists(dataPath))
        {
            Debug.Log("There doesn't exist Run Data");
            //return null;
        }

        //string loadedJson = File.ReadAllText(dataPath);
        //RunData runData = JsonUtility.FromJson<RunData>(loadedJson);

        // ---------------------------------
        // TEST

        RunData runData = new RunData();
        runData.Initialize(gameData);

        // ---------------------------------

        return runData;
    }
}
