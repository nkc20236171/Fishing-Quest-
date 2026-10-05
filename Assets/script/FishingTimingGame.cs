using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum FishGimmick
{
    None,
    MovingSuccessZone
}

[System.Serializable]
public class FishData
{
    [Header("魚")]
    public string fishName = "魚";
    public string rarity = "Common";

    [Min(1)]
    public int spawnWeight = 10;

    [Header("釣りの難易度")]
    public float cursorSpeed = 1.5f;

    [Range(0.05f, 1f)]
    public float successZoneWidth = 0.2f;

    [Header("特殊ギミック")]
    public FishGimmick gimmick = FishGimmick.None;

    [Range(0f, 1f)]
    public float successZoneMoveSpeed = 0.2f;

    [Header("報酬")]
    public string weaponName = "武器";

    [Range(0f, 1f)]
    public float weaponDropChance = 0.1f;

    public string materialName = "素材";

    [Min(1)]
    public int materialAmount = 1;

    [Min(1)]
    public int materialsRequiredForCraft = 5;
}

public class FishingTimingGame : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject fishingPanel;
    [SerializeField] private RectTransform gaugeBackground;
    [SerializeField] private RectTransform successZone;
    [SerializeField] private RectTransform cursor;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text fishText;
    [SerializeField] private TMP_Text resultText;

    [Header("Fish")]
    [SerializeField] private FishData[] fishes;

    private readonly Dictionary<string, int> materials = new Dictionary<string, int>();
    private readonly HashSet<string> ownedWeapons = new HashSet<string>();

    private FishData currentFish;

    private float cursorPosition;
    private float cursorSpeed;
    private int cursorDirection = 1;

    private float successZoneCenter;
    private float successZoneWidth;
    private int successZoneDirection = 1;

    private FishGimmick currentGimmick;
    private float successZoneMoveSpeed;

    private bool isFishing;

    private void Start()
    {
        fishingPanel.SetActive(false);
        statusText.text = "Fで釣りを始める";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isFishing)
        {
            StartFishing();
        }

        if (Input.GetKeyDown(KeyCode.C) && !isFishing && currentFish != null)
        {
            CraftWeapon(currentFish);
        }

        if (!isFishing) return;

        MoveCursor();

        if (currentGimmick == FishGimmick.MovingSuccessZone)
        {
            MoveSuccessZone();
            UpdateSuccessZoneUI();
        }

        UpdateCursorUI();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckResult();
        }
    }

    public void StartFishing()
    {
        if (fishes == null || fishes.Length == 0)
        {
            Debug.LogWarning("Fishの設定がありません。");
            return;
        }

        currentFish = ChooseFishByWeight();

        cursorSpeed = currentFish.cursorSpeed;
        successZoneWidth = currentFish.successZoneWidth;
        currentGimmick = currentFish.gimmick;
        successZoneMoveSpeed = currentFish.successZoneMoveSpeed;

        float halfWidth = successZoneWidth / 2f;
        successZoneCenter = Random.Range(halfWidth, 1f - halfWidth);

        cursorPosition = 0f;
        cursorDirection = 1;
        successZoneDirection = 1;
        isFishing = true;

        fishingPanel.SetActive(true);

        fishText.text = currentFish.rarity + "  " + currentFish.fishName + "がかかった！";

        if (currentGimmick == FishGimmick.MovingSuccessZone)
        {
            fishText.text += "\n成功エリアが動く！";
        }

        resultText.text = "Spaceでタイミングよく止める";
        statusText.text = "釣り中...";

        UpdateSuccessZoneUI();
        UpdateCursorUI();
    }

    private FishData ChooseFishByWeight()
    {
        int totalWeight = 0;

        foreach (FishData fish in fishes)
        {
            totalWeight += fish.spawnWeight;
        }

        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (FishData fish in fishes)
        {
            currentWeight += fish.spawnWeight;

            if (randomValue < currentWeight)
            {
                return fish;
            }
        }

        return fishes[0];
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

    private void MoveSuccessZone()
    {
        float halfWidth = successZoneWidth / 2f;

        successZoneCenter += successZoneDirection * successZoneMoveSpeed * Time.deltaTime;

        if (successZoneCenter >= 1f - halfWidth)
        {
            successZoneCenter = 1f - halfWidth;
            successZoneDirection = -1;
        }
        else if (successZoneCenter <= halfWidth)
        {
            successZoneCenter = halfWidth;
            successZoneDirection = 1;
        }
    }

    private void CheckResult()
    {
        float successMin = successZoneCenter - successZoneWidth / 2f;
        float successMax = successZoneCenter + successZoneWidth / 2f;

        bool isSuccess = cursorPosition >= successMin && cursorPosition <= successMax;

        if (isSuccess)
        {
            GiveReward();
        }
        else
        {
            resultText.text = currentFish.fishName + "に逃げられた...";
        }

        statusText.text = "Fでもう一度釣る";
        isFishing = false;
    }

    private void GiveReward()
    {
        bool gotWeapon = Random.value < currentFish.weaponDropChance;

        if (gotWeapon && !ownedWeapons.Contains(currentFish.weaponName))
        {
            ownedWeapons.Add(currentFish.weaponName);

            resultText.text =
                currentFish.fishName + "を釣り上げた！\n" +
                currentFish.weaponName + "を手に入れた！";
        }
        else
        {
            AddMaterial(currentFish.materialName, currentFish.materialAmount);

            int currentAmount = GetMaterialAmount(currentFish.materialName);

            resultText.text =
                currentFish.fishName + "を釣り上げた！\n" +
                currentFish.materialName + "を" + currentFish.materialAmount + "個手に入れた！\n" +
                "所持数: " + currentAmount + " / " + currentFish.materialsRequiredForCraft;

            if (currentAmount >= currentFish.materialsRequiredForCraft &&
                !ownedWeapons.Contains(currentFish.weaponName))
            {
                resultText.text += "\nCで" + currentFish.weaponName + "を作れる！";
            }
        }
    }

    private void CraftWeapon(FishData fish)
    {
        if (ownedWeapons.Contains(fish.weaponName))
        {
            resultText.text = fish.weaponName + "は、すでに持っています。";
            return;
        }

        int currentAmount = GetMaterialAmount(fish.materialName);

        if (currentAmount < fish.materialsRequiredForCraft)
        {
            resultText.text =
                fish.materialName + "が足りません。\n" +
                "所持数: " + currentAmount + " / " + fish.materialsRequiredForCraft;
            return;
        }

        materials[fish.materialName] -= fish.materialsRequiredForCraft;
        ownedWeapons.Add(fish.weaponName);

        resultText.text =
            fish.materialName + "を" + fish.materialsRequiredForCraft + "個使った。\n" +
            fish.weaponName + "を作った！";
    }

    private void AddMaterial(string materialName, int amount)
    {
        if (!materials.ContainsKey(materialName))
        {
            materials[materialName] = 0;
        }

        materials[materialName] += amount;
    }

    private int GetMaterialAmount(string materialName)
    {
        if (materials.ContainsKey(materialName))
        {
            return materials[materialName];
        }

        return 0;
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