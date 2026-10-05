using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InnController : MonoBehaviour
{
    public static bool IsOpen { get; private set; }

    [SerializeField] private GameObject innPanel;
    [SerializeField] private Text commandText;
    [SerializeField] private Text messageText;
    [SerializeField] private Text moneyText;

    [SerializeField] private PlayerStatus playerStatus;

    [SerializeField] private int innPrice = 20;

    private int selectedIndex;
    private bool canInput;

    private string[] commands =
    {
        "泊まる",
        "やめる"
    };

    private void Start()
    {
        if (playerStatus == null)
        {
            playerStatus = FindAnyObjectByType<PlayerStatus>();
        }

        CloseInn();
    }

    private void Update()
    {
        if (!IsOpen) return;
        if (!canInput) return;
        if (Keyboard.current == null) return;

        if (Keyboard.current.wKey.wasPressedThisFrame ||
            Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            selectedIndex--;

            if (selectedIndex < 0)
            {
                selectedIndex = commands.Length - 1;
            }

            UpdateCommandText();
        }

        if (Keyboard.current.sKey.wasPressedThisFrame ||
            Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            selectedIndex++;

            if (selectedIndex >= commands.Length)
            {
                selectedIndex = 0;
            }

            UpdateCommandText();
        }

        if (Keyboard.current.fKey.wasPressedThisFrame ||
            Keyboard.current.enterKey.wasPressedThisFrame)
        {
            Decide();
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseInn();
        }
    }

    public void OpenInn()
    {
        IsOpen = true;
        canInput = false;

        selectedIndex = 0;

        innPanel.SetActive(true);

        messageText.text =
            "一晩 " +
            innPrice +
            "Gですが、泊まりますか？";

        UpdateCommandText();
        UpdateMoneyText();

        StartCoroutine(EnableInputNextFrame());
    }

    public void CloseInn()
    {
        IsOpen = false;
        canInput = false;

        if (innPanel != null)
        {
            innPanel.SetActive(false);
        }
    }

    private IEnumerator EnableInputNextFrame()
    {
        yield return null;

        canInput = true;
    }

    private void Decide()
    {
        if (selectedIndex == 0)
        {
            StayInn();
        }
        else
        {
            CloseInn();
        }
    }

    private void StayInn()
    {
        if (playerStatus.money < innPrice)
        {
            messageText.text = "お金が足りません！";
            return;
        }

        playerStatus.money -= innPrice;

        // HP全回復
        playerStatus.currentHp = playerStatus.maxHp;

        // セーブ
        SaveGame();

        messageText.text =
            "ゆっくり休んだ！\n" +
            "HPが全回復した！\n" +
            "セーブしました。";

        UpdateMoneyText();
    }

    private void SaveGame()
    {
        
        PlayerPrefs.SetInt("PlayerCurrentHp", playerStatus.currentHp);
        PlayerPrefs.SetInt("PlayerMaxHp", playerStatus.maxHp);
        PlayerPrefs.SetInt("PlayerAttack", playerStatus.attack);
        PlayerPrefs.SetInt("PlayerDefense", playerStatus.defense);
        PlayerPrefs.SetInt("PlayerMoney", playerStatus.money);

        PlayerPrefs.Save();
    }

    private void UpdateCommandText()
    {
        string text = "";

        for (int i = 0; i < commands.Length; i++)
        {
            if (i == selectedIndex)
            {
                text += "▶ ";
            }
            else
            {
                text += "　";
            }

            text += commands[i];

            if (i == 0)
            {
                text += "　" + innPrice + "G";
            }

            text += "\n";
        }

        commandText.text = text;
    }

    private void UpdateMoneyText()
    {
        moneyText.text =
            "所持金：" +
            playerStatus.money +
            "G";
    }
}