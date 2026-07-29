using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public Sprite face;
    public string cName;

    [TextArea(3, 5)]
    public string text;
}
