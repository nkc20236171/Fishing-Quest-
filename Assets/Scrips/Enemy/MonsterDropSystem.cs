using System.Collections.Generic;
using UnityEngine;

public static class MonsterDropSystem
{
    public static List<string>
        GiveDrops()
    {
        List<string> result =
            new List<string>();

        foreach (
            MonsterDropEntry drop
            in BattleSceneData.CurrentDrops)
        {
            float random =
                Random.Range(
                    0f,
                    100f
                );

            if (random >
                drop.dropRate)
            {
                continue;
            }

            int amount =
                Random.Range(
                    drop.minAmount,
                    drop.maxAmount + 1
                );

            MaterialInventory.Add(
                drop.itemType,
                amount
            );

            string itemName =
                DropItemDatabase
                    .GetItemName(
                        drop.itemType
                    );

            result.Add(
                itemName +
                " Å~" +
                amount
            );
        }

        BattleSceneData
            .CurrentDrops
            .Clear();

        return result;
    }
}