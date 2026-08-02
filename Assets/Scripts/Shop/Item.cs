using UnityEngine;


public abstract class Item : ScriptableObject
{
    public Sprite icon;
    public string itemName;
    public float cost;

    public bool disableSell;

    public bool hasMaxCount;
    public int maxCount;

    [TextArea(3, 7)]
    public string description;
}
