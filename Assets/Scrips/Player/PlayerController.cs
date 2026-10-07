using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    // 1マスの大きさ
    [SerializeField] private float tileSize = 1f;

    // 1マス移動にかかる時間
    [SerializeField] private float moveTime = 0.15f;

    private Rigidbody2D rb;
    private bool isMoving;

    public static PlayerController Instance;

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        rb = GetComponent<Rigidbody2D>();

        Vector2 snappedPos =
            SnapToTileCenter(
                transform.position
            );

        transform.position =
            snappedPos;

        rb.position =
            snappedPos;
    }

    private void Update()
    {
        // バトルシーンでは移動させない
        if (SceneManager.GetActiveScene().name == "BattleScene")
        {
            return;
        }

        if (StatusWindowController.IsOpen) return;
        if (ShopController.IsOpen) return;
        if (InnController.IsOpen) return;

        if (isMoving) return;

        Vector2 inputDirection =
            GetInputDirection();

        if (inputDirection != Vector2.zero)
        {
            StartCoroutine(
                MoveOneTile(inputDirection)
            );
        }
    }

    private Vector2 GetInputDirection()
    {
        if (Keyboard.current == null) return Vector2.zero;

        // 左
        if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            return Vector2.left;
        }

        // 右
        if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            return Vector2.right;
        }

        // 上
        if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            return Vector2.up;
        }

        // 下
        if (Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            return Vector2.down;
        }

        return Vector2.zero;
    }

    private IEnumerator MoveOneTile(Vector2 direction)
    {
        isMoving = true;

        Vector2 startPos = rb.position;
        Vector2 targetPos = startPos + direction * tileSize;

        float timer = 0f;

        while (timer < moveTime)
        {
            timer += Time.deltaTime;

            float rate = timer / moveTime;
            Vector2 nextPos = Vector2.Lerp(startPos, targetPos, rate);

            rb.MovePosition(nextPos);

            yield return null;
        }

        rb.MovePosition(targetPos);

        isMoving = false;
    }

    // 指定した場所にPlayerを移動させる
    public void TeleportTo(Vector3 targetPosition)
    {
        StopAllCoroutines();

        isMoving = false;

        Vector2 snappedPos = SnapToTileCenter(targetPosition);

        transform.position = new Vector3(snappedPos.x, snappedPos.y, transform.position.z);

        if (rb != null)
        {
            rb.position = snappedPos;
            rb.linearVelocity = Vector2.zero;
        }
    }

    // Tilemapのマス中心に合わせる
    private Vector2 SnapToTileCenter(Vector3 position)
    {
        return new Vector2(
            Mathf.Floor(position.x) + 0.5f,
            Mathf.Floor(position.y) + 0.5f
        );
    }
}