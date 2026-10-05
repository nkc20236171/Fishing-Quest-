using UnityEngine;

public class FishingReturnCamera : MonoBehaviour
{
    private void Start()
    {
        if (!FishingSceneData.HasReturnCameraPosition) return;

        Vector3 returnPosition = FishingSceneData.ReturnCameraPosition;

        // Z‚Í¡‚ÌƒJƒƒ‰‚ÌZ‚ğg‚¤
        returnPosition.z = transform.position.z;

        transform.position = returnPosition;

        FishingSceneData.HasReturnCameraPosition = false;
    }
}