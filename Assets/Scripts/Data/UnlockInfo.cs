using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class UnlockInfo<T>
{
    public UnlockTarget targetType;
    public string targetName;
    public string description;
    public UnlockTrigger trigger;
    public Action<T> condition;
    public T requirement;
    public Func<T, T, bool> comparer;

    public void CheckCondition(T value)
    {
        Debug.Log("Checking...");
        if (comparer(value, requirement))
        {
            UnlockManager.instance.TryUnlock(targetType, targetName);

            //Unbind();
        }
    }
}

public class UnlockInfoList
{
    public List<UnlockInfo<int>> intList { get; private set; }
    public List<UnlockInfo<bool>> boolList { get; private set; }

    public void Initialize()
    {
        intList = new List<UnlockInfo<int>>(); 

        UnlockInfo<int> wheelInfo = new UnlockInfo<int>();
        wheelInfo.targetType = UnlockTarget.Item;
        wheelInfo.targetName = "Wheel";
        wheelInfo.trigger = UnlockTrigger.RerollCount;
        wheelInfo.condition = wheelInfo.CheckCondition;
        wheelInfo.requirement = 3;
        wheelInfo.description = "블록 리롤" + wheelInfo.requirement + "번 하기";
        wheelInfo.comparer = (value, req) => value >= req;

        UnlockManager.instance.onRerollCountUpdate += wheelInfo.CheckCondition;

        intList.Add(wheelInfo);
    }
}
