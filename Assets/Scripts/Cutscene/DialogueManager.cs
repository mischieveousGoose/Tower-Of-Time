using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public List<Dialogue> conversation;

    public Image face;
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    public float dialogueSpeed;

    public int thisDialogue;

    int cIndex = -1;
    public bool startedDialogue;
    SFXManager sfxManager;

    public GameObject dialogueBox;

    void Start()
    {
        sfxManager = FindFirstObjectByType<SFXManager>();
        if (thisDialogue == 0) StartDialogue();


    }

    public void StartDialogue()
    {
        if (startedDialogue) return;
        startedDialogue = true;
        StartCoroutine(ContinueConversation());
    }


    void Update()
    {
        dialogueBox.SetActive(startedDialogue);
        if (Input.GetKeyDown(KeyCode.Space) && startedDialogue)
        {
            StartCoroutine(ContinueConversation());
        }
    }

    IEnumerator ContinueConversation()
    {
        cIndex++;

        if (cIndex >= conversation.Count)
        {
            EndConversation();

        }
        else
        {
            Dialogue dialogue = conversation[cIndex];
            nameText.text = dialogue.cName;
            face.sprite = dialogue.face;
            dialogueText.maxVisibleCharacters = 0;
            dialogueText.text = dialogue.text;

            while (dialogueText.maxVisibleCharacters < dialogueText.text.Length)
            {

                yield return new WaitForSecondsRealtime(dialogueSpeed);
                dialogueText.maxVisibleCharacters++;
                sfxManager.PlaySFX(3);
            }
        }
    }

    void EndConversation()
    {
        startedDialogue = false;
        if (thisDialogue == 0)
            GameManager.Instance.RetryTutorial();
        else
        {
            FindFirstObjectByType<Boss>().StartFight();
        }
    }
}
