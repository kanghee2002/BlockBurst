using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RerollInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject rerollCountText;
    public void UpdateRerollCount(int countToSet)
    {
        Debug.Log("Reroll Count has been updated.");
    }
}
