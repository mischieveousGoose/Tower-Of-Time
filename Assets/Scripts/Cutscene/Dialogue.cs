using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DialogueLine
{
    public Sprite face;
    public string cName;

    [TextArea(3, 5)]
    public string text;
}

[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> lines;
}
