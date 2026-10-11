using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPCEncounter : MonoBehaviour
{
    public List<NPCEncounterData> dialogues;
    NPCEncounterData npcData;

    [Header("UI")]
    public GameObject choiceParent;
    public GameObject choicePrefab;

    void Start()
    {
        npcData = dialogues[Random.Range(0, dialogues.Count)];
        DialogueManager.Instance.OnEndDialogue.AddListener(SpawnChoices);
        DialogueManager.Instance.StartDialogue(npcData.dialogue);
    }

    void OnDisable()
    {
        DialogueManager.Instance.OnEndDialogue.RemoveListener(SpawnChoices);
    }

    void SpawnChoices()
    {
        for (int i = 0; i < npcData.dialogue.lines.Count; i++)
        {
            GameObject choice = Instantiate(choicePrefab, choiceParent.transform);
            choice.transform.GetChild(0).GetComponent<TMP_Text>().text = npcData.choices[i];
            choice.transform.GetChild(1).GetComponent<TMP_Text>().text = npcData.descriptions[i];
            choice.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected(i));
        }
    }

    public void OnChoiceSelected(int choiceIndex)
    {
        Debug.Log("Selected choice: " + npcData.choices[choiceIndex]);
    }

}

[CreateAssetMenu(fileName = "New NPC Encounter", menuName = "NPC Encounter")]
public class NPCEncounterData : ScriptableObject
{
    public Dialogue dialogue;
    public string[] choices;
    public string[] descriptions;
}