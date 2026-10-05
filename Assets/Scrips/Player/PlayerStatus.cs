using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public static PlayerStatus Instance;

    public string playerName = "カイ";

    public int currentHp = 100;
    public int maxHp = 100;

    public int attack = 10;
    public int defense = 5;

    public int money = 0;


    private void Awake()
    {
        // すでにPlayerStatusが存在する場合
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // シーン移動しても削除しない
        DontDestroyOnLoad(gameObject);
    }
}