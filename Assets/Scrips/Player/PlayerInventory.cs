using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // =========================
    // 通常アイテム
    // =========================

    public static int HerbCount = 3;
    public static int MagicWaterCount = 0;
    public static int PotionCount = 1;

    public int herbCount
    {
        get { return HerbCount; }
        set { HerbCount = value; }
    }

    public int magicWaterCount
    {
        get { return MagicWaterCount; }
        set { MagicWaterCount = value; }
    }

    public int potionCount
    {
        get { return PotionCount; }
        set { PotionCount = value; }
    }

    // =========================
    // 釣具
    // =========================

    public static int FishingRodCount = 0;
    public static int ReelCount = 0;
    public static int BaitCount = 0;

    public int fishingRodCount
    {
        get { return FishingRodCount; }
        set { FishingRodCount = value; }
    }

    public int reelCount
    {
        get { return ReelCount; }
        set { ReelCount = value; }
    }

    public int baitCount
    {
        get { return BaitCount; }
        set { BaitCount = value; }
    }

    // =========================
    // 武器・防具
    // =========================

    public static int CopperSwordCount = 0;
    public static int IronSwordCount = 0;
    public static int WoodenShieldCount = 0;
    public static int LeatherArmorCount = 0;

    public int copperSwordCount
    {
        get { return CopperSwordCount; }
        set { CopperSwordCount = value; }
    }

    public int ironSwordCount
    {
        get { return IronSwordCount; }
        set { IronSwordCount = value; }
    }

    public int woodenShieldCount
    {
        get { return WoodenShieldCount; }
        set { WoodenShieldCount = value; }
    }

    public int leatherArmorCount
    {
        get { return LeatherArmorCount; }
        set { LeatherArmorCount = value; }
    }
}