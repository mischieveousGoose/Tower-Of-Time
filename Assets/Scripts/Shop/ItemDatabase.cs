using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public List<Item> items;

    public Item PullItem()
    {
        return items[Random.Range(0, items.Count)];
    }
}
