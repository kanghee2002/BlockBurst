using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match : MonoBehaviour
{
    public int index { get; set; }
    public MatchType matchType { get; set; }
    public List<BlockType> blockTypes { get; set; }
}