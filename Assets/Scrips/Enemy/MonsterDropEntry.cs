using UnityEngine;

[System.Serializable]
public class MonsterDropEntry
{
    // ドロップする素材
    public DropItemType itemType;

    // ドロップ確率 0～100
    [Range(0f, 100f)]
    public float dropRate = 50f;

    // 最低個数
    [Min(1)]
    public int minAmount = 1;

    // 最大個数
    [Min(1)]
    public int maxAmount = 1;
}