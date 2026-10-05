using System.Collections;
using UnityEngine;

public class AreaCameraController : MonoBehaviour
{
    // ƒJƒƒ‰ˆÚ“®‚É‚©‚©‚éŠÔ
    [SerializeField] private float moveTime = 0.25f;

    private bool isMoving;

    public void MoveTo(Vector3 targetPosition)
    {
        if (isMoving) return;

        targetPosition.z = -10f;
        StartCoroutine(MoveCamera(targetPosition));
    }

    private IEnumerator MoveCamera(Vector3 targetPosition)
    {
        isMoving = true;

        Vector3 startPosition = transform.position;
        float timer = 0f;

        while (timer < moveTime)
        {
            timer += Time.deltaTime;
            float rate = timer / moveTime;

            transform.position = Vector3.Lerp(startPosition, targetPosition, rate);

            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }
}