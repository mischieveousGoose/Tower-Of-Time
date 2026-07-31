using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerItems : MonoBehaviour
{
    List<Item> inv { get { return InventoryManager.Instance.inventory; } }
    LevelLoader loader;
    void Start()
    {
        loader = FindFirstObjectByType<LevelLoader>();

    }

    public int CountItem(string item)
    {

        int count = 0;
        foreach (Item i in inv)
        {
            if (i.itemName == item) count++;
        }
        return count;
    }

    public float CalculateSpeedBoost()
    {
        float boost = 1;
        foreach (Item i in inv)
        {
            if (i.itemName == "Speed Potion") boost += 0.25f;
        }
        return boost;
    }

    public float CalculateEnemyRewardMult()
    {
        float mult = 1;
        foreach (Item i in inv)
        {
            if (i.itemName == "Hourglass") mult += 0.15f;
        }

        return mult;
    }


}
