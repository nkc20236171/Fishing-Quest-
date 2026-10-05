using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
public class FishingTimingGame : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject fishingPanel;
    [SerializeField] private RectTransform gaugeBackground;
    [SerializeField] private RectTransform successZone;
    [SerializeField] private RectTransform cursor;
    [SerializeField] private TMP_Text resultText;

    [Header("Gauge")]
    [SerializeField] private float cursorSpeed = 1.5f;
    [SerializeField] private float successZoneCenter = 0.5f;
    [SerializeField] private float successZoneWidth = 0.2f;

    private float cursorPosition;
    private int cursorDirection = 1;
    private bool isFishing;

    void Start()
    {
        fishingPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isFishing)
        {
            StartFishing();
        }

        if (!isFishing) return;

        MoveCursor();
        UpdateCursorUI();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckResult();
        }
    }

    public void StartFishing()
    {
        cursorPosition = 0f;
        cursorDirection = 1;
        isFishing = true;

        fishingPanel.SetActive(true);
        resultText.text = "Spaceで止める";

        UpdateSuccessZoneUI();
        UpdateCursorUI();
    }

    private void MoveCursor()
    {
        cursorPosition += cursorDirection * cursorSpeed * Time.deltaTime;

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
        float successMin = successZoneCenter - successZoneWidth / 2f;
        float successMax = successZoneCenter + successZoneWidth / 2f;

        bool isSuccess = cursorPosition >= successMin && cursorPosition <= successMax;

        resultText.text = isSuccess ? "釣り成功！" : "釣り失敗...";
        isFishing = false;
    }

    private void UpdateCursorUI()
    {
        float gaugeWidth = gaugeBackground.rect.width;
        float x = Mathf.Lerp(-gaugeWidth / 2f, gaugeWidth / 2f, cursorPosition);

        cursor.anchoredPosition = new Vector2(x, cursor.anchoredPosition.y);
    }

    private void UpdateSuccessZoneUI()
    {
        float gaugeWidth = gaugeBackground.rect.width;
        float zoneWidth = gaugeWidth * successZoneWidth;
        float x = Mathf.Lerp(-gaugeWidth / 2f, gaugeWidth / 2f, successZoneCenter);

        successZone.sizeDelta = new Vector2(zoneWidth, successZone.sizeDelta.y);
        successZone.anchoredPosition = new Vector2(x, successZone.anchoredPosition.y);
    }
}