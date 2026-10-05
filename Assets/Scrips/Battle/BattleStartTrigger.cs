using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleStartTrigger : MonoBehaviour
{
    // Spawnerが自動設定
    [SerializeField]
    private string enemyId = "AUTO";

    // バトルシーン
    [SerializeField]
    private string battleSceneName =
        "BattleScene";

    // 敵名
    [SerializeField]
    private string enemyName =
        "スライム";

    // HP
    [SerializeField]
    private int enemyMaxHp = 30;

    // 攻撃力
    [SerializeField]
    private int enemyAttack = 5;

    // ドロップ品
    [SerializeField]
    private MonsterDropEntry[] dropItems;

    private bool battleStarted;


    public void SetEnemyId(
        string newEnemyId)
    {
        enemyId = newEnemyId;
    }


    private void Start()
    {
        if (BattleSceneData
            .DefeatedEnemyIds
            .Contains(enemyId))
        {
            Destroy(gameObject);
            return;
        }

        if (BattleSceneData
            .DespawnedEnemyIds
            .Contains(enemyId))
        {
            Destroy(gameObject);
            return;
        }
    }


    private void OnTriggerEnter2D(
        Collider2D other)
    {
        if (battleStarted)
        {
            return;
        }

        if (!other.TryGetComponent
            <PlayerController>(out _))
        {
            return;
        }

        battleStarted = true;

        BattleSceneData.ReturnSceneName =
            SceneManager
                .GetActiveScene()
                .name;

        BattleSceneData
            .ReturnPlayerPosition =
            other.transform.position;

        BattleSceneData
            .HasReturnPlayerPosition =
            true;

        if (Camera.main != null)
        {
            BattleSceneData
                .ReturnCameraPosition =
                Camera.main
                    .transform
                    .position;

            BattleSceneData
                .HasReturnCameraPosition =
                true;
        }

        BattleSceneData.CurrentEnemyId =
            enemyId;

        BattleSceneData.EnemyName =
            enemyName;

        BattleSceneData.EnemyMaxHp =
            enemyMaxHp;

        BattleSceneData.EnemyAttack =
            enemyAttack;

        // ドロップ情報をBattleSceneへ
        BattleSceneData.SetDrops(
            dropItems
        );

        SceneManager.LoadScene(
            battleSceneName
        );
    }
}