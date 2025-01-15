using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSetUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(List<ItemData> items)
    {
        string text = "";
        foreach (ItemData itemData in items)
        {
            text += itemData.type.ToString() + "\n";
        }
        Debug.Log(text);
    }
}
