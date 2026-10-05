using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StatusWindowController : MonoBehaviour
{
    // ステータス画面が開いているか
    public static bool IsOpen { get; private set; }

    // ステータス画面のPanel
    [SerializeField] private GameObject statusPanel;

    // ステータス表示用Text
    [SerializeField] private Text statusText;

    // Playerのステータス
    [SerializeField] private PlayerStatus playerStatus;

    private void Start()
    {
        if (playerStatus == null)
        {
            playerStatus = FindFirstObjectByType<PlayerStatus>();
        }

        CloseStatus();
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (IsOpen)
            {
                CloseStatus();
            }
            else
            {
                OpenStatus();
            }
        }
    }

    private void OpenStatus()
    {
        IsOpen = true;

        statusPanel.SetActive(true);
        UpdateStatusText();
    }

    private void CloseStatus()
    {
        IsOpen = false;

        statusPanel.SetActive(false);
    }

    private void UpdateStatusText()
    {
        if (playerStatus == null) return;

        statusText.text =
            "ステータス\n\n" +
            "名前：" + playerStatus.playerName + "\n" +
            "HP：" + playerStatus.currentHp + " / " + playerStatus.maxHp + "\n" +
            "攻撃力：" + playerStatus.attack + "\n" +
            "防御力：" + playerStatus.defense + "\n" +
            "お金：" + playerStatus.money + " G";
    }
}