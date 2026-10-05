using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class FishData
{
    public string fishName = "魚";
    public float cursorSpeed = 1.5f;

    [Range(0.05f, 1f)]
    public float successZoneWidth = 0.2f;
}

public class FishingTimingGame : MonoBehaviour
{
    // UI
    [SerializeField] private GameObject fishingPanel;
    [SerializeField] private RectTransform gaugeBackground;
    [SerializeField] private RectTransform successZone;
    [SerializeField] private RectTransform cursor;

    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text fishText;
    [SerializeField] private TMP_Text resultText;

    // Fish
    [SerializeField] private FishData[] fishes;

    private FishData currentFish;

    private float cursorPosition;
    private float cursorSpeed;

    private float successZoneCenter;
    private float successZoneWidth;

    private int cursorDirection = 1;

    private bool isFishing;


    private void Start()
    {
        // FishingSceneに入ったらPanelは表示しておく
        fishingPanel.SetActive(true);

        isFishing = false;

        statusText.text =
            "Fで釣りを始める";

        fishText.text = "";

        resultText.text = "";
    }


    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }


       

        if (!isFishing)
        {
            if (Keyboard.current.fKey
                .wasPressedThisFrame)
            {
                StartFishing();
            }

            return;
        }


       

        MoveCursor();

        UpdateCursorUI();


        // Spaceでタイミング判定
        if (Keyboard.current.spaceKey
            .wasPressedThisFrame)
        {
            CheckResult();
        }
    }


   

    public void StartFishing()
    {
        if (fishes == null ||
            fishes.Length == 0)
        {
            Debug.LogWarning(
                "Fishの設定がありません。"
            );

            return;
        }


        // ランダムで魚選択
        currentFish =
            fishes[
                Random.Range(
                    0,
                    fishes.Length
                )
            ];


        cursorSpeed =
            currentFish.cursorSpeed;

        successZoneWidth =
            currentFish.successZoneWidth;



        float halfWidth =
            successZoneWidth / 2f;

        successZoneCenter =
            Random.Range(
                halfWidth,
                1f - halfWidth
            );


        cursorPosition = 0f;

        cursorDirection = 1;

        isFishing = true;


      
        fishingPanel.SetActive(true);

        fishText.text =
            currentFish.fishName +
            "がかかった！";

        resultText.text =
            "Spaceでタイミングよく止める";

        statusText.text =
            "釣り中...";


        UpdateSuccessZoneUI();

        UpdateCursorUI();
    }


   

    private void MoveCursor()
    {
        cursorPosition +=
            cursorDirection *
            cursorSpeed *
            Time.deltaTime;


        if (cursorPosition >= 1f)
        {
            cursorPosition = 1f;

            cursorDirection = -1;
        }
        else if (cursorPosition <= 0f)
        {
            cursorPosition = 0f;

            cursorDirection = 1;
        }
    }


    
    private void CheckResult()
    {
        float successMin =
            successZoneCenter -
            successZoneWidth / 2f;

        float successMax =
            successZoneCenter +
            successZoneWidth / 2f;


        bool isSuccess =
            cursorPosition >= successMin &&
            cursorPosition <= successMax;


        if (isSuccess)
        {
            resultText.text =
                currentFish.fishName +
                "を釣り上げた！";
        }
        else
        {
            resultText.text =
                currentFish.fishName +
                "に逃げられた...";
        }


        statusText.text =
            "Fでもう一度釣る";


        isFishing = false;
    }


   
    private void UpdateCursorUI()
    {
        float gaugeWidth =
            gaugeBackground.rect.width;


        float x =
            Mathf.Lerp(
                -gaugeWidth / 2f,
                gaugeWidth / 2f,
                cursorPosition
            );


        cursor.anchoredPosition =
            new Vector2(
                x,
                cursor.anchoredPosition.y
            );
    }


   

    private void UpdateSuccessZoneUI()
    {
        float gaugeWidth =
            gaugeBackground.rect.width;


        float zoneWidth =
            gaugeWidth *
            successZoneWidth;


        float x =
            Mathf.Lerp(
                -gaugeWidth / 2f,
                gaugeWidth / 2f,
                successZoneCenter
            );


        successZone.sizeDelta =
            new Vector2(
                zoneWidth,
                successZone.sizeDelta.y
            );


        successZone.anchoredPosition =
            new Vector2(
                x,
                successZone.anchoredPosition.y
            );
    }
}