using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;


public class Tooltip : MonoBehaviour
{
    public TMP_Text ttHead;
    public TMP_Text ttBody;
    public TMP_Text ttSellText;
    public GameObject tooltip;

    ShopManager shopManager;

    void Start()
    {
        shopManager = FindFirstObjectByType<ShopManager>();
    }

    void Update()
    {
        GameObject hover = GetUIObjectUnderMouse();
        if (hover == null)
        {
            tooltip.SetActive(false);
            return;
        }

        tooltip.SetActive(true);
        tooltip.transform.position = Input.mousePosition;

        if (hover.name == "WPIcon")
        {
            int index = hover.transform.parent.GetSiblingIndex();
            Item item = (index == 0 ? shopManager.primaryWeapon : shopManager.secondaryWeapon).GetComponent<Weapon>().item;
            ItemTooltip(item, index);
        }
        else if (hover.name == "ShopIcon")
        {
            Item item = shopManager.stock[hover.transform.parent.GetSiblingIndex()];
            ItemTooltip(item, -1);
        }
        else
        {
            Item item = InventoryManager.Instance.inventory[hover.transform.GetSiblingIndex()];
            int sellIndex = hover.transform.GetSiblingIndex();
            ItemTooltip(item, sellIndex);
        }
    }

    void ItemTooltip(Item hoverItem, int sellIndex)
    {
        ttHead.text = hoverItem.itemName;
        ttBody.text = hoverItem.description;
        if (hoverItem.disableSell || sellIndex == -1)
        {
            ttSellText.gameObject.SetActive(false);
        }
        else
        {
            ttSellText.gameObject.SetActive(true);
            ttSellText.text = "Right click to sell for " + hoverItem.cost / 2 + "s.";
        }

        if (Input.GetMouseButtonDown(1) && !hoverItem.disableSell && sellIndex != -1)
        {
            shopManager.SellItem(sellIndex);
        }
    }



    GameObject GetUIObjectUnderMouse()
    {

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            // Check if this object's layer is in the mask
            if (result.gameObject.CompareTag("Item"))
            {
                return result.gameObject; // First valid hit
            }
        }

        return null;
    }
}