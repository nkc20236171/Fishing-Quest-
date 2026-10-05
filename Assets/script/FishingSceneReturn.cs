using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FishingSceneReturn : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current == null) return;

        // Rキー または Escapeキーでマップへ戻る
        if (Keyboard.current.rKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ReturnToMap();
        }
    }

    // 釣り担当の人の処理からも呼べるようにpublicにしておく
    public void ReturnToMap()
    {
        SceneManager.LoadScene(FishingSceneData.ReturnSceneName);
    }
}