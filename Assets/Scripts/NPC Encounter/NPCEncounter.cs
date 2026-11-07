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
    public Image npcImage;

    void Start()
    {
        npcData = dialogues[Random.Range(0, dialogues.Count)];
        npcImage.sprite = npcData.npcSprite;
        DialogueManager.Instance.OnEndDialogue.AddListener(SpawnChoices);
        DialogueManager.Instance.StartDialogue(npcData.dialogue);
    }

    void OnDisable()
    {
        DialogueManager.Instance.OnEndDialogue.RemoveListener(SpawnChoices);
    }

    void SpawnChoices()
    {
        for (int i = 0; i < npcData.choices.Count; i++)
        {
            int index = i;
            GameObject choice = Instantiate(choicePrefab, choiceParent.transform);
            choice.transform.GetChild(0).GetComponent<TMP_Text>().text = npcData.choices[i].choiceName;
            choice.transform.GetChild(1).GetComponent<TMP_Text>().text = npcData.choices[i].choiceDescription;
            choice.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected(index));
        }
    }

    public void OnChoiceSelected(int choiceIndex)
    {
        NPCChoice selectedChoice = npcData.choices[choiceIndex];

        Debug.Log("Selected choice: " + selectedChoice.choiceName);

        foreach (ChoiceEffect effect in selectedChoice.effects)
        {
            if (effect != null)
                effect.Apply();
        }

        SceneTransitions.Instance.TransitionToScene("Roadmap");
    }

}

