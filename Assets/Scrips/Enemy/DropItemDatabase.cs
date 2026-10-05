public static class DropItemDatabase
{
    public static string GetItemName(
        DropItemType type)
    {
        switch (type)
        {
            case DropItemType.SlimeGel:
                return "スライムゼリー";

            case DropItemType.GoblinFang:
                return "ゴブリンの牙";

            case DropItemType.WolfPelt:
                return "オオカミの毛皮";

            case DropItemType.SkeletonBone:
                return "骨のかけら";

            case DropItemType.GolemFragment:
                return "ゴーレムの欠片";
        }

        return "不明な素材";
    }

    public static int GetSellPrice(
        DropItemType type)
    {
        switch (type)
        {
            case DropItemType.SlimeGel:
                return 8;

            case DropItemType.GoblinFang:
                return 15;

            case DropItemType.WolfPelt:
                return 20;

            case DropItemType.SkeletonBone:
                return 18;

            case DropItemType.GolemFragment:
                return 30;
        }

        return 0;
    }
}