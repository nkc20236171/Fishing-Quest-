using UnityEngine;

public class CameraFitArea : MonoBehaviour
{
    // 1画面で見せたい横のマス数
    [SerializeField] private float areaWidth = 16f;

    // 1画面で見せたい縦のマス数
    [SerializeField] private float areaHeight = 9f;

    // 少し余白を入れる
    [SerializeField] private float margin = 0.5f;

    private void Start()
    {
        Camera cam = GetComponent<Camera>();

        if (cam == null) return;
        if (!cam.orthographic) return;

        float sizeByHeight = areaHeight / 2f;
        float sizeByWidth = areaWidth / (2f * cam.aspect);

        cam.orthographicSize = Mathf.Max(sizeByHeight, sizeByWidth) + margin;
    }
}