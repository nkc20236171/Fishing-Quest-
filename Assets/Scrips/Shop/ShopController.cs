using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    // ショップが開いているか
    public static bool IsOpen { get; private set; }

    // ショップ全体のPanel
    [SerializeField] private GameObject shopPanel;

    // 商品・メニュー一覧表示
    [SerializeField] private Text itemText;

    // メッセージ表示
    [SerializeField] private Text messageText;

    // 所持金・所持数表示
    [SerializeField] private Text moneyText;

    // Playerのステータス
    [SerializeField] private PlayerStatus playerStatus;

    // Playerの持ち物
    [SerializeField] private PlayerInventory playerInventory;

    private int selectedIndex;

    private bool canInput;

    private ShopState shopState;

    // 現在開いている店の種類
    private ShopNPC.ShopType currentShopType;

    private enum ShopState
    {
        MainMenu,
        Buy,
        Sell
    }

    // 現在の店の商品名
    private string[] currentItemNames;

    // 現在の店の買値
    private int[] currentBuyPrices;

    // 現在の店の売値
    private int[] currentSellPrices;

    private void Start()
    {
        if (playerStatus == null)
        {
            playerStatus = FindAnyObjectByType<PlayerStatus>();
        }

        if (playerInventory == null)
        {
            playerInventory = FindAnyObjectByType<PlayerInventory>();
        }

        CloseShop();
    }

    private void Update()
    {
        if (!IsOpen) return;
        if (!canInput) return;
        if (Keyboard.current == null) return;

        // 上
        if (Keyboard.current.wKey.wasPressedThisFrame ||
            Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            selectedIndex--;

            if (selectedIndex < 0)
            {
                selectedIndex = GetCurrentMenuLength() - 1;
            }

            UpdateShopText();
        }

        // 下
        if (Keyboard.current.sKey.wasPressedThisFrame ||
            Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            selectedIndex++;

            if (selectedIndex >= GetCurrentMenuLength())
            {
                selectedIndex = 0;
            }

            UpdateShopText();
        }

        // F または Enterで決定
        if (Keyboard.current.fKey.wasPressedThisFrame ||
            Keyboard.current.enterKey.wasPressedThisFrame)
        {
            Decide();
        }

        // Escapeで戻る
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (shopState == ShopState.MainMenu)
            {
                CloseShop();
            }
            else
            {
                OpenMainMenu();
            }
        }
    }

    // -------------------------
    // ショップを開く
    // -------------------------

    public void OpenShop(ShopNPC.ShopType shopType)
    {
        IsOpen = true;

        // 今開いている店の種類を保存
        currentShopType = shopType;

        // NPCに話しかけた瞬間のF入力を無視
        canInput = false;

        // 店舗の商品を設定
        SetupShopItems(shopType);

        shopPanel.SetActive(true);

        OpenMainMenu();

        StartCoroutine(EnableInputNextFrame());
    }

    // -------------------------
    // ショップを閉じる
    // -------------------------

    public void CloseShop()
    {
        IsOpen = false;
        canInput = false;

        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    // -------------------------
    // 次のフレームから入力受付
    // -------------------------

    private IEnumerator EnableInputNextFrame()
    {
        yield return null;

        canInput = true;
    }

    // -------------------------
    // 店舗ごとの商品設定
    // -------------------------

    private void SetupShopItems(ShopNPC.ShopType shopType)
    {
        // 通常ショップ
        if (shopType == ShopNPC.ShopType.ItemShop)
        {
            currentItemNames = new string[]
            {
                "やくそう",
                "まほうの水",
                "きずぐすり"
            };

            currentBuyPrices = new int[]
            {
                10,
                20,
                30
            };

            currentSellPrices = new int[]
            {
                5,
                10,
                15
            };
        }

        // 釣具屋
        else if (shopType == ShopNPC.ShopType.FishingShop)
        {
            currentItemNames = new string[]
            {
                "木の釣り竿",
                "丈夫なリール",
                "ミミズ"
            };

            currentBuyPrices = new int[]
            {
                50,
                80,
                10
            };

            currentSellPrices = new int[]
            {
                25,
                40,
                5
            };
        }

        // 武器屋
        else if (shopType == ShopNPC.ShopType.WeaponShop)
        {
            currentItemNames = new string[]
            {
                "どうのつるぎ",
                "てつのつるぎ",
                "木の盾",
                "皮のよろい"
            };

            currentBuyPrices = new int[]
            {
                100,
                250,
                80,
                150
            };

            currentSellPrices = new int[]
            {
                50,
                125,
                40,
                75
            };
        }
    }

    // -------------------------
    // メインメニュー
    // -------------------------

    private void OpenMainMenu()
    {
        shopState = ShopState.MainMenu;
        selectedIndex = 0;

        messageText.text = "何をしますか？";

        UpdateShopText();
    }

    // -------------------------
    // 決定処理
    // -------------------------

    private void Decide()
    {
        if (shopState == ShopState.MainMenu)
        {
            DecideMainMenu();
        }
        else if (shopState == ShopState.Buy)
        {
            DecideBuy();
        }
        else if (shopState == ShopState.Sell)
        {
            DecideSell();
        }
    }

    // -------------------------
    // メインメニュー決定
    // -------------------------

    private void DecideMainMenu()
    {
        // 買う
        if (selectedIndex == 0)
        {
            shopState = ShopState.Buy;
            selectedIndex = 0;

            messageText.text = "何を買いますか？";

            UpdateShopText();
        }

        // 売る
        else if (selectedIndex == 1)
        {
            shopState = ShopState.Sell;
            selectedIndex = 0;

            messageText.text = "何を売りますか？";

            UpdateShopText();
        }

        // 店を出る
        else if (selectedIndex == 2)
        {
            CloseShop();
        }
    }

    // -------------------------
    // 購入処理
    // -------------------------

    private void DecideBuy()
    {
        // 一番下は「もどる」
        if (selectedIndex == currentItemNames.Length)
        {
            OpenMainMenu();
            return;
        }

        int price = currentBuyPrices[selectedIndex];

        if (playerStatus.money < price)
        {
            messageText.text = "お金が足りない！";
            return;
        }

        playerStatus.money -= price;

        AddItem(selectedIndex);

        messageText.text =
            currentItemNames[selectedIndex] +
            "を買った！";

        UpdateShopText();
    }

    // -------------------------
    // 売却処理
    // -------------------------

    private void DecideSell()
    {
        // 一番下は「もどる」
        if (selectedIndex == currentItemNames.Length)
        {
            OpenMainMenu();
            return;
        }

        int count = GetItemCount(selectedIndex);

        if (count <= 0)
        {
            messageText.text =
                currentItemNames[selectedIndex] +
                "を持っていない！";

            return;
        }

        RemoveItem(selectedIndex);

        playerStatus.money += currentSellPrices[selectedIndex];

        messageText.text =
            currentItemNames[selectedIndex] +
            "を " +
            currentSellPrices[selectedIndex] +
            "Gで売った！";

        UpdateShopText();
    }

    // -------------------------
    // アイテム追加
    // -------------------------

    private void AddItem(int index)
    {
        // 通常ショップ
        if (currentShopType == ShopNPC.ShopType.ItemShop)
        {
            if (index == 0)
            {
                playerInventory.herbCount++;
            }
            else if (index == 1)
            {
                playerInventory.magicWaterCount++;
            }
            else if (index == 2)
            {
                playerInventory.potionCount++;
            }
        }

        // 釣具屋
        else if (currentShopType == ShopNPC.ShopType.FishingShop)
        {
            if (index == 0)
            {
                playerInventory.fishingRodCount++;
            }
            else if (index == 1)
            {
                playerInventory.reelCount++;
            }
            else if (index == 2)
            {
                playerInventory.baitCount++;
            }
        }

        // 武器屋
        else if (currentShopType == ShopNPC.ShopType.WeaponShop)
        {
            if (index == 0)
            {
                playerInventory.copperSwordCount++;
            }
            else if (index == 1)
            {
                playerInventory.ironSwordCount++;
            }
            else if (index == 2)
            {
                playerInventory.woodenShieldCount++;
            }
            else if (index == 3)
            {
                playerInventory.leatherArmorCount++;
            }
        }
    }

    // -------------------------
    // アイテム削除
    // -------------------------

    private void RemoveItem(int index)
    {
        // 通常ショップ
        if (currentShopType == ShopNPC.ShopType.ItemShop)
        {
            if (index == 0)
            {
                playerInventory.herbCount--;
            }
            else if (index == 1)
            {
                playerInventory.magicWaterCount--;
            }
            else if (index == 2)
            {
                playerInventory.potionCount--;
            }
        }

        // 釣具屋
        else if (currentShopType == ShopNPC.ShopType.FishingShop)
        {
            if (index == 0)
            {
                playerInventory.fishingRodCount--;
            }
            else if (index == 1)
            {
                playerInventory.reelCount--;
            }
            else if (index == 2)
            {
                playerInventory.baitCount--;
            }
        }

        // 武器屋
        else if (currentShopType == ShopNPC.ShopType.WeaponShop)
        {
            if (index == 0)
            {
                playerInventory.copperSwordCount--;
            }
            else if (index == 1)
            {
                playerInventory.ironSwordCount--;
            }
            else if (index == 2)
            {
                playerInventory.woodenShieldCount--;
            }
            else if (index == 3)
            {
                playerInventory.leatherArmorCount--;
            }
        }
    }

    // -------------------------
    // 所持数取得
    // -------------------------

    private int GetItemCount(int index)
    {
        // 通常ショップ
        if (currentShopType == ShopNPC.ShopType.ItemShop)
        {
            if (index == 0)
            {
                return playerInventory.herbCount;
            }

            if (index == 1)
            {
                return playerInventory.magicWaterCount;
            }

            if (index == 2)
            {
                return playerInventory.potionCount;
            }
        }

        // 釣具屋
        else if (currentShopType == ShopNPC.ShopType.FishingShop)
        {
            if (index == 0)
            {
                return playerInventory.fishingRodCount;
            }

            if (index == 1)
            {
                return playerInventory.reelCount;
            }

            if (index == 2)
            {
                return playerInventory.baitCount;
            }

        }

        // 武器屋
        else if (currentShopType == ShopNPC.ShopType.WeaponShop)
        {
            if (index == 0)
            {
                return playerInventory.copperSwordCount;
            }

            if (index == 1)
            {
                return playerInventory.ironSwordCount;
            }

            if (index == 2)
            {
                return playerInventory.woodenShieldCount;
            }

            if (index == 3)
            {
                return playerInventory.leatherArmorCount;
            }
        }

        return 0;
    }

    // -------------------------
    // 表示更新
    // -------------------------

    private void UpdateShopText()
    {
        if (shopState == ShopState.MainMenu)
        {
            UpdateMainMenuText();
        }
        else if (shopState == ShopState.Buy)
        {
            UpdateBuyText();
        }
        else if (shopState == ShopState.Sell)
        {
            UpdateSellText();
        }

        UpdateMoneyText();
    }

    // -------------------------
    // メインメニュー表示
    // -------------------------

    private void UpdateMainMenuText()
    {
        string[] menu =
        {
            "買う",
            "売る",
            "店を出る"
        };

        string text = "";

        for (int i = 0; i < menu.Length; i++)
        {
            if (i == selectedIndex)
            {
                text += "▶ ";
            }
            else
            {
                text += "　";
            }

            text += menu[i] + "\n";
        }

        itemText.text = text;
    }

    // -------------------------
    // 購入画面表示
    // -------------------------

    private void UpdateBuyText()
    {
        string text = "";

        for (int i = 0; i < currentItemNames.Length; i++)
        {
            if (i == selectedIndex)
            {
                text += "▶ ";
            }
            else
            {
                text += "　";
            }

            text +=
                currentItemNames[i] +
                "　" +
                currentBuyPrices[i] +
                "G\n";
        }

        // もどる
        if (selectedIndex == currentItemNames.Length)
        {
            text += "▶ もどる\n";
        }
        else
        {
            text += "　もどる\n";
        }

        itemText.text = text;
    }

    // -------------------------
    // 売却画面表示
    // -------------------------

    private void UpdateSellText()
    {
        string text = "";

        for (int i = 0; i < currentItemNames.Length; i++)
        {
            if (i == selectedIndex)
            {
                text += "▶ ";
            }
            else
            {
                text += "　";
            }

            text +=
                currentItemNames[i] +
                " ×" +
                GetItemCount(i) +
                "　売値 " +
                currentSellPrices[i] +
                "G\n";
        }

        // もどる
        if (selectedIndex == currentItemNames.Length)
        {
            text += "▶ もどる\n";
        }
        else
        {
            text += "　もどる\n";
        }

        itemText.text = text;
    }

    // -------------------------
    // 所持金・現在の店の商品所持数
    // -------------------------

    private void UpdateMoneyText()
    {
        string text =
            "所持金：" +
            playerStatus.money +
            "G\n\n";

        if (currentItemNames != null)
        {
            for (int i = 0; i < currentItemNames.Length; i++)
            {
                text +=
                    currentItemNames[i] +
                    "：" +
                    GetItemCount(i) +
                    "\n";
            }
        }

        moneyText.text = text;
    }

    // -------------------------
    // 現在のメニュー項目数
    // -------------------------

    private int GetCurrentMenuLength()
    {
        if (shopState == ShopState.MainMenu)
        {
            return 3;
        }

        // 商品数 + もどる
        return currentItemNames.Length + 1;
    }
}