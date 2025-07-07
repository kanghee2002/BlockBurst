using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "BlockBurst/Level")]
public class LevelData : ScriptableObject
{
    public int level;
    public List<int> additionalStageScores;          // 추가적인 스테이지 기본 점수
    public int additionalStageReward;                // 추가적인 스테이지 보상
    public int additionalBaseRerollCount;            // 추가적인 기본 리롤 횟수
    public int additionalBlockScore;                 // 추가적인 블록 점수
    public int additionalMaxItemCount;               // 추가적인 최대 아이템 수
    [TextArea] public string description;                       // 레벨 설명
}
