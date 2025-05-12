using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    string savedRunData;
    string savedGameData;

    public void SaveRunData(RunData runData, int chapterIndex, int stageIndex)
    {
        // Add chapterIndex, stageIndex to RunData
        // Add history to RunData
    }

    public void SaveGameData(GameData gameData)
    {

    }

    public RunData LoadRunData()
    {
        RunData loadedRunData = JsonUtility.FromJson<RunData>(savedRunData);


        return loadedRunData;
    }

    public GameData LoadGameData()
    {
        GameData loadedGameData = JsonUtility.FromJson<GameData>(savedGameData);


        return loadedGameData;
    }

}
