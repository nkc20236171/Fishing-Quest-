using System.Collections;
using UnityEngine;

public class MonsterChase : MonoBehaviour
{
    // 追いかけるPlayer
    [SerializeField] private Transform player;

    // 何マス以内なら追いかけるか
    [SerializeField] private float chaseRange = 6f;

    // 1マス移動にかかる時間
    [SerializeField] private float moveTime = 0.25f;

    // 次の移動までの待ち時間
    [SerializeField] private float moveInterval = 0.4f;

    private bool isMoving;
    private float moveTimer;

    private void Start()
    {
        if (player == null)
        {
            PlayerController playerController =
                FindAnyObjectByType<PlayerController>();

            if (playerController != null)
            {
                player = playerController.transform;
            }
        }
    }

    private void Update()
    {
        if (player == null) return;
        if (isMoving) return;

        // ショップ中は動かさない
        if (ShopController.IsOpen) return;

        // 宿屋中は動かさない
        if (InnController.IsOpen) return;

        moveTimer -= Time.deltaTime;

        if (moveTimer > 0f) return;

        float distance =
            Vector2.Distance(transform.position, player.position);

        if (distance <= chaseRange)
        {
            Vector2 direction = GetChaseDirection();

            if (direction != Vector2.zero)
            {
                StartCoroutine(MoveOneTile(direction));
            }
        }

        moveTimer = moveInterval;
    }

    private Vector2 GetChaseDirection()
    {
        Vector2 difference =
            player.position - transform.position;

        // 横方向の距離の方が大きい
        if (Mathf.Abs(difference.x) >
            Mathf.Abs(difference.y))
        {
            if (difference.x > 0)
            {
                return Vector2.right;
            }
            else
            {
                return Vector2.left;
            }
        }

        // 縦方向
        if (difference.y > 0)
        {
            return Vector2.up;
        }
        else if (difference.y < 0)
        {
            return Vector2.down;
        }

        return Vector2.zero;
    }

    private IEnumerator MoveOneTile(Vector2 direction)
    {
        isMoving = true;

        Vector3 startPosition = transform.position;

        Vector3 targetPosition =
            startPosition +
            (Vector3)direction;

        float timer = 0f;

        while (timer < moveTime)
        {
            timer += Time.deltaTime;

            float rate =
                timer / moveTime;

            transform.position =
                Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    rate
                );

            yield return null;
        }

        transform.position = targetPosition;

        isMoving = false;
    }
}