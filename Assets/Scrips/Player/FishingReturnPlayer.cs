using UnityEngine;

public class FishingReturnPlayer : MonoBehaviour
{
    private void Start()
    {
        if (!FishingSceneData.HasReturnPlayerPosition) return;

        if (TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            playerController.TeleportTo(FishingSceneData.ReturnPlayerPosition);
        }
        else
        {
            transform.position = FishingSceneData.ReturnPlayerPosition;
        }

        FishingSceneData.HasReturnPlayerPosition = false;
    }
}