using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    // -------------------------
    // UI
    // -------------------------

    [SerializeField]
    private Text playerHpText;

    [SerializeField]
    private Text enemyHpText;

    [SerializeField]
    private Text messageText;

    [SerializeField]
    private Text commandText;

    [SerializeField]
    private Text gaugeText;


    // -------------------------
    // Player
    // -------------------------

    [SerializeField]
    private PlayerStatus playerStatus;


    // -------------------------
    // 敵
    // -------------------------

    private int enemyCurrentHp;


    // -------------------------
    // コマンド
    // -------------------------

    private readonly string[] commands =
    {
        "たたかう",
        "どうぐ",
        "にげる"
    };

    private int commandIndex;


    // -------------------------
    // アイテム
    // -------------------------

    private readonly string[] itemCommands =
    {
        "やくそう",
        "きずぐすり",
        "もどる"
    };

    private int itemIndex;


    // -------------------------
    // Playerゲージ
    // -------------------------

    private int playerActionGauge;
    private int playerAttackGauge;
    private int playerDefenseGauge;


    // -------------------------
    // Enemyゲージ
    // -------------------------

    private int enemyActionGauge;
    private int enemyAttackGauge;
    private int enemyDefenseGauge;


    // -------------------------
    // バトル状態
    // -------------------------

    private enum BattleState
    {
        Command,

        SelectAction,
        SelectAttack,
        SelectDefense,
        ConfirmGauge,

        Item,

        Processing,

        BattleEnd
    }

    private BattleState currentState;


    private void Start()
    {
        if (PlayerStatus.Instance != null)
        {
            playerStatus =
                PlayerStatus.Instance;
        }
        else
        {
            playerStatus =
                FindAnyObjectByType<PlayerStatus>();
        }

        if (playerStatus == null)
        {
            Debug.LogError(
                "PlayerStatusが見つかりません"
            );

            enabled = false;
            return;
        }

        // 敵HP設定
        enemyCurrentHp =
            BattleSceneData.EnemyMaxHp;

        // Playerゲージ初期化
        ResetPlayerGauge();

        // 敵ゲージ決定
        AllocateEnemyGauge();

        // HP表示
        UpdateHpText();

        // ゲージ表示
        UpdateGaugeText();

        // コマンド開始
        OpenCommandMenu();

        messageText.text =
            BattleSceneData.EnemyName +
            "があらわれた！";
    }


    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (currentState ==
            BattleState.Processing)
        {
            return;
        }

        if (currentState ==
            BattleState.BattleEnd)
        {
            return;
        }

        switch (currentState)
        {
            case BattleState.Command:

                CommandInput();

                break;


            case BattleState.SelectAction:

                ActionGaugeInput();

                break;


            case BattleState.SelectAttack:

                AttackGaugeInput();

                break;


            case BattleState.SelectDefense:

                DefenseGaugeInput();

                break;


            case BattleState.ConfirmGauge:

                ConfirmGaugeInput();

                break;


            case BattleState.Item:

                ItemInput();

                break;
        }
    }


    // =========================
    // コマンド
    // =========================

    private void OpenCommandMenu()
    {
        currentState =
            BattleState.Command;

        commandIndex = 0;

        UpdateCommandText();
    }


    private void CommandInput()
    {
        if (Keyboard.current.wKey
                .wasPressedThisFrame ||
            Keyboard.current.upArrowKey
                .wasPressedThisFrame)
        {
            commandIndex--;

            if (commandIndex < 0)
            {
                commandIndex =
                    commands.Length - 1;
            }

            UpdateCommandText();
        }


        if (Keyboard.current.sKey
                .wasPressedThisFrame ||
            Keyboard.current.downArrowKey
                .wasPressedThisFrame)
        {
            commandIndex++;

            if (commandIndex >=
                commands.Length)
            {
                commandIndex = 0;
            }

            UpdateCommandText();
        }


        if (IsConfirmPressed())
        {
            DecideCommand();
        }
    }


    private void DecideCommand()
    {
        switch (commandIndex)
        {
            // たたかう
            case 0:

                StartGaugeSelection();

                break;


            // どうぐ
            case 1:

                OpenItemMenu();

                break;


            // にげる
            case 2:

                StartCoroutine(
                    PlayerRun()
                );

                break;
        }
    }


    private void UpdateCommandText()
    {
        commandText.text = "";

        for (int i = 0;
             i < commands.Length;
             i++)
        {
            if (i == commandIndex)
            {
                commandText.text +=
                    "▶ " +
                    commands[i];
            }
            else
            {
                commandText.text +=
                    "　" +
                    commands[i];
            }

            if (i <
                commands.Length - 1)
            {
                commandText.text +=
                    "\n";
            }
        }
    }


    // =========================
    // Playerゲージ
    // =========================

    private void StartGaugeSelection()
    {
        ResetPlayerGauge();

        currentState =
            BattleState.SelectAction;

        commandText.text =
            "行動力を数字キーで入力";

        messageText.text =
            "行動・攻撃・防御の合計を" +
            "10にしてください。";

        UpdateGaugeText();
    }


    private void ActionGaugeInput()
    {
        int number =
            GetPressedNumber();

        if (number < 0)
        {
            return;
        }

        playerActionGauge =
            number;

        currentState =
            BattleState.SelectAttack;

        commandText.text =
            "攻撃力を数字キーで入力";

        UpdateGaugeText();
    }


    private void AttackGaugeInput()
    {
        int number =
            GetPressedNumber();

        if (number < 0)
        {
            return;
        }

        int total =
            playerActionGauge +
            number;

        if (total > 10)
        {
            messageText.text =
                "合計が10を超えています！";

            return;
        }

        playerAttackGauge =
            number;

        currentState =
            BattleState.SelectDefense;

        commandText.text =
            "防御力を数字キーで入力";

        UpdateGaugeText();
    }


    private void DefenseGaugeInput()
    {
        int number =
            GetPressedNumber();

        if (number < 0)
        {
            return;
        }

        int total =
            playerActionGauge +
            playerAttackGauge +
            number;

        if (total != 10)
        {
            messageText.text =
                "合計を10にしてください！" +
                "\n現在：" +
                total;

            return;
        }

        playerDefenseGauge =
            number;

        currentState =
            BattleState.ConfirmGauge;

        commandText.text =
            "F：決定" +
            "\n0：やり直す";

        messageText.text =
            "この配分で決定しますか？";

        UpdateGaugeText();
    }


    private void ConfirmGaugeInput()
    {
        // 0キーでリセット
        if (Keyboard.current.digit0Key
                .wasPressedThisFrame ||
            Keyboard.current.numpad0Key
                .wasPressedThisFrame)
        {
            StartGaugeSelection();

            return;
        }

        if (!IsConfirmPressed())
        {
            return;
        }

        StartCoroutine(ExecuteBattleTurn());
    }


    private void ResetPlayerGauge()
    {
        playerActionGauge = 0;
        playerAttackGauge = 0;
        playerDefenseGauge = 0;
    }


    // =========================
    // Enemyゲージ
    // =========================

    private void AllocateEnemyGauge()
    {
        // 行動力
        enemyActionGauge =
            Random.Range(
                0,
                11
            );

        int remaining =
            10 -
            enemyActionGauge;

        // 攻撃力
        enemyAttackGauge =
            Random.Range(
                0,
                remaining + 1
            );

        remaining -=
            enemyAttackGauge;

        // 残りを防御
        enemyDefenseGauge =
            remaining;
    }


    // =========================
    // ターン処理
    // =========================

    private IEnumerator ExecuteBattleTurn()
    {
        currentState =
            BattleState.Processing;

        commandText.text = "";

        // Playerの方が速い
        if (playerActionGauge >
            enemyActionGauge)
        {
            yield return
                StartCoroutine(
                    PlayerAttack()
                );

            if (enemyCurrentHp <= 0)
            {
                yield return
                    StartCoroutine(
                        WinBattle()
                    );

                yield break;
            }

            yield return
                StartCoroutine(
                    EnemyAttack()
                );

            if (playerStatus.currentHp <= 0)
            {
                yield return
                    StartCoroutine(
                        LoseBattle()
                    );

                yield break;
            }
        }

        // 敵の方が速い
        else if (enemyActionGauge >
                 playerActionGauge)
        {
            yield return
                StartCoroutine(
                    EnemyAttack()
                );

            if (playerStatus.currentHp <= 0)
            {
                yield return
                    StartCoroutine(
                        LoseBattle()
                    );

                yield break;
            }

            yield return
                StartCoroutine(
                    PlayerAttack()
                );

            if (enemyCurrentHp <= 0)
            {
                yield return
                    StartCoroutine(
                        WinBattle()
                    );

                yield break;
            }
        }

        // 同速
        else
        {
            bool playerFirst =
                Random.Range(0, 2) == 0;

            if (playerFirst)
            {
                yield return
                    StartCoroutine(
                        PlayerAttack()
                    );

                if (enemyCurrentHp <= 0)
                {
                    yield return
                        StartCoroutine(
                            WinBattle()
                        );

                    yield break;
                }

                yield return
                    StartCoroutine(
                        EnemyAttack()
                    );

                if (playerStatus.currentHp <= 0)
                {
                    yield return
                        StartCoroutine(
                            LoseBattle()
                        );

                    yield break;
                }
            }
            else
            {
                yield return
                    StartCoroutine(
                        EnemyAttack()
                    );

                if (playerStatus.currentHp <= 0)
                {
                    yield return
                        StartCoroutine(
                            LoseBattle()
                        );

                    yield break;
                }

                yield return
                    StartCoroutine(
                        PlayerAttack()
                    );

                if (enemyCurrentHp <= 0)
                {
                    yield return
                        StartCoroutine(
                            WinBattle()
                        );

                    yield break;
                }
            }
        }

        StartNextTurn();
    }


    // =========================
    // Player攻撃
    // =========================

    private IEnumerator PlayerAttack()
    {
        int damage =
            playerStatus.attack +
            playerAttackGauge -
            enemyDefenseGauge;

        damage =
            Mathf.Max(
                damage,
                1
            );

        enemyCurrentHp -=
            damage;

        enemyCurrentHp =
            Mathf.Max(
                enemyCurrentHp,
                0
            );

        messageText.text =
            "プレイヤーの攻撃！" +
            "\n" +
            BattleSceneData.EnemyName +
            "に" +
            damage +
            "ダメージ！";

        UpdateHpText();

        yield return new WaitForSeconds(
            1f
        );
    }


    // =========================
    // Enemy攻撃
    // =========================

    private IEnumerator EnemyAttack()
    {
        int damage =
            BattleSceneData.EnemyAttack +
            enemyAttackGauge -
            playerDefenseGauge;

        damage =
            Mathf.Max(
                damage,
                1
            );

        playerStatus.currentHp -=
            damage;

        playerStatus.currentHp =
            Mathf.Max(
                playerStatus.currentHp,
                0
            );

        messageText.text =
            BattleSceneData.EnemyName +
            "の攻撃！" +
            "\nプレイヤーに" +
            damage +
            "ダメージ！";

        UpdateHpText();

        yield return new WaitForSeconds(
            1f
        );
    }


    // =========================
    // 次のターン
    // =========================

    private void StartNextTurn()
    {
        ResetPlayerGauge();

        AllocateEnemyGauge();

        currentState =
            BattleState.Command;

        commandIndex = 0;

        messageText.text =
            "コマンドを選んでください。";

        UpdateCommandText();

        // ゲージは消さずに更新
        UpdateGaugeText();
    }


    // =========================
    // アイテム
    // =========================

    private void OpenItemMenu()
    {
        currentState =
            BattleState.Item;

        itemIndex = 0;

        messageText.text =
            "使うどうぐを選んでください。";

        UpdateItemText();
    }


    private void ItemInput()
    {
        if (Keyboard.current.wKey
                .wasPressedThisFrame ||
            Keyboard.current.upArrowKey
                .wasPressedThisFrame)
        {
            itemIndex--;

            if (itemIndex < 0)
            {
                itemIndex =
                    itemCommands.Length - 1;
            }

            UpdateItemText();
        }


        if (Keyboard.current.sKey
                .wasPressedThisFrame ||
            Keyboard.current.downArrowKey
                .wasPressedThisFrame)
        {
            itemIndex++;

            if (itemIndex >=
                itemCommands.Length)
            {
                itemIndex = 0;
            }

            UpdateItemText();
        }


        // Escapeでも戻る
        if (Keyboard.current.escapeKey
                .wasPressedThisFrame)
        {
            OpenCommandMenu();

            messageText.text =
                "コマンドを選んでください。";

            return;
        }


        if (!IsConfirmPressed())
        {
            return;
        }


        switch (itemIndex)
        {
            // やくそう
            case 0:

                TryUseHerb();

                break;


            // きずぐすり
            case 1:

                TryUsePotion();

                break;


            // もどる
            case 2:

                OpenCommandMenu();

                messageText.text =
                    "コマンドを選んでください。";

                break;
        }
    }


    private void TryUseHerb()
    {
        if (PlayerInventory.HerbCount <= 0)
        {
            messageText.text =
                "やくそうを持っていない！";

            return;
        }

        if (playerStatus.currentHp >=
            playerStatus.maxHp)
        {
            messageText.text =
                "HPは満タンだ！";

            return;
        }

        PlayerInventory.HerbCount--;

        StartCoroutine(
            UseHealItem(
                "やくそう",
                30
            )
        );
    }


    private void TryUsePotion()
    {
        if (PlayerInventory.PotionCount <= 0)
        {
            messageText.text =
                "きずぐすりを持っていない！";

            return;
        }

        if (playerStatus.currentHp >=
            playerStatus.maxHp)
        {
            messageText.text =
                "HPは満タンだ！";

            return;
        }

        PlayerInventory.PotionCount--;

        StartCoroutine(
            UseHealItem(
                "きずぐすり",
                60
            )
        );
    }


    private IEnumerator UseHealItem(
        string itemName,
        int healAmount)
    {
        currentState =
            BattleState.Processing;

        commandText.text = "";

        int oldHp =
            playerStatus.currentHp;

        playerStatus.currentHp +=
            healAmount;

        playerStatus.currentHp =
            Mathf.Min(
                playerStatus.currentHp,
                playerStatus.maxHp
            );

        int actualHeal =
            playerStatus.currentHp -
            oldHp;

        messageText.text =
            itemName +
            "を使った！" +
            "\nHPが" +
            actualHeal +
            "回復した！";

        UpdateHpText();

        yield return new WaitForSeconds(
            1f
        );

        // アイテム使用後は敵のターン
        yield return
            StartCoroutine(
                EnemyAttack()
            );

        if (playerStatus.currentHp <= 0)
        {
            yield return
                StartCoroutine(
                    LoseBattle()
                );

            yield break;
        }

        StartNextTurn();
    }


    private void UpdateItemText()
    {
        commandText.text = "";

        for (int i = 0;
             i < itemCommands.Length;
             i++)
        {
            if (i == itemIndex)
            {
                commandText.text +=
                    "▶ ";
            }
            else
            {
                commandText.text +=
                    "　";
            }


            if (i == 0)
            {
                commandText.text +=
                    "やくそう ×" +
                    PlayerInventory.HerbCount;
            }
            else if (i == 1)
            {
                commandText.text +=
                    "きずぐすり ×" +
                    PlayerInventory.PotionCount;
            }
            else
            {
                commandText.text +=
                    "もどる";
            }


            if (i <
                itemCommands.Length - 1)
            {
                commandText.text +=
                    "\n";
            }
        }
    }


    // =========================
    // 勝利
    // =========================

    private IEnumerator WinBattle()
    {
        currentState =
            BattleState.BattleEnd;

        commandText.text = "";
        gaugeText.text = "";

        // 倒したEnemyIDを登録
        if (!string.IsNullOrEmpty(
            BattleSceneData.CurrentEnemyId))
        {
            BattleSceneData
                .DefeatedEnemyIds
                .Add(
                    BattleSceneData
                        .CurrentEnemyId
                );
        }


        // -------------------------
        // ドロップ処理
        // -------------------------

        List<string> drops =
            MonsterDropSystem.GiveDrops();


        messageText.text =
            BattleSceneData.EnemyName +
            "を倒した！";


        if (drops.Count > 0)
        {
            messageText.text +=
                "\n\nドロップ！";

            foreach (string drop
                     in drops)
            {
                messageText.text +=
                    "\n" +
                    drop +
                    "を手に入れた！";
            }
        }
        else
        {
            messageText.text +=
                "\n\nドロップ品はなかった。";
        }


        yield return new WaitForSeconds(
            2f
        );


        SceneManager.LoadScene(
            BattleSceneData.ReturnSceneName
        );
    }


    // =========================
    // 敗北
    // =========================

    private IEnumerator LoseBattle()
    {
        currentState =
            BattleState.BattleEnd;

        commandText.text = "";
        gaugeText.text = "";

        messageText.text =
            "プレイヤーは倒れた……";

        // ドロップは入手しない
        BattleSceneData
            .CurrentDrops
            .Clear();

        yield return new WaitForSeconds(
            2f
        );

        SceneManager.LoadScene(
            BattleSceneData.ReturnSceneName
        );
    }


    // =========================
    // 逃げる
    // =========================

    private IEnumerator PlayerRun()
    {
        currentState =
            BattleState.Processing;

        commandText.text = "";
        gaugeText.text = "";

        messageText.text =
            "プレイヤーは逃げ出した！";


        // 戦っていたモンスターを
        // デスポーン対象にする
        if (!string.IsNullOrEmpty(
            BattleSceneData.CurrentEnemyId))
        {
            BattleSceneData
                .DespawnedEnemyIds
                .Add(
                    BattleSceneData
                        .CurrentEnemyId
                );
        }


        // 逃げた場合ドロップなし
        BattleSceneData
            .CurrentDrops
            .Clear();


        yield return new WaitForSeconds(
            1f
        );


        SceneManager.LoadScene(
            BattleSceneData.ReturnSceneName
        );
    }


    // =========================
    // HP表示
    // =========================

    private void UpdateHpText()
    {
        if (playerHpText != null)
        {
            playerHpText.text =
                "HP " +
                playerStatus.currentHp +
                " / " +
                playerStatus.maxHp;
        }


        if (enemyHpText != null)
        {
            enemyHpText.text =
                BattleSceneData.EnemyName +
                "\nHP " +
                enemyCurrentHp +
                " / " +
                BattleSceneData.EnemyMaxHp;
        }
    }


    // =========================
    // ゲージ表示
    // =========================

    private void UpdateGaugeText()
    {
        if (gaugeText == null)
        {
            return;
        }


        gaugeText.text =
            "【プレイヤー】" +
            "\n行動：" +
            playerActionGauge +
            "　攻撃：" +
            playerAttackGauge +
            "　防御：" +
            playerDefenseGauge +
            "\n\n" +
            "【" +
            BattleSceneData.EnemyName +
            "】" +
            "\n行動：" +
            enemyActionGauge +
            "　攻撃：" +
            enemyAttackGauge +
            "　防御：" +
            enemyDefenseGauge;
    }


    // =========================
    // 決定キー
    // =========================

    private bool IsConfirmPressed()
    {
        return
            Keyboard.current.fKey
                .wasPressedThisFrame ||
            Keyboard.current.enterKey
                .wasPressedThisFrame;
    }


    // =========================
    // 数字キー
    // =========================

    private int GetPressedNumber()
    {
        if (Keyboard.current.digit0Key
                .wasPressedThisFrame ||
            Keyboard.current.numpad0Key
                .wasPressedThisFrame)
        {
            return 0;
        }

        if (Keyboard.current.digit1Key
                .wasPressedThisFrame ||
            Keyboard.current.numpad1Key
                .wasPressedThisFrame)
        {
            return 1;
        }

        if (Keyboard.current.digit2Key
                .wasPressedThisFrame ||
            Keyboard.current.numpad2Key
                .wasPressedThisFrame)
        {
            return 2;
        }

        if (Keyboard.current.digit3Key
                .wasPressedThisFrame ||
            Keyboard.current.numpad3Key
                .wasPressedThisFrame)
        {
            return 3;
        }

        if (Keyboard.current.digit4Key
                .wasPressedThisFrame ||
            Keyboard.current.numpad4Key
                .wasPressedThisFrame)
        {
            return 4;
        }

        if (Keyboard.current.digit5Key
                .wasPressedThisFrame ||
            Keyboard.current.numpad5Key
                .wasPressedThisFrame)
        {
            return 5;
        }

        if (Keyboard.current.digit6Key
                .wasPressedThisFrame ||
            Keyboard.current.numpad6Key
                .wasPressedThisFrame)
        {
            return 6;
        }

        if (Keyboard.current.digit7Key
                .wasPressedThisFrame ||
            Keyboard.current.numpad7Key
                .wasPressedThisFrame)
        {
            return 7;
        }

        if (Keyboard.current.digit8Key
                .wasPressedThisFrame ||
            Keyboard.current.numpad8Key
                .wasPressedThisFrame)
        {
            return 8;
        }

        if (Keyboard.current.digit9Key
                .wasPressedThisFrame ||
            Keyboard.current.numpad9Key
                .wasPressedThisFrame)
        {
            return 9;
        }

        return -1;
    }
}