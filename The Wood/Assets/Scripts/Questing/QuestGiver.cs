﻿using System.Collections;
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

    private void Start()
    {
        QuestController.OnQuestAccepted += QuestAccepted;
        QuestController.OnQuestDeclined += QuestDeclined;
        QuestController.OnQuestAbandoned += QuestAbandoned;

        interactionName = Name;

        Quest = QuestDatabase.instance.GetQuestByName(questName);
    }

    private void OnDestroy()
    {
        QuestController.OnQuestAccepted -= QuestAccepted;
        QuestController.OnQuestDeclined -= QuestDeclined;
        QuestController.OnQuestAbandoned -= QuestAbandoned;
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

    void CheckQuest()
    {
        // Quest is done, receive reward
        if (Quest.Completed)
        {
            Quest.GiveReward();
            FinishedQuest = true;
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
