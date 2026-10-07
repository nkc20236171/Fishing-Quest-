using UnityEngine;

public class BattleEnemySpawner : MonoBehaviour
{
    [SerializeField]
    private Transform enemySpawnPoint;


    private GameObject spawnedEnemy;


    private void Start()
    {
        SpawnEnemy();
    }


    private void SpawnEnemy()
    {
        if (enemySpawnPoint == null)
        {
            Debug.LogError(
                "EnemySpawnPoint‚ªİ’è‚³‚ê‚Ä‚¢‚Ü‚¹‚ñ"
            );

            return;
        }


        if (BattleSceneData
            .CurrentEnemyBattlePrefab == null)
        {
            Debug.LogError(
                "Battle—pEnemyPrefab‚ªİ’è‚³‚ê‚Ä‚¢‚Ü‚¹‚ñ"
            );

            return;
        }


        spawnedEnemy =
            Instantiate(
                BattleSceneData
                    .CurrentEnemyBattlePrefab,
                enemySpawnPoint.position,
                Quaternion.identity
            );
    }
}