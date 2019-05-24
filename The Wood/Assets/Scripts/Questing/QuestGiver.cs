using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : NPC
{
    public bool ProposedQuest { get; set; }
    public bool AcceptedQuest { get; set; }
    public bool FinishedQuest { get; set; }

    [Header("Quest")]
    [SerializeField] string questName;
    private Quest Quest;

    protected override void Awake()
    {
        base.Awake();

        QuestController.OnQuestAccepted += QuestAccepted;
        QuestController.OnQuestDeclined += QuestDeclined;
        QuestController.OnQuestAbandoned += QuestAbandoned;
        QuestController.OnQuestCompleted += QuestCompleted;

        interactionName = Name;

        Quest = QuestDatabase.instance.GetQuestByName(questName);
    }

    private void OnDestroy()
    {
        QuestController.OnQuestAccepted -= QuestAccepted;
        QuestController.OnQuestDeclined -= QuestDeclined;
        QuestController.OnQuestAbandoned -= QuestAbandoned;
        QuestController.OnQuestCompleted -= QuestCompleted;

        DialogueSystem.OnDialogueFinished -= DialogueSystem_OnDialogueFinished;
    }

    private void DialogueSystem_OnDialogueFinished()
    {
        // Get Quest proposal from NPC
        if (!ProposedQuest)
        {
            ProposeQuest();
        }
    }

    public override void Interact()
    {
        // Get Quest proposal from NPC
        if (!ProposedQuest)
        {
            DialogueSystem.OnDialogueFinished += DialogueSystem_OnDialogueFinished;

            base.Interact();
        }
        // Check if Quest is done
        else if (AcceptedQuest)
        {
            if (FinishedQuest)
                DialogueSystem.instance.AddNewDialogue(Name, new string[] { "Hey! Thanks for your help from earlier." });
            else
                CheckQuest();
        }
    }

    void ProposeQuest()
    {
        ProposedQuest = true;
        AcceptedQuest = false;
        FinishedQuest = false;

        Quest.InitializeQuest();
        QuestController.QuestProposed(Quest);
    }

    private void QuestAccepted(Quest quest)
    {
        if (quest.Equals(Quest))
            AcceptedQuest = true;
    }

    private void QuestDeclined(Quest quest)
    {
        if (quest.Equals(Quest))
        {
            ProposedQuest = false;
            AcceptedQuest = false;
        }
    }

    private void QuestAbandoned(Quest quest)
    {
        if (quest.Equals(Quest))
        {
            ProposedQuest = false;
            AcceptedQuest = false;
        }
    }
    
    private void QuestCompleted(Quest quest)
    {
        if (quest.QuestName != Quest.QuestName)
            return;

        print("Quest givers' quest [" + quest.QuestName + "] has been completed!");

        Quest = quest;
        ProposedQuest = true;
        AcceptedQuest = true;
    }

    /// <summary>
    /// This is called when the player returns to the Quest Giver to cash in the Quest.
    /// </summary>
    private void CheckQuest()
    {
        // Quest is done, receive reward
        if (Quest.Completed && !FinishedQuest)
        {
            FinishedQuest = true;

            Quest.GiveReward();
            QuestController.TurnInQuest(Quest);

            DialogueSystem.instance.AddNewDialogue(Name, new string[] { "Thank you! Here's your reward." });

            DialogueSystem.OnDialogueFinished -= DialogueSystem_OnDialogueFinished;
        }
        // Quest still in progress
        else
        {
            DialogueSystem.instance.AddNewDialogue(Name, new string[] { "You haven't finished this quest. Come back when you're done." });
        }
    }
    
}
