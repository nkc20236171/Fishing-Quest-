using UnityEngine;

public class BattlePlayerPlacer : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;

    private void Start()
    {
        if (spawnPoint == null) return;

        transform.position = spawnPoint.position;
    }
}