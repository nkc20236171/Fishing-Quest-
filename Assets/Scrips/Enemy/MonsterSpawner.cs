using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    // このスポナー固有の名前
    // エリアごとに必ず別名にする
    [SerializeField] private string areaId = "Field_A";

    // スポーンさせるモンスターPrefab
    [SerializeField] private GameObject[] monsterPrefabs;

    // スポーン候補地点
    [SerializeField] private Transform[] spawnPoints;

    // 最低配置数
    [SerializeField] private int minMonsterCount = 4;

    // 最大配置数
    [SerializeField] private int maxMonsterCount = 6;

    // 倒した後に再スポーンするまでの時間
    [SerializeField] private float respawnDelay = 5f;

    // エリアごとの状態を保持
    private static Dictionary<string, AreaState> areaStates =
        new Dictionary<string, AreaState>();

    private AreaState currentState;


    // 1体分の保存情報
    private class SpawnRecord
    {
        public string enemyId;
        public int prefabIndex;
        public int spawnPointIndex;
    }


    // エリア全体の保存情報
    private class AreaState
    {
        public int targetMonsterCount;

        public int nextEnemyNumber = 1;

        public List<SpawnRecord> monsters =
            new List<SpawnRecord>();
    }


    private void Start()
    {
        if (monsterPrefabs == null ||
            monsterPrefabs.Length == 0)
        {
            Debug.LogError(
                areaId +
                " にMonster Prefabが設定されていません"
            );

            return;
        }

        if (spawnPoints == null ||
            spawnPoints.Length == 0)
        {
            Debug.LogError(
                areaId +
                " にSpawn Pointが設定されていません"
            );

            return;
        }

        SetupAreaState();

        // 戦闘で消えた敵を確認
        int removedCount =
            RemoveDefeatedMonsters();

        // 生きている敵を再配置
        SpawnSavedMonsters();

        // 初回なら必要数まで生成
        int missingCount =
            currentState.targetMonsterCount -
            currentState.monsters.Count;

        if (missingCount > 0)
        {
            // 戦闘から戻ってきた場合は
            // 少し待ってから補充
            if (removedCount > 0)
            {
                StartCoroutine(
                    RespawnMonsters(missingCount)
                );
            }
            else
            {
                // 初回は即生成
                for (int i = 0;
                     i < missingCount;
                     i++)
                {
                    CreateNewMonster();
                }
            }
        }
    }


    // -------------------------
    // エリア状態作成
    // -------------------------

    private void SetupAreaState()
    {
        if (areaStates.ContainsKey(areaId))
        {
            currentState =
                areaStates[areaId];

            return;
        }

        currentState =
            new AreaState();

        int maxCount =
            Mathf.Min(
                maxMonsterCount,
                spawnPoints.Length
            );

        int minCount =
            Mathf.Min(
                minMonsterCount,
                maxCount
            );

        currentState.targetMonsterCount =
            Random.Range(
                minCount,
                maxCount + 1
            );

        areaStates.Add(
            areaId,
            currentState
        );
    }


    // -------------------------
    // 倒した / 逃げた敵を削除
    // -------------------------

    private int RemoveDefeatedMonsters()
    {
        int removedCount = 0;

        for (int i =
             currentState.monsters.Count - 1;
             i >= 0;
             i--)
        {
            SpawnRecord record =
                currentState.monsters[i];

            bool defeated =
                BattleSceneData
                    .DefeatedEnemyIds
                    .Contains(record.enemyId);

            bool despawned =
                BattleSceneData
                    .DespawnedEnemyIds
                    .Contains(record.enemyId);

            if (!defeated &&
                !despawned)
            {
                continue;
            }

            currentState.monsters
                .RemoveAt(i);

            removedCount++;

            // 処理済みなのでリストから消す
            BattleSceneData
                .DefeatedEnemyIds
                .Remove(record.enemyId);

            BattleSceneData
                .DespawnedEnemyIds
                .Remove(record.enemyId);
        }

        return removedCount;
    }


    // -------------------------
    // 保存されている敵を再配置
    // -------------------------

    private void SpawnSavedMonsters()
    {
        foreach (
            SpawnRecord record
            in currentState.monsters)
        {
            SpawnMonster(record);
        }
    }


    // -------------------------
    // 新しい敵を作る
    // -------------------------

    private void CreateNewMonster()
    {
        int spawnPointIndex =
            GetFreeSpawnPoint();

        if (spawnPointIndex < 0)
        {
            return;
        }

        int prefabIndex =
            Random.Range(
                0,
                monsterPrefabs.Length
            );

        SpawnRecord record =
            new SpawnRecord();

        record.prefabIndex =
            prefabIndex;

        record.spawnPointIndex =
            spawnPointIndex;

        // 自動EnemyID
        record.enemyId =
            areaId +
            "_Enemy_" +
            currentState
                .nextEnemyNumber
                .ToString("000");

        currentState.nextEnemyNumber++;

        currentState.monsters.Add(
            record
        );

        SpawnMonster(record);
    }


    // -------------------------
    // 実際にPrefab生成
    // -------------------------

    private void SpawnMonster(
        SpawnRecord record)
    {
        if (record.prefabIndex < 0 ||
            record.prefabIndex >=
            monsterPrefabs.Length)
        {
            return;
        }

        if (record.spawnPointIndex < 0 ||
            record.spawnPointIndex >=
            spawnPoints.Length)
        {
            return;
        }

        Transform point =
            spawnPoints[
                record.spawnPointIndex
            ];

        GameObject monster =
            Instantiate(
                monsterPrefabs[
                    record.prefabIndex
                ],
                point.position,
                Quaternion.identity
            );

        // 自動でEnemyID設定
        BattleStartTrigger battleTrigger =
            monster.GetComponent<
                BattleStartTrigger>();

        if (battleTrigger != null)
        {
            battleTrigger.SetEnemyId(
                record.enemyId
            );
        }
    }


    // -------------------------
    // 空いているスポーン地点取得
    // -------------------------

    private int GetFreeSpawnPoint()
    {
        List<int> freePoints =
            new List<int>();

        for (int i = 0;
             i < spawnPoints.Length;
             i++)
        {
            bool used = false;

            foreach (
                SpawnRecord record
                in currentState.monsters)
            {
                if (record.spawnPointIndex == i)
                {
                    used = true;
                    break;
                }
            }

            if (!used)
            {
                freePoints.Add(i);
            }
        }

        if (freePoints.Count == 0)
        {
            return -1;
        }

        return freePoints[
            Random.Range(
                0,
                freePoints.Count
            )
        ];
    }


    // -------------------------
    // 敵補充
    // -------------------------

    private IEnumerator RespawnMonsters(
        int count)
    {
        for (int i = 0;
             i < count;
             i++)
        {
            yield return new WaitForSeconds(
                respawnDelay
            );

            CreateNewMonster();
        }
    }
}