using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class NPCChoice
{
    [SerializeReference]
    public List<ChoiceEffect> effects = new();

    public string choiceName;

    [TextArea(3, 10)]
    public string choiceDescription;
}


public abstract class ChoiceEffect : ScriptableObject
{
    public abstract void Apply();
}