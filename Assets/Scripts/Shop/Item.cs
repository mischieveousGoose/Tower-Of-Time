using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Item")]
public class Item : ScriptableObject
{
    public Sprite icon;
    public string itemName;
    public float cost;

    public bool hasMaxCount;
    public int maxCount;

    public bool disableSell;

    [TextArea(3, 7)]
    public string description;
}
