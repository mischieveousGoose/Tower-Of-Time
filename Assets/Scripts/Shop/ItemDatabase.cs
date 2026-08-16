using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public List<Item> items;

    public Item PullItem()
    {
        if (items == null || items.Count == 0)
        {
            Debug.LogError("ItemDatabase.items is empty.");
            return null;
        }

        Item item = items[Random.Range(0, items.Count)];
        return item;
    }
}
