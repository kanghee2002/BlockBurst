using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using System.Linq;

[CreateAssetMenu(fileName = "UnlockInfo", menuName = "BlockBurst/UnlockInfo")]
public class UnlockInfo :ScriptableObject
{
    public UnlockTarget targetType;
    public string targetName;
    public string description;
    public UnlockTrigger trigger;
    public Action<int> condition;
    public int requirement;

    public void CheckCondition(int value)
    {
        if (value >= requirement)
        {
            UnlockManager.instance.Unlock(this, targetName);
        }
    }

    public void Initialize()
    {
        condition = CheckCondition;
    }

    public string GetDescription()
    {
        string result = description.Replace("Requirement", requirement.ToString());

        result = UIUtils.SetBlockNameToIcon(result);

        return result; 
    }
}
