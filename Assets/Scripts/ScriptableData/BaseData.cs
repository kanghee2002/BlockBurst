using System;
using UnityEngine;

public abstract class BaseData : ScriptableObject
{
    public string id;
    public string resourceKey;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(id))
        {
            id = Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
}

