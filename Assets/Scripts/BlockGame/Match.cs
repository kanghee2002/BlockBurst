using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match : MonoBehaviour
{
    public int index { get; set; }
    public MatchType matchType { get; set; }
    public List<(BlockType, string)> blocks { get; set; }  // 각 칸에 해당하는 BlockType과 ID
}