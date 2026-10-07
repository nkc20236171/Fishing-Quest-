using System.Collections.Generic;
using UnityEngine;

public static class BattleSceneData
{
    // 戦闘後に戻るシーン
    public static string ReturnSceneName = "Map";

    // Playerの戻り位置
    public static Vector3 ReturnPlayerPosition;

    public static bool HasReturnPlayerPosition = false;

    // カメラの戻り位置
    public static Vector3 ReturnCameraPosition;

    public static bool HasReturnCameraPosition = false;


    // -------------------------
    // 敵情報
    // -------------------------

    public static string EnemyName = "スライム";

    public static int EnemyMaxHp = 30;

    public static int EnemyAttack = 5;

    public static string CurrentEnemyId = "";


    // BattleSceneに表示する敵Prefab
    public static GameObject CurrentEnemyBattlePrefab;


    // -------------------------
    // 倒した敵
    // -------------------------

    public static HashSet<string> DefeatedEnemyIds =
        new HashSet<string>();


    // -------------------------
    // 逃げた敵
    // -------------------------

    public static HashSet<string> DespawnedEnemyIds =
        new HashSet<string>();


    // -------------------------
    // ドロップ
    // -------------------------

    public static List<MonsterDropEntry> CurrentDrops =
        new List<MonsterDropEntry>();


    public static void SetDrops(
        MonsterDropEntry[] drops)
    {
        CurrentDrops.Clear();

        if (drops == null)
        {
            return;
        }

        foreach (MonsterDropEntry drop in drops)
        {
            MonsterDropEntry copy =
                new MonsterDropEntry();

            copy.itemType =
                drop.itemType;

            copy.dropRate =
                drop.dropRate;

            copy.minAmount =
                drop.minAmount;

            copy.maxAmount =
                drop.maxAmount;

            CurrentDrops.Add(copy);
        }
    }
}