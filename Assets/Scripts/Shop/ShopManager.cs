using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    SFXManager sfxManager;
    List<Item> stock = new List<Item>();
    ItemDatabase db;
    public float time;
    public float[] costMultipliers;

    float shopTimer;

    LevelLoader loader;

    List<Item> inventory
    {
        get
        {
            return InventoryManager.Instance.inventory;
        }
    }


    [Header("UI")]
    public List<Image> itemIcons;
    public List<TMP_Text> itemNameTexts;
    public List<TMP_Text> itemDescTexts;
    public List<TMP_Text> itemCostTexts;

    public TMP_Text moneyText;
    public TMP_Text nextLevelText;
    public GameObject inventoryPrefab;
    public Transform inventoryParent;
    SettingsManager settingsManager;


    void Start()
    {
        settingsManager = FindFirstObjectByType<SettingsManager>();
        sfxManager = FindFirstObjectByType<SFXManager>();
        Time.timeScale = 1;
        shopTimer = 1;
        db = GetComponent<ItemDatabase>();
        loader = FindFirstObjectByType<LevelLoader>();
        time = loader.time;

        if (loader.level + 1 == loader.bossLevel)
        {
            nextLevelText.text = "-> Final Floor";
        }
        else
        {
            nextLevelText.text = "-> Floor " + (loader.level + 1).ToString() + "/10";
        }

        RefreshStock();
        RenderShop();
        SetupInventory();
    }

    void SetupInventory()
    {
        foreach (Item item in inventory)
        {
            Instantiate(inventoryPrefab, inventoryParent).GetComponent<Image>().sprite = item.icon;
        }
    }

    public void Reroll()
    {
        if (time >= 5)
        {
            sfxManager.PlaySFX(2);
            time -= 5;
            RefreshStock();
            RenderShop();
        }
    }

    void RefreshStock()
    {
        while (stock.Count < 3)
        {
            stock.Add(null);
        }

        for (int i = 0; i < 3; i++)
        {
            stock[i] = PullValidItem();
        }
    }

    int GetItemCount(string itemName)
    {
        int c = 0;
        foreach (Item i in inventory)
        {
            if (itemName == i.itemName) c++;
        }
        return c;
    }

    int GetStockCount(string itemName)
    {
        int c = 0;
        foreach (Item i in stock)
        {
            if (itemName == i.itemName) c++;
        }
        return c;
    }

    Item PullValidItem()
    {
        Item i = db.PullItem();
        while (i.hasMaxCount && GetItemCount(i.itemName) + GetStockCount(i.itemName) >= i.maxCount)
        {
            i = db.PullItem();
        }

        return i;
    }

    public void SellItem(int i)
    {
        sfxManager.PlaySFX(2);
        Destroy(inventoryParent.GetChild(i).gameObject);
        time += GetItemCost(inventory[i], true) / 2;
        inventory.RemoveAt(i);
        RenderShop();

    }

    void RenderShop()
    {
        moneyText.text = RoundFloat(time).ToString() + "s";
        for (int i = 0; i < 3; i++)
        {
            itemIcons[i].sprite = stock[i].icon;
            itemNameTexts[i].text = stock[i].itemName;
            itemCostTexts[i].text = RoundFloat(stock[i].cost * (1 - (GetItemCount("Coupon") * 0.1f))).ToString() + "s";
            itemDescTexts[i].text = stock[i].description.ToString();
        }
    }

    float RoundFloat(float val)
    {
        return Mathf.Round(val * 100) / 100;
    }

    float GetItemCost(Item item, bool baseC = false)
    {
        float baseCost = item.cost * costMultipliers[settingsManager.difficulty];
        if (baseC) return baseCost;
        return baseCost * (1 - (GetItemCount("Coupon") * 0.1f));
    }

    public void BuyItem(int i)
    {
        if (shopTimer > 0f) return;
        Item buyItem = stock[i];
        if (time < GetItemCost(buyItem)) return;

        sfxManager.PlaySFX(2);
        time -= GetItemCost(buyItem);

        while (buyItem.itemName == "Mystery Box") buyItem = PullValidItem();
        inventory.Add(buyItem);

        Instantiate(inventoryPrefab, inventoryParent).GetComponent<Image>().sprite = buyItem.icon;

        if (buyItem.itemName == "Credit Card") time += 40;

        stock[i] = PullValidItem();

        RenderShop();
    }

    public void Continue()
    {
        loader.LoadLevel("SampleScene");
    }

    void Update()
    {
        shopTimer -= Time.deltaTime;
    }

}
