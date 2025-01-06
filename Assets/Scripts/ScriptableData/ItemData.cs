using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "BlockBurst/Item")]
public class ItemData : ScriptableObject
{
    public string id;                  // ������ ID
    public List<EffectData> effects;   // ������ ȿ��
    public int cost;                   // ���� ���
    public string targetBlockId;       // ��� ��� ID (�ʿ��� ���)
}

public enum MatchType
{
    ROW,    // �� ��ġ
    COLUMN, // �� ��ġ
    SQUARE  // ���簢�� ��ġ
}