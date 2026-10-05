using UnityEngine;

// 釣りシーンとマップシーンの間で情報を保存するクラス
public static class FishingSceneData
{
    // 釣り後に戻るシーン名
    public static string ReturnSceneName = "Map";

    // 釣り後に戻るPlayer位置
    public static Vector3 ReturnPlayerPosition;

    // Player位置を保存しているか
    public static bool HasReturnPlayerPosition = false;

    // 釣り後に戻るカメラ位置
    public static Vector3 ReturnCameraPosition;

    // カメラ位置を保存しているか
    public static bool HasReturnCameraPosition = false;
}