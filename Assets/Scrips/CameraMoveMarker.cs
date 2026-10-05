using UnityEngine;

public class CameraMoveMarker : MonoBehaviour
{
    // “®‚©‚·ƒJƒƒ‰
    [SerializeField] private AreaCameraController cameraController;

    // ƒJƒƒ‰‚ÌˆÚ“®æ
    [SerializeField] private Transform cameraTargetPoint;

    // Player‚ÌˆÚ“®æ
    [SerializeField] private Transform playerTargetPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // PlayerController‚ğ‚Á‚Ä‚¢‚é‘Šè‚¾‚¯”½‰‚·‚é
        if (!other.TryGetComponent<PlayerController>(out PlayerController player)) return;

        if (cameraController == null) return;
        if (cameraTargetPoint == null) return;
        if (playerTargetPoint == null) return;

        // ƒJƒƒ‰‚ğˆÚ“®
        cameraController.MoveTo(cameraTargetPoint.position);

        // Player‚ğŸ‚Ì“üŒû‚ÖˆÚ“®
        player.TeleportTo(playerTargetPoint.position);
    }
}