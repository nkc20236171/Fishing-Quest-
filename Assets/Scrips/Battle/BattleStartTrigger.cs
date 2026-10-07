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


    // -------------------------
    // 敵ステータス
    // -------------------------

    [SerializeField]
    private string enemyName =
        "スライム";

    [SerializeField]
    private int enemyMaxHp = 30;

    [SerializeField]
    private int enemyAttack = 5;


    // -------------------------
    // BattleScene用Prefab
    // -------------------------

    [SerializeField]
    private GameObject battleEnemyPrefab;


    // -------------------------
    // ドロップ
    // -------------------------

    [SerializeField]
    private MonsterDropEntry[] dropItems;


    private bool battleStarted;


    // -------------------------
    // EnemyID設定
    // -------------------------

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


        // -------------------------
        // 戻るシーン
        // -------------------------

        BattleSceneData.ReturnSceneName =
            SceneManager
                .GetActiveScene()
                .name;


        // -------------------------
        // Player位置保存
        // -------------------------

        BattleSceneData.ReturnPlayerPosition =
            other.transform.position;

        BattleSceneData.HasReturnPlayerPosition =
            true;


        // -------------------------
        // カメラ位置保存
        // -------------------------

        if (Camera.main != null)
        {
            BattleSceneData.ReturnCameraPosition =
                Camera.main.transform.position;

            BattleSceneData.HasReturnCameraPosition =
                true;
        }


        // -------------------------
        // 敵情報
        // -------------------------

        BattleSceneData.CurrentEnemyId =
            enemyId;

        BattleSceneData.EnemyName =
            enemyName;

        BattleSceneData.EnemyMaxHp =
            enemyMaxHp;

        BattleSceneData.EnemyAttack =
            enemyAttack;


        // BattleSceneで表示するPrefab
        BattleSceneData.CurrentEnemyBattlePrefab =
            battleEnemyPrefab;


        // ドロップ情報
        BattleSceneData.SetDrops(
            dropItems
        );


        // -------------------------
        // BattleSceneへ
        // -------------------------

        SceneManager.LoadScene(
            battleSceneName
        );
    }
}