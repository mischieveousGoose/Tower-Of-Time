using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Encounter", menuName = "NPC Encounter")]
public class NPCEncounterData : ScriptableObject
{
    public Dialogue dialogue;
    public List<NPCChoice> choices;
    public Sprite npcSprite;
}