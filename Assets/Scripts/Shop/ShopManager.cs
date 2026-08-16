using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    SFXManager sfxManager;
    List<Item> stock = new List<Item>();
    ItemDatabase db;
    public float time;

    float shopTimer;
    List<Item> inventory => InventoryManager.Instance.inventory;

    [HideInInspector] public GameObject primaryWeapon;
    [HideInInspector] public GameObject secondaryWeapon;



    [Header("UI")]
    public List<Image> itemIcons;
    public List<TMP_Text> itemNameTexts;
    public List<TMP_Text> itemDescTexts;
    public List<TMP_Text> itemCostTexts;

    public Image primaryWPIcon;
    public Image secondaryWPIcon;

    public TMP_Text moneyText;
    public TMP_Text nextLevelText;
    public GameObject inventoryPrefab;
    public Transform inventoryParent;
    SettingsManager settingsManager;

    GameManager gameManager => GameManager.Instance;
    RunData runData => GameManager.Instance.runData;


    void Start()
    {
        settingsManager = FindFirstObjectByType<SettingsManager>();
        sfxManager = FindFirstObjectByType<SFXManager>();
        Time.timeScale = 1;
        shopTimer = 1;
        db = GetComponent<ItemDatabase>();
        time = gameManager.runData.time;

        primaryWeapon = runData.primaryWeapon;
        secondaryWeapon = runData.secondaryWeapon;

        if (runData.level + 1 == gameManager.bossLevel)
        {
            nextLevelText.text = "-> Final Floor";
        }
        else
        {
            nextLevelText.text = "-> Floor " + (runData.level + 1).ToString() + "/10";
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
        stock.Clear();

        if (db == null || db.items == null || db.items.Count == 0)
        {
            Debug.LogError("ItemDatabase is missing or empty.");
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            Item item = PullValidItem();
            if (item != null)
                stock.Add(item);
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
        time += inventory[i].cost / 2;
        inventory.RemoveAt(i);
        RenderShop();

    }

    void RenderShop()
    {
        moneyText.text = RoundFloat(time).ToString() + "s";

        for (int i = 0; i < 3; i++)
        {
            if (stock[i] == null)
            {
                itemIcons[i].enabled = false;
                itemNameTexts[i].text = "Out of Stock";
                itemCostTexts[i].text = "";
                itemDescTexts[i].text = "";
            }
            else
            {
                itemIcons[i].enabled = true;
                itemIcons[i].sprite = stock[i].icon;
                itemNameTexts[i].text = stock[i].itemName;
                itemCostTexts[i].text = RoundFloat(GetItemCost(stock[i])).ToString() + "s";
                itemDescTexts[i].text = stock[i].description.ToString();
            }
        }

        if (primaryWeapon == null) primaryWPIcon.enabled = false;
        else
        {
            primaryWPIcon.enabled = true;
            primaryWPIcon.sprite = primaryWeapon.GetComponent<Weapon>().item.icon;
        }


        if (secondaryWeapon == null) secondaryWPIcon.enabled = false;
        else
        {
            secondaryWPIcon.enabled = true;
            secondaryWPIcon.sprite = secondaryWeapon.GetComponent<Weapon>().item.icon;
        }
    }

    float RoundFloat(float val)
    {
        return Mathf.Round(val * 100) / 100;
    }

    float GetItemCost(Item item)
    {
        return item.cost * StatsManager.Instance.GetStat(StatType.ShopPrices);
    }



    public void BuyItem(int i)
    {
        if (stock[i] == null) return;
        if (shopTimer > 0f) return;
        Item buyItem = stock[i];
        if (time < GetItemCost(buyItem)) return;

        if (buyItem is WeaponItem && WeaponsFull()) return;

        sfxManager.PlaySFX(2);
        time -= GetItemCost(buyItem);



        if (buyItem is IOnBuy onBuy)
        {
            onBuy.OnBuy(this);
        }



        if (!(buyItem is WeaponItem))
        {
            Instantiate(inventoryPrefab, inventoryParent).GetComponent<Image>().sprite = buyItem.icon;
            inventory.Add(buyItem);
        }
        StatsManager.Instance.CalculateStats();

        stock[i] = null;

        RenderShop();
    }

    bool WeaponsFull()
    {
        return primaryWeapon != null && secondaryWeapon != null;
    }

    public void SwitchWeaponSlots()
    {
        GameObject temp = primaryWeapon;
        primaryWeapon = secondaryWeapon;
        secondaryWeapon = temp;
        RenderShop();
    }

    public void AddWeapon(GameObject weapon)
    {
        if (primaryWeapon == null) primaryWeapon = weapon;
        else if (secondaryWeapon == null) secondaryWeapon = weapon;
    }

    public void SellWeapon(bool isPrimary)
    {
        if (isPrimary)
        {
            if (primaryWeapon == null) return;
            time += primaryWeapon.GetComponent<Weapon>().item.cost / 2;
            primaryWeapon = null;
        }
        else
        {
            if (secondaryWeapon == null) return;
            time += secondaryWeapon.GetComponent<Weapon>().item.cost / 2;
            secondaryWeapon = null;
        }

        RenderShop();
    }

    public void Continue()
    {
        gameManager.ContinueFromShop();
    }

    void Update()
    {
        shopTimer -= Time.deltaTime;
    }

}
