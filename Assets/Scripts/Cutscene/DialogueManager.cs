using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    private Dialogue conversation;

    [SerializeField] private Image face;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;

    [SerializeField] private float dialogueSpeed;

    int cIndex = -1;
    [HideInInspector] public bool inDialogue;
    SFXManager sfxManager;

    public GameObject dialogueBox;
    [HideInInspector] public UnityEvent OnEndDialogue;


    public static DialogueManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    void Start()
    {
        sfxManager = FindFirstObjectByType<SFXManager>();
    }

    public void StartDialogue(Dialogue conversation)
    {
        if (inDialogue) return;
        this.conversation = conversation;
        cIndex = -1;
        inDialogue = true;
        StartCoroutine(ContinueConversation());
    }

    void Update()
    {
        dialogueBox.SetActive(inDialogue);
        if (InputManager.Instance.InteractPerformed && inDialogue)
        {
            StartCoroutine(ContinueConversation());
        }
    }

    IEnumerator ContinueConversation()
    {
        cIndex++;

        if (cIndex >= conversation.lines.Count)
        {
            EndConversation();

        }
        else
        {
            DialogueLine line = conversation.lines[cIndex];
            nameText.text = line.cName;
            face.sprite = line.face;
            dialogueText.maxVisibleCharacters = 0;
            dialogueText.text = line.text;

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
        inDialogue = false;
        OnEndDialogue?.Invoke();
        OnEndDialogue.RemoveAllListeners();
    }
}
