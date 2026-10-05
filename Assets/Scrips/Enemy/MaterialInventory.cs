using System.Collections.Generic;

public static class MaterialInventory
{
    private static Dictionary
        <DropItemType, int> materials =
        new Dictionary<DropItemType, int>();


    public static int GetCount(
        DropItemType type)
    {
        if (!materials.ContainsKey(type))
        {
            return 0;
        }

        return materials[type];
    }


    public static void Add(
        DropItemType type,
        int amount)
    {
        if (!materials.ContainsKey(type))
        {
            materials[type] = 0;
        }

        materials[type] += amount;
    }


    public static bool Remove(
        DropItemType type,
        int amount)
    {
        int currentCount =
            GetCount(type);

        if (currentCount < amount)
        {
            return false;
        }

        materials[type] -= amount;

        return true;
    }


    public static bool Has(
        DropItemType type,
        int amount)
    {
        return GetCount(type) >= amount;
    }
}