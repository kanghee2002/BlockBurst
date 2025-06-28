using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using System.Linq;

public class UnlockInfo
{
    public UnlockTarget targetType;
    public string targetName;
    public string description;
    public UnlockTrigger trigger;
    public Action<int> condition;
    public int requirement;

    public void CheckCondition(int value)
    {
        Debug.Log("Checking Unlock: " + targetName);
        if (value >= requirement)
        {
            UnlockManager.instance.Unlock(this, targetName);
        }
    }
}

public class UnlockInfoList
{
    public List<UnlockInfo> list { get; private set; }

    public void Initialize()
    {
        list = new List<UnlockInfo>();

        UnlockInfo wheelInfo = new UnlockInfo();
        wheelInfo.targetType = UnlockTarget.Item;
        wheelInfo.targetName = "Wheel";
        wheelInfo.trigger = UnlockTrigger.RerollCount;
        wheelInfo.condition = wheelInfo.CheckCondition;
        wheelInfo.requirement = 3;
        wheelInfo.description = "블록 리롤" + wheelInfo.requirement + "번 하기";

        list.Add(wheelInfo);

        UnlockInfo puzzlePieceI = new UnlockInfo();
        puzzlePieceI.targetType = UnlockTarget.Item;
        puzzlePieceI.targetName = "PuzzlePieceI";
        puzzlePieceI.trigger = UnlockTrigger.IplaceCount;
        puzzlePieceI.condition = puzzlePieceI.CheckCondition;
        puzzlePieceI.requirement = 1;
        puzzlePieceI.description = "I를" + puzzlePieceI.requirement + "번 배치하기";

        list.Add(puzzlePieceI);

    }
}
