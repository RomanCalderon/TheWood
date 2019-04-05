using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem instance { get; set; }

    public delegate void DialogueProgressHandler();
    public static event DialogueProgressHandler OnDialogueFinished;

    Player player;

    public GameObject dialoguePanel;
    [HideInInspector]
    public string npcName;
    public List<string> dialogueLines = new List<string>();
    public bool IsOpen = false;

    [Header("UI")]
    [SerializeField]
    Button continueButton;
    [SerializeField]
    Text dialogueText, nameText;
    int dialogueIndex;

    private void Awake()
    {
        continueButton.onClick.AddListener(delegate { ContinueDialogue(); });
        dialoguePanel.SetActive(IsOpen = false);

        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        dialoguePanel.SetActive(IsOpen);

        if (IsOpen)
            if (Input.GetKeyDown(KeyCode.Return))
                ContinueDialogue();
    }
    
    public void AddNewDialogue(string npcName, string[] lines)
    {
        dialogueIndex = 0;
        dialogueLines = new List<string>(lines.Length);
        dialogueLines.AddRange(lines);
        this.npcName = npcName;
        
        CreateDialogue();
    }

    // Starts a dialogue
    public void CreateDialogue()
    {
        dialogueText.text = dialogueLines[dialogueIndex];
        nameText.text = npcName;
        IsOpen = true;

        UIEventHandler.UIDisplayed(true);
    }

    public void ContinueDialogue()
    {
        // Continue dialogue
        if (dialogueIndex < dialogueLines.Count - 1)
        {
            dialogueIndex++;
            dialogueText.text = dialogueLines[dialogueIndex];
        }
        // Finished dialogue
        else
        {
            IsOpen = false;
            UIEventHandler.UIDisplayed(false);
            FinishedDialogue();
        }
    }

    // Events
    public static void FinishedDialogue()
    {
        OnDialogueFinished?.Invoke();
    }
}
