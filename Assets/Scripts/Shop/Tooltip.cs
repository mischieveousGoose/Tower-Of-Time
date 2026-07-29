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
        if (hover != null)
        {
            tooltip.SetActive(true);
            Item hoverItem = shopManager.inventory[hover.transform.GetSiblingIndex()];

            ttHead.text = hoverItem.itemName;
            ttBody.text = hoverItem.description;
            if (hoverItem.disableSell)
            {
                ttSellText.gameObject.SetActive(false);
            }
            else
            {
                ttSellText.gameObject.SetActive(true);
                ttSellText.text = "Right click to sell for " + hoverItem.cost / 2 + "s.";
            }

            tooltip.transform.position = Input.mousePosition;

            if (Input.GetMouseButtonDown(1) && !hoverItem.disableSell)
            {
                shopManager.SellItem(hover.transform.GetSiblingIndex());
            }
        }
        else
        {
            tooltip.SetActive(false);
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