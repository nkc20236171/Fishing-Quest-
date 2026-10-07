using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSceneMover : MonoBehaviour
{
    private PlayerController playerController;

    private void Awake()
    {
        playerController =
            GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded +=
            OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -=
            OnSceneLoaded;
    }


    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode)
    {
        // -------------------------
        // BattleScene
        // -------------------------

        if (scene.name == "BattleScene")
        {
            MoveToBattlePosition();

            return;
        }


        // -------------------------
        // Mapへ戻った
        // -------------------------

        ReturnToMapPosition();
    }


    private void MoveToBattlePosition()
    {
        GameObject spawnPoint =
            GameObject.Find(
                "BattlePlayerSpawnPoint"
            );

        if (spawnPoint == null)
        {
            Debug.LogError(
                "BattlePlayerSpawnPointがありません"
            );

            return;
        }

        // バトル用位置へ
        playerController.TeleportTo(
            spawnPoint.transform.position
        );

        // バトル中はMap移動を停止
        playerController.enabled = false;
    }


    private void ReturnToMapPosition()
    {
        // 移動を再開
        playerController.enabled = true;

        if (!BattleSceneData
            .HasReturnPlayerPosition)
        {
            return;
        }

        // 敵と接触した位置へ戻る
        playerController.TeleportTo(
            BattleSceneData
                .ReturnPlayerPosition
        );

        BattleSceneData
            .HasReturnPlayerPosition =
            false;
    }
}