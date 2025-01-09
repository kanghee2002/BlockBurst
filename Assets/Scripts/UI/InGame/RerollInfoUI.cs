using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RerollInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject rerollCountText;
    public void UpdateReroll(int _reroll)
    {
        Debug.Log("Reroll Count has been updated.");
    }
}
