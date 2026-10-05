using UnityEngine;

public class BattleReturnPlayer : MonoBehaviour
{
    private void Start()
    {
        if (!BattleSceneData.HasReturnPlayerPosition) return;

        if (TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            playerController.TeleportTo(BattleSceneData.ReturnPlayerPosition);
        }
        else
        {
            transform.position = BattleSceneData.ReturnPlayerPosition;
        }

        BattleSceneData.HasReturnPlayerPosition = false;
    }
}