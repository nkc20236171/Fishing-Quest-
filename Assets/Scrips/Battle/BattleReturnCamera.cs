using UnityEngine;

public class BattleReturnCamera : MonoBehaviour
{
    private void Start()
    {
        if (!BattleSceneData.HasReturnCameraPosition) return;

        Vector3 returnPosition = BattleSceneData.ReturnCameraPosition;

        // ƒJƒƒ‰‚ÌZ‚Í•K‚¸¡‚ÌZ‚ğg‚¤
        returnPosition.z = transform.position.z;

        transform.position = returnPosition;

        BattleSceneData.HasReturnCameraPosition = false;
    }
}