using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FishingSceneTrigger : MonoBehaviour
{
    // 移動先の釣りシーン名
    [SerializeField] private string fishingSceneName = "FishingScene";

    private bool canMoveToFishingScene;

    private void Update()
    {
        if (!canMoveToFishingScene) return;
        if (Keyboard.current == null) return;

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            // 今いるマップシーン名を保存
            FishingSceneData.ReturnSceneName = SceneManager.GetActiveScene().name;

            // Player位置を保存
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                FishingSceneData.ReturnPlayerPosition = player.transform.position;
                FishingSceneData.HasReturnPlayerPosition = true;
            }

            // カメラ位置を保存
            if (Camera.main != null)
            {
                FishingSceneData.ReturnCameraPosition = Camera.main.transform.position;
                FishingSceneData.HasReturnCameraPosition = true;
            }

            // 釣りシーンへ移動
            SceneManager.LoadScene(fishingSceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out _)) return;

        canMoveToFishingScene = true;
        Debug.Log("Fキーで釣りシーンへ移動");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out _)) return;

        canMoveToFishingScene = false;
        Debug.Log("釣り場から離れた");
    }
}