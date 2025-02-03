using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match
{
    public int index { get; set; }
    public List<int> validIndices { get; set; }            // 블록이 있는 위치들
    public MatchType matchType { get; set; }
    public List<(BlockType, string)> blocks { get; set; }  // 각 칸에 해당하는 BlockType과 ID
    public bool isForceMatch { get; set; }                 // 강제 매치 여부
}